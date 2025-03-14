# Node Exporter Sharp

<img src="icon.svg" width="128" />

Node Exporter Sharp is a comprehensive replacement for the popular `node_exporter` and `windows_exporter` tools used with Prometheus monitoring system. It provides a wide range of system metrics for Windows-based machines, including hardware monitoring, network, CPU, and memory statistics.

## Features

- **Comprehensive Metrics**: Node Exporter Sharp collects a wide range of system metrics, including:
  - Hardware monitoring (using LibreHardwareMonitor)
  - Network interface statistics
  - CPU utilization
  - Memory usage

- **GPU and SSD Sensors**: In addition to the standard metrics, Node Exporter Sharp also provides access to GPU and SSD sensors, which are not available in the original `node_exporter` and `windows_exporter` tools.

- **Flexible Configuration**: The tool accepts various command-line arguments to enable or disable specific collectors, as well as to set the listening port.

## Command-Line Arguments

Node Exporter Sharp supports the following command-line arguments:

- `--port <port>`: Specifies the port on which the server will listen (default is 9100).
- `--no-<collector>`: Disables the specified collector. Available collectors are: `hwmon`, `netdev`, `cpu`, `memory`.
- `--collector.<collector>`: Enables the specified collector. Available collectors are: `hwmon`, `netdev`, `cpu`, `memory`.

By default, all collectors are enabled.

## Highlights

- **LibreHardwareMonitor Integration**: Node Exporter Sharp utilizes the LibreHardwareMonitor library to access a wide range of hardware sensors, including those that are not accessible through the standard `node_exporter` and `windows_exporter` tools.
- **GPU and SSD Sensors**: In addition to the standard system metrics, Node Exporter Sharp provides access to GPU and SSD sensors, allowing for more comprehensive monitoring of your hardware.

## Installation and Usage

1. Download the latest release of Node Exporter Sharp from the GitHub repository.
2. Run the executable with the desired command-line arguments.
3. Configure your Prometheus server to scrape metrics from the Node Exporter Sharp instance.

## Sample Output

```
 HELP node_hwmon_chip_names Hardware monitoring chip names
# TYPE node_hwmon_chip_names gauge
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/0",name="Fan #1"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/1",name="Fan #2"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/2",name="Fan #3"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/3",name="Fan #4"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/4",name="Fan #5"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/5",name="Fan #6"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/6",name="Fan #7"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/0",name="CPU Core"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/1",name="Temperature #1"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/2",name="Temperature #2"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/3",name="Temperature #3"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/4",name="Temperature #4"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/5",name="Temperature #5"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/6",name="Temperature #6"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/0",name="Fan #1"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/1",name="Fan #2"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/2",name="Fan #3"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/3",name="Fan #4"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/4",name="Fan #5"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/5",name="Fan #6"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/6",name="Fan #7"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/0",name="Core Max"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/1",name="Core Average"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/2",name="CPU Core #1"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/3",name="CPU Core #2"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/4",name="CPU Core #3"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/5",name="CPU Core #4"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/6",name="CPU Core #5"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/7",name="CPU Core #6"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/8",name="CPU Core #7"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/9",name="CPU Core #8"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/10",name="CPU Core #9"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/11",name="CPU Core #10"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/12",name="CPU Core #11"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/13",name="CPU Core #12"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/14",name="CPU Core #13"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/15",name="CPU Core #14"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/16",name="CPU Package"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/17",name="CPU Core #1 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/18",name="CPU Core #2 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/19",name="CPU Core #3 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/20",name="CPU Core #4 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/21",name="CPU Core #5 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/22",name="CPU Core #6 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/23",name="CPU Core #7 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/24",name="CPU Core #8 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/25",name="CPU Core #9 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/26",name="CPU Core #10 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/27",name="CPU Core #11 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/28",name="CPU Core #12 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/29",name="CPU Core #13 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/30",name="CPU Core #14 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/temperature/0",name="GPU Core"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/fan/1",name="GPU Fan 1"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/fan/2",name="GPU Fan 2"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/control/1",name="GPU Fan 1"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/control/2",name="GPU Fan 2"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/temperature/2",name="GPU Hot Spot"} 1
node_hwmon_chip_names{chip="/nvme/0/temperature/0",name="Temperature"} 1
node_hwmon_chip_names{chip="/nvme/0/temperature/6",name="Temperature 1"} 1
node_hwmon_chip_names{chip="/nvme/0/temperature/7",name="Temperature 2"} 1
node_hwmon_chip_names{chip="/nvme/1/temperature/0",name="Temperature"} 1
node_hwmon_chip_names{chip="/nvme/1/temperature/6",name="Temperature 1"} 1
node_hwmon_chip_names{chip="/nvme/1/temperature/7",name="Temperature 2"} 1
# HELP node_hwmon_hardware_names Hardware monitoring hardware names
# TYPE node_hwmon_hardware_names gauge
node_hwmon_hardware_names{hardware="/motherboard",name="ASUS ROG STRIX B760-I GAMING WIFI"} 1
node_hwmon_hardware_names{hardware="/intelcpu/0",name="13th Gen Intel Core i5-13600KF"} 1
node_hwmon_hardware_names{hardware="/gpu-nvidia/0",name="NVIDIA GeForce RTX 4090 D"} 1
node_hwmon_hardware_names{hardware="/nvme/0",name="Samsung SSD 970 EVO Plus 1TB"} 1
node_hwmon_hardware_names{hardware="/nvme/1",name="Samsung SSD 990 PRO 2TB"} 1
# HELP node_hwmon_temp_celsius Hardware monitoring temperature
# TYPE node_hwmon_temp_celsius gauge
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/0",hardware="/lpc/nct6798d/0"} 44
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/1",hardware="/lpc/nct6798d/0"} 42.5
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/2",hardware="/lpc/nct6798d/0"} 39
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/3",hardware="/lpc/nct6798d/0"} 43.5
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/4",hardware="/lpc/nct6798d/0"} 7
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/5",hardware="/lpc/nct6798d/0"} -13
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/6",hardware="/lpc/nct6798d/0"} 25
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/0",hardware="/intelcpu/0"} 45
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/1",hardware="/intelcpu/0"} 40.285713
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/2",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/3",hardware="/intelcpu/0"} 41
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/4",hardware="/intelcpu/0"} 44
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/5",hardware="/intelcpu/0"} 42
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/6",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/7",hardware="/intelcpu/0"} 45
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/8",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/9",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/10",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/11",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/12",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/13",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/14",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/15",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/16",hardware="/intelcpu/0"} 44
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/17",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/18",hardware="/intelcpu/0"} 59
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/19",hardware="/intelcpu/0"} 56
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/20",hardware="/intelcpu/0"} 58
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/21",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/22",hardware="/intelcpu/0"} 55
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/23",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/24",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/25",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/26",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/27",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/28",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/29",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/30",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/gpu-nvidia/0/temperature/0",hardware="/gpu-nvidia/0"} 46
node_hwmon_temp_celsius{chip="/gpu-nvidia/0/temperature/2",hardware="/gpu-nvidia/0"} 56
node_hwmon_temp_celsius{chip="/nvme/0/temperature/0",hardware="/nvme/0"} 52
node_hwmon_temp_celsius{chip="/nvme/0/temperature/6",hardware="/nvme/0"} 52
node_hwmon_temp_celsius{chip="/nvme/0/temperature/7",hardware="/nvme/0"} 60
node_hwmon_temp_celsius{chip="/nvme/1/temperature/0",hardware="/nvme/1"} 54
node_hwmon_temp_celsius{chip="/nvme/1/temperature/6",hardware="/nvme/1"} 54
node_hwmon_temp_celsius{chip="/nvme/1/temperature/7",hardware="/nvme/1"} 59
# HELP node_hwmon_pwm Hardware monitoring fan PWM
# TYPE node_hwmon_pwm gauge
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 102
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 102
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 255
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 255
node_hwmon_pwm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
node_hwmon_pwm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
# HELP node_hwmon_fan_rpm Hardware monitoring fan RPM
# TYPE node_hwmon_fan_rpm gauge
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 1124.0632
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 961.53845
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
node_hwmon_fan_rpm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
# HELP node_network_receive_bytes_total Network device statistic receive_bytes
# TYPE node_network_receive_bytes_total counter
node_network_receive_bytes_total{device="本地连接* 1"} 0
node_network_receive_bytes_total{device="本地连接* 2"} 0
node_network_receive_bytes_total{device="蓝牙网络连接"} 0
node_network_receive_bytes_total{device="以太网"} 0
node_network_receive_bytes_total{device="WLAN"} 0
# HELP node_network_transmit_bytes_total Network device statistic transmit_bytes
# TYPE node_network_transmit_bytes_total counter
node_network_transmit_bytes_total{device="本地连接* 1"} 0
node_network_transmit_bytes_total{device="本地连接* 2"} 0
node_network_transmit_bytes_total{device="蓝牙网络连接"} 0
node_network_transmit_bytes_total{device="以太网"} 0
node_network_transmit_bytes_total{device="WLAN"} 0
# HELP node_cpu_usage_percentage CPU usage percentage per core and total
# TYPE node_cpu_usage_percentage gauge
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="total"} 6.967431
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #1 Thread #1"} 27.872414
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #1 Thread #2"} 0
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #2 Thread #1"} 35.532093
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #2 Thread #2"} 0
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #3 Thread #1"} 30.006403
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #3 Thread #2"} 0
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #4 Thread #1"} 31.174082
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #4 Thread #2"} 0
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #5 Thread #1"} 14.635295
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #5 Thread #2"} 0.59766173
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #6 Thread #1"} 28.752644
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #6 Thread #2"} 0.74141026
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #7"} 2.2586584
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #8"} 0.94723105
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #9"} 0.8518398
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #10"} 0
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #11"} 0.6394565
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #12"} 0
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #13"} 3.5041273
node_cpu_usage_percentage{cpu="13th Gen Intel Core i5-13600KF", core="CPU Core #14"} 1.3972819
# HELP node_cpu_power_watts CPU power consumption in watts
# TYPE node_cpu_power_watts gauge
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="package"} 37.530994
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cores"} 27.333067
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cpu_memory"} 0
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cpu_platform"} 0
# HELP node_memory_MemTotal_bytes Memory information field MemTotal_bytes
# TYPE node_memory_MemTotal_bytes gauge
node_memory_MemTotal_bytes 66892996
# HELP node_memory_MemAvailable_bytes Memory information field MemAvailable_bytes
# TYPE node_memory_MemAvailable_bytes gauge
node_memory_MemAvailable_bytes 45272328
# HELP node_memory_VirtualMemoryTotal_bytes Memory information field VirtualMemoryTotal_bytes
# TYPE node_memory_VirtualMemoryTotal_bytes gauge
node_memory_VirtualMemoryTotal_bytes 71087296
# HELP node_memory_VirtualMemoryAvailable_bytes Memory information field VirtualMemoryAvailable_bytes
# TYPE node_memory_VirtualMemoryAvailable_bytes gauge
node_memory_VirtualMemoryAvailable_bytes 40306968
# HELP node_exporter_build_info Build information
# TYPE node_exporter_build_info gauge
node_exporter_build_info{version="1.0.0.0"} 1
# HELP node_exporter_errors_total Errors total
# TYPE node_exporter_errors_total counter
node_exporter_errors_total 0

```

## Contributing

If you encounter any issues or have suggestions for improvements, please feel free to open an issue or submit a pull request on the [GitHub repository](https://github.com/your-username/node-exporter-sharp).