using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using LibreHardwareMonitor.Hardware;

namespace HardwareExporter
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineUnix(this StringBuilder sb, string value = "")
        {
            return sb.Append(value).Append('\n');
        }
    }

    class Program
    {
        private static readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
        private static HttpListener _listener;
        private static Dictionary<string, bool> _enabledCollectors = new Dictionary<string, bool>();
        private static int _port = 9182; // Default port similar to node_exporter
        private static int errorsCount = 0;
        private static Computer computer;
        private static Dictionary<string, bool> readSensors = new Dictionary<string, bool>(); // updated sensors in current read

        static void Main(string[] args)
        {

            Console.WriteLine("Hardware Metrics Exporter");
            Console.WriteLine("------------------------");

            ParseArgs(args);
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
            // Default: enable all collectors
            _enabledCollectors["hwmon"] = true;
            _enabledCollectors["netdev"] = true;
            _enabledCollectors["cpu"] = true;
            _enabledCollectors["memory"] = true;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--port" && i + 1 < args.Length)
                {
                    if (int.TryParse(args[i + 1], out int port))
                    {
                        _port = port;
                    }
                }
                else if (args[i].StartsWith("--no-"))
                {
                    string collector = args[i].Substring(5);
                    if (_enabledCollectors.ContainsKey(collector))
                    {
                        _enabledCollectors[collector] = false;
                    }
                }
                else if (args[i].StartsWith("--collector."))
                {
                    string collector = args[i].Substring(12);
                    if (_enabledCollectors.ContainsKey(collector))
                    {
                        _enabledCollectors[collector] = true;
                    }
                }
            }
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
                    metrics.AppendLineUnix($"node_hwmon_pwm{{chip=\"{sensor.Hardware.Identifier}\",hardware=\"{sensor.Hardware.Identifier}\"}} {sensor.Value.Value / 100 * 255}");
                }

                // Fan RPM metrics
                metrics.AppendLineUnix("# HELP node_hwmon_fan_rpm Hardware monitoring fan RPM");
                metrics.AppendLineUnix(
                    "# TYPE node_hwmon_fan_rpm gauge");
                foreach (var sensor in rpms)
                {
                    metrics.AppendLineUnix($"node_hwmon_fan_rpm{{chip=\"{sensor.Hardware.Identifier}\",hardware=\"{sensor.Hardware.Identifier}\"}} {sensor.Value.Value}");
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
    }
}