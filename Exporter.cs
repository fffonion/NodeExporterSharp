using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace HardwareExporter
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineUnix(this StringBuilder sb, string value = "")
        {
            return sb.Append(value).Append('\n');
        }
    }

    public struct ApcUpsInfo
    {
        public string Name { get; set; }
        public bool DeviceOnline { get; set; }
        public double UpsLoad { get; set; }
        public int RuntimeMinutesRemaining { get; set; }
        public double InputVoltage { get; set; }
        public double BatteryCharge { get; set; }
        public double BatteryVoltage { get; set; }
    }

    public class ApcUpsStatusReader
    {
        public static async Task<ApcUpsInfo> GetUpsStatusAlternativeAsync()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using (var client = new HttpClient(handler))
            {
                string html = await client.GetStringAsync("https://localhost:6547");
                var upsInfo = new ApcUpsInfo();

                var namePattern = @"<li class=""submenuheader"">([^<]+)</li>";
                var nameMatch = Regex.Match(html, namePattern);
                if (nameMatch.Success)
                {
                    upsInfo.Name = nameMatch.Groups[1].Value.Trim();
                }
                else
                {
                    upsInfo.Name = "Unknown Model";
                }

                // Generic pattern to match all value divs
                var valuePattern = @"<div class=""value"" id=""value_(\w+)"">([^<]+)</div>";
                var matches = Regex.Matches(html, valuePattern);

                foreach (Match match in matches)
                {
                    string id = match.Groups[1].Value;
                    string value = match.Groups[2].Value.Trim();

                    switch (id)
                    {
                        case "DeviceStatus":
                            upsInfo.DeviceOnline = value == "On Line";
                            break;
                        case "RealPowerPct":
                            double.TryParse(value, out double upsLoad);
                            upsInfo.UpsLoad = upsLoad;
                            break;
                        case "RuntimeRemaining":
                            int.TryParse(value, out int runtime);
                            upsInfo.RuntimeMinutesRemaining = runtime;
                            break;
                        case "InputVoltage":
                            double.TryParse(value, out double inputVoltage);
                            upsInfo.InputVoltage = inputVoltage;
                            break;
                        case "BatteryCharge":
                            double.TryParse(value, out double batteryCharge);
                            upsInfo.BatteryCharge = batteryCharge;
                            break;
                        case "VoltageDC":
                            double.TryParse(value, out double batteryVoltage);
                            upsInfo.BatteryVoltage = batteryVoltage;
                            break;
                    }
                }

                return upsInfo;
            }
        }
    }

    public sealed class UpsdConfiguration
    {
        public string Host { get; }
        public int Port { get; }

        public UpsdConfiguration(string host, int port)
        {
            Host = string.IsNullOrWhiteSpace(host) ? "127.0.0.1" : host;
            Port = port;
        }

        public static UpsdConfiguration FromEnvironment(Func<string, string?>? getEnvironmentVariable = null)
        {
            getEnvironmentVariable ??= Environment.GetEnvironmentVariable;

            string host = getEnvironmentVariable("NODE_EXPORTER_UPSD_HOST") ?? "127.0.0.1";
            string? portValue = getEnvironmentVariable("NODE_EXPORTER_UPSD_PORT");
            int port = 3493;

            if (!string.IsNullOrWhiteSpace(portValue) &&
                (!int.TryParse(portValue, NumberStyles.None, CultureInfo.InvariantCulture, out port) ||
                 port < IPEndPoint.MinPort ||
                 port > IPEndPoint.MaxPort))
            {
                throw new InvalidOperationException("NODE_EXPORTER_UPSD_PORT must be a TCP port between 0 and 65535.");
            }

            return new UpsdConfiguration(host, port);
        }
    }

    public sealed class ExporterOptions
    {
        public int Port { get; }
        public Dictionary<string, bool> EnabledCollectors { get; }
        public UpsdConfiguration Upsd { get; }

        private ExporterOptions(int port, Dictionary<string, bool> enabledCollectors, UpsdConfiguration upsd)
        {
            Port = port;
            EnabledCollectors = enabledCollectors;
            Upsd = upsd;
        }

        public static ExporterOptions Parse(string[] args, Func<string, string?>? getEnvironmentVariable = null)
        {
            int port = 9182;
            Dictionary<string, bool> enabledCollectors = new Dictionary<string, bool>
            {
                ["hwmon"] = true,
                ["netdev"] = true,
                ["cpu"] = true,
                ["memory"] = true,
                ["apcups"] = false,
                ["upsd"] = false
            };

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--port" && i + 1 < args.Length)
                {
                    if (int.TryParse(args[i + 1], NumberStyles.None, CultureInfo.InvariantCulture, out int parsedPort))
                    {
                        port = parsedPort;
                    }
                }
                else if (args[i].StartsWith("--no-"))
                {
                    string collector = args[i].Substring(5);
                    if (enabledCollectors.ContainsKey(collector))
                    {
                        enabledCollectors[collector] = false;
                    }
                }
                else if (args[i].StartsWith("--collector."))
                {
                    string collector = args[i].Substring(12);
                    if (enabledCollectors.ContainsKey(collector))
                    {
                        enabledCollectors[collector] = true;
                    }
                }
            }

            if (enabledCollectors["apcups"] && enabledCollectors["upsd"])
            {
                throw new InvalidOperationException("Collectors apcups and upsd cannot be enabled at the same time.");
            }

            return new ExporterOptions(port, enabledCollectors, UpsdConfiguration.FromEnvironment(getEnvironmentVariable));
        }
    }

    public static class UpsdStatusReader
    {
        private static readonly TimeSpan ProtocolTimeout = TimeSpan.FromSeconds(5);
        private static readonly Encoding ProtocolEncoding = Encoding.ASCII;

        public static async Task<IReadOnlyList<ApcUpsInfo>> GetUpsStatusAsync(UpsdConfiguration configuration)
        {
            using TcpClient client = await ConnectAsync(configuration);

            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, ProtocolEncoding, leaveOpen: true);
            using StreamWriter writer = new StreamWriter(stream, ProtocolEncoding, leaveOpen: true)
            {
                AutoFlush = true,
                NewLine = "\n"
            };

            string upsListResponse = await SendCommandAsync(reader, writer, "LIST UPS");
            List<ApcUpsInfo> results = new List<ApcUpsInfo>();

            foreach (string upsName in ParseUpsNames(upsListResponse))
            {
                string variablesResponse = await SendCommandAsync(reader, writer, $"LIST VAR {upsName}");
                Dictionary<string, string> variables = ParseVariables(upsName, variablesResponse);
                results.Add(ToUpsInfo(upsName, variables));
            }

            await writer.WriteLineAsync("LOGOUT");
            return results;
        }

        private static async Task<TcpClient> ConnectAsync(UpsdConfiguration configuration)
        {
            if (IPAddress.TryParse(configuration.Host, out IPAddress? address))
            {
                return await ConnectToAddressAsync(address, configuration.Port);
            }

            IPAddress[] addresses = await Dns.GetHostAddressesAsync(configuration.Host).WaitAsync(ProtocolTimeout);
            foreach (IPAddress candidate in addresses.OrderBy(address => address.AddressFamily == AddressFamily.InterNetwork ? 0 : 1))
            {
                try
                {
                    return await ConnectToAddressAsync(candidate, configuration.Port);
                }
                catch
                {
                    // Try the next resolved address before reporting failure to the caller.
                }
            }

            throw new InvalidOperationException($"Could not connect to upsd at {configuration.Host}:{configuration.Port}.");
        }

        private static async Task<TcpClient> ConnectToAddressAsync(IPAddress address, int port)
        {
            TcpClient client = new TcpClient(address.AddressFamily);
            try
            {
                await client.ConnectAsync(address, port).WaitAsync(ProtocolTimeout);
                return client;
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        public static IEnumerable<string> ParseUpsNames(string response)
        {
            foreach (string line in SplitLines(response))
            {
                Match match = Regex.Match(line, @"^UPS\s+(\S+)(?:\s+.*)?$");
                if (match.Success)
                {
                    yield return match.Groups[1].Value;
                }
            }
        }

        public static Dictionary<string, string> ParseVariables(string upsName, string response)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();

            foreach (string line in SplitLines(response))
            {
                Match match = Regex.Match(line, "^VAR\\s+(\\S+)\\s+(\\S+)\\s+\"(.*)\"$");
                if (!match.Success || match.Groups[1].Value != upsName)
                {
                    continue;
                }

                variables[match.Groups[2].Value] = UnescapeUpsdValue(match.Groups[3].Value);
            }

            return variables;
        }

        public static ApcUpsInfo ToUpsInfo(string upsName, IReadOnlyDictionary<string, string> variables)
        {
            return new ApcUpsInfo
            {
                Name = GetFirstValue(variables, "ups.model", "device.model") ?? upsName,
                DeviceOnline = IsOnline(GetValueOrEmpty(variables, "ups.status")),
                UpsLoad = ParseDouble(GetValueOrEmpty(variables, "ups.load")),
                RuntimeMinutesRemaining = ParseRuntimeMinutes(GetValueOrEmpty(variables, "battery.runtime")),
                InputVoltage = ParseDouble(GetValueOrEmpty(variables, "input.voltage")),
                BatteryCharge = ParseDouble(GetValueOrEmpty(variables, "battery.charge")),
                BatteryVoltage = ParseDouble(GetValueOrEmpty(variables, "battery.voltage"))
            };
        }

        private static async Task<string> SendCommandAsync(StreamReader reader, StreamWriter writer, string command)
        {
            await writer.WriteLineAsync(command);
            string firstLine = await ReadLineAsync(reader);

            if (firstLine.StartsWith("ERR ", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"upsd returned {firstLine}");
            }

            StringBuilder response = new StringBuilder();
            response.AppendLineUnix(firstLine);

            if (!firstLine.StartsWith("BEGIN ", StringComparison.OrdinalIgnoreCase))
            {
                return response.ToString();
            }

            while (true)
            {
                string line = await ReadLineAsync(reader);
                response.AppendLineUnix(line);

                if (line.StartsWith("END ", StringComparison.OrdinalIgnoreCase))
                {
                    return response.ToString();
                }
            }
        }

        private static async Task<string> ReadLineAsync(StreamReader reader)
        {
            string? line = await reader.ReadLineAsync().WaitAsync(ProtocolTimeout);
            return line ?? throw new InvalidOperationException("upsd closed the connection unexpectedly.");
        }

        private static IEnumerable<string> SplitLines(string value)
        {
            return value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim());
        }

        private static string UnescapeUpsdValue(string value)
        {
            return value.Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        private static string? GetFirstValue(IReadOnlyDictionary<string, string> variables, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (variables.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }

        private static string GetValueOrEmpty(IReadOnlyDictionary<string, string> variables, string key)
        {
            return variables.TryGetValue(key, out string? value) ? value : string.Empty;
        }

        private static double ParseDouble(string value)
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result) ? result : 0;
        }

        private static int ParseRuntimeMinutes(string value)
        {
            double seconds = ParseDouble(value);
            return (int)(seconds / 60);
        }

        private static bool IsOnline(string status)
        {
            string[] tokens = status.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return tokens.Any(token => token.Equals("OL", StringComparison.OrdinalIgnoreCase)) ||
                   status.Contains("ONLINE", StringComparison.OrdinalIgnoreCase) ||
                   status.Contains("ON LINE", StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class UpsMetricsWriter
    {
        public static void AppendMetrics(StringBuilder metrics, ApcUpsInfo upsInfo)
        {
            string name = EscapeLabelValue(upsInfo.Name);

            metrics.AppendLineUnix("# HELP node_ups_device_online Whether the UPS device is online");
            metrics.AppendLineUnix("# TYPE node_ups_device_online gauge");
            metrics.AppendLineUnix(FormattableString.Invariant($"node_ups_device_online{{name=\"{name}\"}} {(upsInfo.DeviceOnline ? 1 : 0)}"));
            metrics.AppendLineUnix("# HELP node_ups_load_percent UPS load percentage");
            metrics.AppendLineUnix("# TYPE node_ups_load_percent gauge");
            metrics.AppendLineUnix(FormattableString.Invariant($"node_ups_load_percent{{name=\"{name}\"}} {upsInfo.UpsLoad}"));
            metrics.AppendLineUnix("# HELP node_ups_runtime_minutes_remaining UPS runtime in minutes remaining");
            metrics.AppendLineUnix("# TYPE node_ups_runtime_minutes_remaining gauge");
            metrics.AppendLineUnix(FormattableString.Invariant($"node_ups_runtime_minutes_remaining{{name=\"{name}\"}} {upsInfo.RuntimeMinutesRemaining}"));
            metrics.AppendLineUnix("# HELP node_ups_input_voltage UPS input voltage in volts");
            metrics.AppendLineUnix("# TYPE node_ups_input_voltage gauge");
            metrics.AppendLineUnix(FormattableString.Invariant($"node_ups_input_voltage{{name=\"{name}\"}} {upsInfo.InputVoltage}"));
            metrics.AppendLineUnix("# HELP node_ups_battery_charge UPS battery charge in percentage");
            metrics.AppendLineUnix("# TYPE node_ups_battery_charge gauge");
            metrics.AppendLineUnix(FormattableString.Invariant($"node_ups_battery_charge{{name=\"{name}\"}} {upsInfo.BatteryCharge}"));
            metrics.AppendLineUnix("# HELP node_ups_battery_voltage UPS battery voltage in volts");
            metrics.AppendLineUnix("# TYPE node_ups_battery_voltage gauge");
            metrics.AppendLineUnix(FormattableString.Invariant($"node_ups_battery_voltage{{name=\"{name}\"}} {upsInfo.BatteryVoltage}"));
        }

        private static string EscapeLabelValue(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\"", "\\\"");
        }
    }

    class Program
    {
        private static readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
        private static HttpListener _listener;
        private static Dictionary<string, bool> _enabledCollectors = new Dictionary<string, bool>();
        private static int _port = 9182; // Default port similar to node_exporter
        private static UpsdConfiguration _upsdConfiguration = new UpsdConfiguration("127.0.0.1", 3493);
        private static int errorsCount = 0;
        private static Computer computer;
        private static Dictionary<string, bool> readSensors = new Dictionary<string, bool>(); // updated sensors in current read

        static void Main(string[] args)
        {

            Console.WriteLine("Hardware Metrics Exporter");
            Console.WriteLine("------------------------");

            try
            {
                ParseArgs(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Configuration error: {ex.Message}");
                Environment.Exit(1);
                return;
            }

            PrintEnabledCollectors();

            computer = new Computer
            {
                IsCpuEnabled = _enabledCollectors["hwmon"] || _enabledCollectors["cpu"],
                IsGpuEnabled = _enabledCollectors["hwmon"],
                IsMemoryEnabled = _enabledCollectors["memory"],
                IsMotherboardEnabled = _enabledCollectors["hwmon"],
                //IsControllerEnabled = true,
                IsNetworkEnabled = _enabledCollectors["netdev"],
                IsStorageEnabled = _enabledCollectors["hwmon"]
            };

            // Initialize LibreHardwareMonitor
            computer.Open();

            // Setup HTTP listener
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{_port}/");
            _listener.Start();
            Console.WriteLine($"Server started on port {_port}");

            // Start processing requests
            Task.Run(() => ProcessRequests());

            // Setup shutdown handler
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                _shutdown.Set();
            };

            // Wait for shutdown signal
            _shutdown.WaitOne();
            _listener.Stop();
            Console.WriteLine("Server stopped");
        }

        private static void ParseArgs(string[] args)
        {
            ExporterOptions options = ExporterOptions.Parse(args);
            _port = options.Port;
            _enabledCollectors = options.EnabledCollectors;
            _upsdConfiguration = options.Upsd;
        }

        private static void PrintEnabledCollectors()
        {
            Console.WriteLine("Enabled collectors:");
            foreach (var collector in _enabledCollectors)
            {
                Console.WriteLine($"  {collector.Key}: {(collector.Value ? "enabled" : "disabled")}");
            }
        }

        private static async Task ProcessRequests()
        {
            while (_listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = await _listener.GetContextAsync();
                    ProcessRequest(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing request: {ex.Message}");
                }
            }
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                string path = context.Request.Url.AbsolutePath;
                if (path == "/metrics")
                {
                    readSensors.Clear();
                    StringBuilder metrics = new StringBuilder();

                    try {
                        if (_enabledCollectors["hwmon"])
                            CollectHardwareMonitorMetrics(computer, metrics);

                        if (_enabledCollectors["netdev"])
                            CollectNetworkMetrics(computer, metrics);

                        if (_enabledCollectors["cpu"])
                            CollectCpuMetrics(computer, metrics);

                        if (_enabledCollectors["memory"])
                            CollectMemoryMetrics(computer, metrics);

                        if (_enabledCollectors["apcups"])
                            CollectApcUpsMetrics(metrics);

                        if (_enabledCollectors["upsd"])
                            CollectUpsdMetrics(metrics);
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error opening librehardwaremonitor: {ex.Message} {ex.StackTrace}");
                        errorsCount += 1;
                    }

                    // Add build info
                    metrics.AppendLineUnix("# HELP node_exporter_build_info Build information");
                    metrics.AppendLineUnix("# TYPE node_exporter_build_info gauge");
                    metrics.AppendLineUnix($"node_exporter_build_info{{version=\"{Assembly.GetExecutingAssembly().GetName().Version}\"}} 1");

                    // Add Errors total
                    metrics.AppendLineUnix("# HELP node_exporter_errors_total Errors total");
                    metrics.AppendLineUnix("# TYPE node_exporter_errors_total counter");
                    metrics.AppendLineUnix($"node_exporter_errors_total {errorsCount}");

                    // Send response
                    byte[] buffer = Encoding.UTF8.GetBytes(metrics.ToString());
                    context.Response.ContentType = "text/plain; version=0.0.4";
                    context.Response.ContentLength64 = buffer.Length;
                    context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serving request: {ex.Message}");
                context.Response.StatusCode = 500;
            }
            finally
            {
                context.Response.Close();
            }
        }

        private static void updateHardwareSensor(IHardware hardware)
        {
            if(!readSensors.ContainsKey(hardware.Identifier.ToString()))
            {
                hardware.Update();
                readSensors[hardware.Identifier.ToString()] = true;
            }
        }

        private static void CollectHardwareMonitorMetrics(Computer computer, StringBuilder metrics)
        {
            try
            {
                // chip names mapping
                var chipNames = new Dictionary<string, string>();
                var hardwareNames = new Dictionary<string, string>();

                var temps = new List<ISensor>();
                var pwms = new List<ISensor>();
                var rpms = new List<ISensor>();

                foreach (var hardware in computer.Hardware)
                {
                    // Report chip
                    string chipName = hardware.Name.Replace("\"", "\\\"");
                    updateHardwareSensor(hardware);

                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.Value.HasValue)
                        {
                            string sensorName = sensor.Name.Replace("\"", "\\\"");

                            if (sensor.SensorType == SensorType.Temperature)
                            {
                                temps.Add(sensor);
                                chipNames[sensor.Identifier.ToString()] = sensorName;
                                hardwareNames[hardware.Identifier.ToString()] = hardware.Name;
                            }
                            else if (sensor.SensorType == SensorType.Control) // Fan PWM
                            {
                                pwms.Add(sensor);
                                chipNames[sensor.Identifier.ToString()] = sensorName;
                                hardwareNames[hardware.Identifier.ToString()] = hardware.Name;
                            }
                            else if (sensor.SensorType == SensorType.Fan)
                            {
                                rpms.Add(sensor);
                                chipNames[sensor.Identifier.ToString()] = sensorName;
                                hardwareNames[hardware.Identifier.ToString()] = hardware.Name;
                            }
                        }
                    }

                    // Also check subhardware
                    foreach (var subhardware in hardware.SubHardware)
                    {
                        updateHardwareSensor(subhardware);

                        foreach (var sensor in subhardware.Sensors)
                        {
                            if (sensor.Value.HasValue)
                            {
                                string sensorName = sensor.Name.Replace("\"", "\\\"");

                                if (sensor.SensorType == SensorType.Temperature)
                                {
                                    temps.Add(sensor);
                                    chipNames[sensor.Identifier.ToString()] = sensorName;
                                    hardwareNames[hardware.Identifier.ToString()] = hardware.Name;

                                }
                                else if (sensor.SensorType == SensorType.Control) // Fan PWM
                                {
                                    pwms.Add(sensor);
                                    chipNames[sensor.Identifier.ToString()] = sensorName;
                                    hardwareNames[hardware.Identifier.ToString()] = hardware.Name;
                                }
                                else if (sensor.SensorType == SensorType.Fan)
                                {
                                    rpms.Add(sensor);
                                    chipNames[sensor.Identifier.ToString()] = sensorName;
                                    hardwareNames[hardware.Identifier.ToString()] = hardware.Name;
                                }
                            }
                        }
                    }
                }


                // Chip names
                metrics.AppendLineUnix("# HELP node_hwmon_chip_names Hardware monitoring chip names");
                metrics.AppendLineUnix("# TYPE node_hwmon_chip_names gauge");
                // All chip names mapping
                foreach (var chipName in chipNames)
                {
                    metrics.AppendLineUnix($"node_hwmon_chip_names{{chip=\"{chipName.Key}\",chip_name=\"{chipName.Value}\"}} 1");
                }

                // Hardware names
                metrics.AppendLineUnix("# HELP node_hwmon_hardware_names Hardware monitoring hardware names");
                metrics.AppendLineUnix("# TYPE node_hwmon_hardware_names gauge");
                // All hardware names mapping
                foreach (var hardwareName in hardwareNames)
                {
                    metrics.AppendLineUnix($"node_hwmon_hardware_names{{hardware=\"{hardwareName.Key}\",hardware_name=\"{hardwareName.Value}\"}} 1");
                }

                // Temperature metrics
                metrics.AppendLineUnix("# HELP node_hwmon_temp_celsius Hardware monitoring temperature");
                metrics.AppendLineUnix("# TYPE node_hwmon_temp_celsius gauge");
                foreach (var sensor in temps)
                {
                    metrics.AppendLineUnix($"node_hwmon_temp_celsius{{chip=\"{sensor.Identifier}\",hardware=\"{sensor.Hardware.Identifier}\"}} {sensor.Value.Value}");
                }

                // PWM metrics
                metrics.AppendLineUnix("# HELP node_hwmon_pwm Hardware monitoring fan PWM");
                metrics.AppendLineUnix(
                    "# TYPE node_hwmon_pwm gauge");
                foreach (var sensor in pwms)
                {
                    metrics.AppendLineUnix($"node_hwmon_pwm{{chip=\"{sensor.Identifier}\",hardware=\"{sensor.Hardware.Identifier}\"}} {sensor.Value.Value / 100 * 255}");
                }

                // Fan RPM metrics
                metrics.AppendLineUnix("# HELP node_hwmon_fan_rpm Hardware monitoring fan RPM");
                metrics.AppendLineUnix(
                    "# TYPE node_hwmon_fan_rpm gauge");
                foreach (var sensor in rpms)
                {
                    metrics.AppendLineUnix($"node_hwmon_fan_rpm{{chip=\"{sensor.Identifier}\",hardware=\"{sensor.Hardware.Identifier}\"}} {sensor.Value.Value}");
                }
            }
            finally
            {
            }
        }

        private static void CollectNetworkMetrics(Computer _, StringBuilder metrics)
        {

            PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");
            String[] instancename = category.GetInstanceNames();
            Dictionary<string, float> txReadings = new Dictionary<string, float>();
            Dictionary<string, float> rxReadings = new Dictionary<string, float>();

            foreach (string name in instancename)
            {
                // Using RawValue gives the accumulation instead of per second calculation
                using (var counter = new PerformanceCounter("Network Interface", "Bytes Received/sec", name, true))
                {
                    rxReadings[name] = counter.RawValue;
                }
                using (var counter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name, true))
                {
                    txReadings[name] = counter.RawValue;
                }
            }
            metrics.AppendLineUnix("# HELP node_network_receive_bytes_total Network device statistic receive_bytes");
            metrics.AppendLineUnix("# TYPE node_network_receive_bytes_total counter");
            foreach(var name in instancename)
            {
                metrics.AppendLineUnix($"node_network_receive_bytes_total{{device=\"{name}\"}} {rxReadings[name]}");
            }

            metrics.AppendLineUnix("# HELP node_network_transmit_bytes_total Network device statistic transmit_bytes");
            metrics.AppendLineUnix("# TYPE node_network_transmit_bytes_total counter");
            foreach (var name in instancename)
            {
                metrics.AppendLineUnix($"node_network_transmit_bytes_total{{device=\"{name}\"}} {txReadings[name]}");
            }
        }

        private static void CollectCpuMetrics(Computer computer, StringBuilder metrics)
        {
            PerformanceCounterCategory category = new PerformanceCounterCategory("Processor Information");
            String[] instancename = category.GetInstanceNames();

            metrics.AppendLineUnix("# HELP node_cpu_seconds_total Time that processor spent in different modes (dpc, idle, interrupt, privileged, user)");
            metrics.AppendLineUnix("# TYPE node_cpu_seconds_total counter");

            foreach (string name in instancename)
            {
                if (name.EndsWith("_Total"))
                    continue;

                // Using RawValue gives the accumulation instead of per second calculation
                using (var counter = new PerformanceCounter("Processor Information", "% DPC Time", name, true))
                {
                    metrics.AppendLineUnix($"node_cpu_seconds_total{{cpu=\"{name}\",mode=\"dpc\"}} {counter.RawValue / 10000000f}");
                }
                using (var counter = new PerformanceCounter("Processor Information", "% Idle Time", name, true))
                {
                    metrics.AppendLineUnix($"node_cpu_seconds_total{{cpu=\"{name}\",mode=\"idle\"}} {counter.RawValue / 10000000f}");
                }
                using (var counter = new PerformanceCounter("Processor Information", "% Interrupt Time", name, true))
                {
                    metrics.AppendLineUnix($"node_cpu_seconds_total{{cpu=\"{name}\",mode=\"interrupt\"}} {counter.RawValue / 10000000f}");
                }
                using (var counter = new PerformanceCounter("Processor Information", "% Privileged Time", name, true))
                {
                    metrics.AppendLineUnix($"node_cpu_seconds_total{{cpu=\"{name}\",mode=\"privileged\"}} {counter.RawValue / 10000000f}");
                }
                using (var counter = new PerformanceCounter("Processor Information", "% User Time", name, true))
                {
                    metrics.AppendLineUnix($"node_cpu_seconds_total{{cpu=\"{name}\",mode=\"user\"}} {counter.RawValue / 10000000f}");
                }
            }

            // Second metric - CPU Power Consumption
            metrics.AppendLineUnix("# HELP node_cpu_power_watts CPU power consumption in watts");
            metrics.AppendLineUnix("# TYPE node_cpu_power_watts gauge");

            // Output all CPU power metrics
            foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == HardwareType.Cpu))
            {
                string cpuName = hardware.Name.Replace("\"", "\\\"");

                // Package/Total power
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Power)
                    {
                        string powerType = sensor.Name.Contains("Package") ? "package" :
                                          sensor.Name.Contains("Core") ? "cores" :
                                          sensor.Name.ToLower().Replace(" ", "_");

                        metrics.AppendLineUnix($"node_cpu_power_watts{{cpu=\"{cpuName}\", type=\"{powerType}\"}} {sensor.Value}");
                    }
                }
            }
        }

        private static void CollectMemoryMetrics(Computer computer, StringBuilder metrics)
        {

            try
            {
                // Default values in case we can't get the actual metrics
                float? usedMemory = null;
                float? availableMemory = null;
                float? usedVirtualMemory = null;
                float? availableVirtualMemory = null;

                // Find the memory hardware
                foreach (var hardware in computer.Hardware)
                {
                    if (hardware.HardwareType == HardwareType.Memory)
                    {
                        // Update the sensors
                        updateHardwareSensor(hardware);

                        // Process all sensors
                        foreach (var sensor in hardware.Sensors)
                        {
                            switch (sensor.Name)
                            {
                                case "Memory Used":
                                    usedMemory = sensor.Value;
                                    break;
                                case "Memory Available":
                                    availableMemory = sensor.Value;
                                    break;
                                case "Virtual Memory Used":
                                    usedVirtualMemory = sensor.Value;
                                    break;
                                case "Virtual Memory Available":
                                    availableVirtualMemory = sensor.Value;
                                    break;
                            }
                        }

                        break; // Only process the first memory hardware
                    }
                }

                // Convert from MB to bytes
                const long bytesInGB = 1024 * 1024 * 1024;

                // Output Total Memory metric with help and type
                metrics.AppendLineUnix("# HELP node_memory_MemTotal_bytes Memory information field MemTotal_bytes");
                metrics.AppendLineUnix("# TYPE node_memory_MemTotal_bytes gauge");
                if (availableMemory.HasValue && usedMemory.HasValue)
                {
                    metrics.AppendLineUnix($"node_memory_MemTotal_bytes {(long)((availableMemory.Value + usedMemory.Value) * bytesInGB)}");
                }
                else
                {
                    metrics.AppendLineUnix("node_memory_MemTotal_bytes 0");
                }

                // Output Available Memory metric with help and type
                metrics.AppendLineUnix("# HELP node_memory_MemAvailable_bytes Memory information field MemAvailable_bytes");
                metrics.AppendLineUnix("# TYPE node_memory_MemAvailable_bytes gauge");
                if (availableMemory.HasValue)
                {
                    metrics.AppendLineUnix($"node_memory_MemAvailable_bytes {(long)(availableMemory.Value * bytesInGB)}");
                }
                else
                {
                    metrics.AppendLineUnix("node_memory_MemAvailable_bytes 0");
                }

                // Output Virtual Memory Total metric with help and type
                metrics.AppendLineUnix("# HELP node_memory_VirtualMemoryTotal_bytes Memory information field VirtualMemoryTotal_bytes");
                metrics.AppendLineUnix("# TYPE node_memory_VirtualMemoryTotal_bytes gauge");
                if (availableVirtualMemory.HasValue && usedVirtualMemory.HasValue)
                {
                    metrics.AppendLineUnix($"node_memory_VirtualMemoryTotal_bytes {(long)((availableVirtualMemory.Value + usedVirtualMemory.Value) * bytesInGB)}");
                }
                else
                {
                    metrics.AppendLineUnix("node_memory_VirtualMemoryTotal_bytes 0");
                }

                // Output Virtual Memory Available metric with help and type
                metrics.AppendLineUnix(
                    "# HELP node_memory_VirtualMemoryAvailable_bytes Memory information field VirtualMemoryAvailable_bytes");
                metrics.AppendLineUnix("# TYPE node_memory_VirtualMemoryAvailable_bytes gauge");
                if (availableVirtualMemory.HasValue)
                {
                    metrics.AppendLineUnix($"node_memory_VirtualMemoryAvailable_bytes {(long)(availableVirtualMemory.Value * bytesInGB)}");
                }
                else
                {
                    metrics.AppendLineUnix("node_memory_VirtualMemoryAvailable_bytes 0");
                }



            }
            catch (Exception ex)
            {
                // Log the exception but don't re-throw to prevent metrics collection failure
                Console.WriteLine($"Error collecting memory metrics: {ex.Message}");
            }
        }

        private static void CollectApcUpsMetrics(StringBuilder metrics)
        {
            try
            {
                ApcUpsInfo upsInfo = ApcUpsStatusReader.GetUpsStatusAlternativeAsync().GetAwaiter().GetResult();
                UpsMetricsWriter.AppendMetrics(metrics, upsInfo);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error collecting APC UPS metrics: {ex.Message}");
            }
        }

        private static void CollectUpsdMetrics(StringBuilder metrics)
        {
            try
            {
                IReadOnlyList<ApcUpsInfo> upsInfos = UpsdStatusReader.GetUpsStatusAsync(_upsdConfiguration).GetAwaiter().GetResult();
                foreach (ApcUpsInfo upsInfo in upsInfos)
                {
                    UpsMetricsWriter.AppendMetrics(metrics, upsInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error collecting upsd UPS metrics: {ex.Message}");
            }
        }

    }
}
