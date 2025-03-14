using System.Net;
using System.Reflection;
using System.Text;
using LibreHardwareMonitor.Hardware;

namespace HardwareExporter
{
    class Program
    {
        private static readonly ManualResetEvent _shutdown = new ManualResetEvent(false);
        private static HttpListener _listener;
        private static Dictionary<string, bool> _enabledCollectors = new Dictionary<string, bool>();
        private static int _port = 9182; // Default port similar to node_exporter
        private static int errorsCount = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Hardware Metrics Exporter");
            Console.WriteLine("------------------------");

            ParseArgs(args);
            PrintEnabledCollectors();

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
                    StringBuilder metrics = new StringBuilder();

                    var computer = new Computer
                    {
                        IsCpuEnabled = true,
                        IsGpuEnabled = true,
                        IsMemoryEnabled = true,
                        IsMotherboardEnabled = true,
                        //IsControllerEnabled = true,
                        IsNetworkEnabled = true,
                        IsStorageEnabled = true
                    };
                    try {
                        // Initialize LibreHardwareMonitor
                        computer.Open();

                        // Collect enabled metrics
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
                    finally
                    {
                        // Always close the computer connection
                        if (computer != null) computer.Close();
                    }

                    // Add build info
                    metrics.AppendLine("# HELP node_exporter_build_info Build information");
                    metrics.AppendLine("# TYPE node_exporter_build_info gauge");
                    metrics.AppendLine($"node_exporter_build_info{{version=\"{Assembly.GetExecutingAssembly().GetName().Version}\"}} 1");

                    // Add Errors total
                    metrics.AppendLine("# HELP node_exporter_errors_total Errors total");
                    metrics.AppendLine("# TYPE node_exporter_errors_total counter");
                    metrics.AppendLine($"node_exporter_errors_total {errorsCount}");

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
                    hardware.Update();

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
                        subhardware.Update();

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
                metrics.AppendLine("# HELP node_hwmon_chip_names Hardware monitoring chip names");
                metrics.AppendLine("# TYPE node_hwmon_chip_names gauge");
                // All chip names mapping
                foreach (var chipName in chipNames)
                {
                    metrics.AppendLine($"node_hwmon_chip_names{{chip=\"{chipName.Key}\",name=\"{chipName.Value}\"}} 1");
                }

                // Hardware names
                metrics.AppendLine("# HELP node_hwmon_hardware_names Hardware monitoring hardware names");
                metrics.AppendLine("# TYPE node_hwmon_hardware_names gauge");
                // All hardware names mapping
                foreach (var hardwareName in hardwareNames)
                {
                    metrics.AppendLine($"node_hwmon_hardware_names{{hardware=\"{hardwareName.Key}\",name=\"{hardwareName.Value}\"}} 1");
                }

                // Temperature metrics
                metrics.AppendLine("# HELP node_hwmon_temp_celsius Hardware monitoring temperature");
                metrics.AppendLine("# TYPE node_hwmon_temp_celsius gauge");
                foreach (var sensor in temps)
                {
                    metrics.AppendLine($"node_hwmon_temp_celsius{{chip=\"{sensor.Identifier}\",hardware=\"{sensor.Hardware.Identifier}\"}} {sensor.Value.Value}");
                }

                // PWM metrics
                metrics.AppendLine("# HELP node_hwmon_pwm Hardware monitoring fan PWM");
                metrics.AppendLine(
                    "# TYPE node_hwmon_pwm gauge");
                foreach (var sensor in pwms)
                {
                    metrics.AppendLine($"node_hwmon_pwm{{chip=\"{sensor.Hardware.Identifier}\",hardware=\"{sensor.Hardware.Identifier}\"}} {sensor.Value.Value / 100 * 255}");
                }

                // Fan RPM metrics
                metrics.AppendLine("# HELP node_hwmon_fan_rpm Hardware monitoring fan RPM");
                metrics.AppendLine(
                    "# TYPE node_hwmon_fan_rpm gauge");
                foreach (var sensor in rpms)
                {
                    metrics.AppendLine($"node_hwmon_fan_rpm{{chip=\"{sensor.Hardware.Identifier}\",hardware=\"{sensor.Hardware.Identifier}\"}} {sensor.Value.Value}");
                }
            }
            finally
            {
            }
        }

        private static void CollectNetworkMetrics(Computer computer, StringBuilder metrics)
        {
            // First metric - Receive bytes
            metrics.AppendLine("# HELP node_network_receive_bytes_total Network device statistic receive_bytes");
            metrics.AppendLine("# TYPE node_network_receive_bytes_total counter");

            // Ensure network hardware is enabled for monitoring
            computer.IsNetworkEnabled = true;

            // Update hardware information
            computer.Hardware.ToList().ForEach(hardware => hardware.Update());

            // Output all receive metrics first
            foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.Network))
            {
                hardware.Update();
                string interfaceName = hardware.Name.Replace("\"", "\\\"");

                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Throughput &&
                        (sensor.Name.Contains("Received") || sensor.Name.Contains("Download")))
                    {
                        metrics.AppendLine($"node_network_receive_bytes_total{{device=\"{interfaceName}\"}} {sensor.Value * 1024}");
                    }
                }
            }

            // Second metric - Transmit bytes
            metrics.AppendLine("# HELP node_network_transmit_bytes_total Network device statistic transmit_bytes");
            metrics.AppendLine("# TYPE node_network_transmit_bytes_total counter");

            // Output all transmit metrics
            foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.Network))
            {
                string interfaceName = hardware.Name.Replace("\"", "\\\"");

                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Throughput &&
                        (sensor.Name.Contains("Sent") || sensor.Name.Contains("Upload")))
                    {
                        metrics.AppendLine($"node_network_transmit_bytes_total{{device=\"{interfaceName}\"}} {sensor.Value * 1024}");
                    }
                }
            }
        }

        private static void CollectCpuMetrics(Computer computer, StringBuilder metrics)
        {
            // First metric - CPU Usage Percentage
            metrics.AppendLine("# HELP node_cpu_usage_percentage CPU usage percentage per core and total");
            metrics.AppendLine("# TYPE node_cpu_usage_percentage gauge");

            // Ensure CPU hardware is enabled for monitoring
            computer.IsCpuEnabled = true;

            // Update hardware information
            computer.Hardware.ToList().ForEach(hardware => hardware.Update());

            // Output all CPU usage metrics
            foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.Cpu))
            {
                hardware.Update();
                string cpuName = hardware.Name.Replace("\"", "\\\"");

                // Track if we found the total CPU usage
                bool foundTotalUsage = false;

                // First look for total CPU usage
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Load &&
                        sensor.Name.Contains("Total"))
                    {
                        metrics.AppendLine($"node_cpu_usage_percentage{{cpu=\"{cpuName}\", core=\"total\"}} {sensor.Value}");
                        foundTotalUsage = true;
                    }
                }

                // Then output per-core usage
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Load &&
                        sensor.Name.Contains("Core #"))
                    {
                        // Extract core number from name (typically in format "Core #1" or similar)
                        string coreName = sensor.Name;
                        metrics.AppendLine($"node_cpu_usage_percentage{{cpu=\"{cpuName}\", core=\"{coreName}\"}} {sensor.Value}");
                    }
                }

                // If we didn't find a total, calculate average from cores as fallback
                if (!foundTotalUsage)
                {

                    var coreLoads = hardware.Sensors
                        .Where(s => s.SensorType == LibreHardwareMonitor.Hardware.SensorType.Load &&
                               s.Name.Contains("Core #"))
                        .Select(s => s.Value);

                    if (coreLoads.Any())
                    {
                        double avgLoad = coreLoads.Average() ?? 0.0;
                        metrics.AppendLine($"node_cpu_usage_percentage{{cpu=\"{cpuName}\", core=\"total\"}} {avgLoad}");
                    }
                }
            }

            // Second metric - CPU Power Consumption
            metrics.AppendLine("# HELP node_cpu_power_watts CPU power consumption in watts");
            metrics.AppendLine("# TYPE node_cpu_power_watts gauge");

            // Output all CPU power metrics
            foreach (var hardware in computer.Hardware.Where(h => h.HardwareType == LibreHardwareMonitor.Hardware.HardwareType.Cpu))
            {
                string cpuName = hardware.Name.Replace("\"", "\\\"");

                // Package/Total power
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == LibreHardwareMonitor.Hardware.SensorType.Power)
                    {
                        string powerType = sensor.Name.Contains("Package") ? "package" :
                                          sensor.Name.Contains("Core") ? "cores" :
                                          sensor.Name.ToLower().Replace(" ", "_");

                        metrics.AppendLine($"node_cpu_power_watts{{cpu=\"{cpuName}\", type=\"{powerType}\"}} {sensor.Value}");
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
                        hardware.Update();

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
                const long bytesInMB = 1024 * 1024;

                // Output Total Memory metric with help and type
                metrics.AppendLine("# HELP node_memory_MemTotal_bytes Memory information field MemTotal_bytes");
                metrics.AppendLine("# TYPE node_memory_MemTotal_bytes gauge");
                if (availableMemory.HasValue && usedMemory.HasValue)
                {
                    metrics.AppendLine($"node_memory_MemTotal_bytes {(long)((availableMemory.Value + usedMemory.Value) * bytesInMB)}");
                }
                else
                {
                    metrics.AppendLine("node_memory_MemTotal_bytes 0");
                }

                // Output Available Memory metric with help and type
                metrics.AppendLine("# HELP node_memory_MemAvailable_bytes Memory information field MemAvailable_bytes");
                metrics.AppendLine("# TYPE node_memory_MemAvailable_bytes gauge");
                if (availableMemory.HasValue)
                {
                    metrics.AppendLine($"node_memory_MemAvailable_bytes {(long)(availableMemory.Value * bytesInMB)}");
                }
                else
                {
                    metrics.AppendLine("node_memory_MemAvailable_bytes 0");
                }

                // Output Virtual Memory Total metric with help and type
                metrics.AppendLine("# HELP node_memory_VirtualMemoryTotal_bytes Memory information field VirtualMemoryTotal_bytes");
                metrics.AppendLine("# TYPE node_memory_VirtualMemoryTotal_bytes gauge");
                if (availableVirtualMemory.HasValue && usedVirtualMemory.HasValue)
                {
                    metrics.AppendLine($"node_memory_VirtualMemoryTotal_bytes {(long)((availableVirtualMemory.Value + usedVirtualMemory.Value) * bytesInMB)}");
                }
                else
                {
                    metrics.AppendLine("node_memory_VirtualMemoryTotal_bytes 0");
                }

                // Output Virtual Memory Available metric with help and type
                metrics.AppendLine(
                    "# HELP node_memory_VirtualMemoryAvailable_bytes Memory information field VirtualMemoryAvailable_bytes");
                metrics.AppendLine("# TYPE node_memory_VirtualMemoryAvailable_bytes gauge");
                if (availableVirtualMemory.HasValue)
                {
                    metrics.AppendLine($"node_memory_VirtualMemoryAvailable_bytes {(long)(availableVirtualMemory.Value * bytesInMB)}");
                }
                else
                {
                    metrics.AppendLine("node_memory_VirtualMemoryAvailable_bytes 0");
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