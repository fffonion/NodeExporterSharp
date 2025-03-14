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
# HELP node_hwmon_chip_names Hardware monitoring chip names
# TYPE node_hwmon_chip_names gauge
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/0",chip_name="Fan #1"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/1",chip_name="Fan #2"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/2",chip_name="Fan #3"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/3",chip_name="Fan #4"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/4",chip_name="Fan #5"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/5",chip_name="Fan #6"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/control/6",chip_name="Fan #7"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/0",chip_name="CPU Core"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/1",chip_name="Temperature #1"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/2",chip_name="Temperature #2"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/3",chip_name="Temperature #3"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/4",chip_name="Temperature #4"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/5",chip_name="Temperature #5"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/temperature/6",chip_name="Temperature #6"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/0",chip_name="Fan #1"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/1",chip_name="Fan #2"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/2",chip_name="Fan #3"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/3",chip_name="Fan #4"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/4",chip_name="Fan #5"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/5",chip_name="Fan #6"} 1
node_hwmon_chip_names{chip="/lpc/nct6798d/0/fan/6",chip_name="Fan #7"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/0",chip_name="Core Max"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/1",chip_name="Core Average"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/2",chip_name="CPU Core #1"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/3",chip_name="CPU Core #2"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/4",chip_name="CPU Core #3"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/5",chip_name="CPU Core #4"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/6",chip_name="CPU Core #5"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/7",chip_name="CPU Core #6"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/8",chip_name="CPU Core #7"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/9",chip_name="CPU Core #8"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/10",chip_name="CPU Core #9"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/11",chip_name="CPU Core #10"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/12",chip_name="CPU Core #11"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/13",chip_name="CPU Core #12"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/14",chip_name="CPU Core #13"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/15",chip_name="CPU Core #14"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/16",chip_name="CPU Package"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/17",chip_name="CPU Core #1 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/18",chip_name="CPU Core #2 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/19",chip_name="CPU Core #3 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/20",chip_name="CPU Core #4 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/21",chip_name="CPU Core #5 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/22",chip_name="CPU Core #6 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/23",chip_name="CPU Core #7 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/24",chip_name="CPU Core #8 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/25",chip_name="CPU Core #9 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/26",chip_name="CPU Core #10 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/27",chip_name="CPU Core #11 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/28",chip_name="CPU Core #12 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/29",chip_name="CPU Core #13 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/intelcpu/0/temperature/30",chip_name="CPU Core #14 Distance to TjMax"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/temperature/0",chip_name="GPU Core"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/fan/1",chip_name="GPU Fan 1"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/fan/2",chip_name="GPU Fan 2"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/control/1",chip_name="GPU Fan 1"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/control/2",chip_name="GPU Fan 2"} 1
node_hwmon_chip_names{chip="/gpu-nvidia/0/temperature/2",chip_name="GPU Hot Spot"} 1
node_hwmon_chip_names{chip="/nvme/0/temperature/0",chip_name="Temperature"} 1
node_hwmon_chip_names{chip="/nvme/0/temperature/6",chip_name="Temperature 1"} 1
node_hwmon_chip_names{chip="/nvme/0/temperature/7",chip_name="Temperature 2"} 1
node_hwmon_chip_names{chip="/nvme/1/temperature/0",chip_name="Temperature"} 1
node_hwmon_chip_names{chip="/nvme/1/temperature/6",chip_name="Temperature 1"} 1
node_hwmon_chip_names{chip="/nvme/1/temperature/7",chip_name="Temperature 2"} 1
# HELP node_hwmon_hardware_names Hardware monitoring hardware names
# TYPE node_hwmon_hardware_names gauge
node_hwmon_hardware_names{hardware="/motherboard",hardware_name="ASUS ROG STRIX B760-I GAMING WIFI"} 1
node_hwmon_hardware_names{hardware="/intelcpu/0",hardware_name="13th Gen Intel Core i5-13600KF"} 1
node_hwmon_hardware_names{hardware="/gpu-nvidia/0",hardware_name="NVIDIA GeForce RTX 4090 D"} 1
node_hwmon_hardware_names{hardware="/nvme/0",hardware_name="Samsung SSD 970 EVO Plus 1TB"} 1
node_hwmon_hardware_names{hardware="/nvme/1",hardware_name="Samsung SSD 990 PRO 2TB"} 1
# HELP node_hwmon_temp_celsius Hardware monitoring temperature
# TYPE node_hwmon_temp_celsius gauge
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/0",hardware="/lpc/nct6798d/0"} 44
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/1",hardware="/lpc/nct6798d/0"} 43
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/2",hardware="/lpc/nct6798d/0"} 39
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/3",hardware="/lpc/nct6798d/0"} 44
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/4",hardware="/lpc/nct6798d/0"} 7
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/5",hardware="/lpc/nct6798d/0"} -13
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/6",hardware="/lpc/nct6798d/0"} 25
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/0",hardware="/intelcpu/0"} 47
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/1",hardware="/intelcpu/0"} 39.5
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/2",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/3",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/4",hardware="/intelcpu/0"} 47
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/5",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/6",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/7",hardware="/intelcpu/0"} 46
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/8",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/9",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/10",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/11",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/12",hardware="/intelcpu/0"} 39
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/13",hardware="/intelcpu/0"} 39
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/14",hardware="/intelcpu/0"} 39
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/15",hardware="/intelcpu/0"} 39
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/16",hardware="/intelcpu/0"} 47
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/17",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/18",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/19",hardware="/intelcpu/0"} 53
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/20",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/21",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/22",hardware="/intelcpu/0"} 54
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/23",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/24",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/25",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/26",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/27",hardware="/intelcpu/0"} 61
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/28",hardware="/intelcpu/0"} 61
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/29",hardware="/intelcpu/0"} 61
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/30",hardware="/intelcpu/0"} 61
node_hwmon_temp_celsius{chip="/gpu-nvidia/0/temperature/0",hardware="/gpu-nvidia/0"} 47
node_hwmon_temp_celsius{chip="/gpu-nvidia/0/temperature/2",hardware="/gpu-nvidia/0"} 56.3125
node_hwmon_temp_celsius{chip="/nvme/0/temperature/0",hardware="/nvme/0"} 52
node_hwmon_temp_celsius{chip="/nvme/0/temperature/6",hardware="/nvme/0"} 52
node_hwmon_temp_celsius{chip="/nvme/0/temperature/7",hardware="/nvme/0"} 60
node_hwmon_temp_celsius{chip="/nvme/1/temperature/0",hardware="/nvme/1"} 54
node_hwmon_temp_celsius{chip="/nvme/1/temperature/6",hardware="/nvme/1"} 54
node_hwmon_temp_celsius{chip="/nvme/1/temperature/7",hardware="/nvme/1"} 59
# HELP node_hwmon_pwm Hardware monitoring fan PWM
# TYPE node_hwmon_pwm gauge
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 98.99999
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 98.99999
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 255
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 255
node_hwmon_pwm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
node_hwmon_pwm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
# HELP node_hwmon_fan_rpm Hardware monitoring fan RPM
# TYPE node_hwmon_fan_rpm gauge
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 1191.527
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 1008.9686
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
node_hwmon_fan_rpm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
# HELP node_network_receive_bytes_total Network device statistic receive_bytes
# TYPE node_network_receive_bytes_total counter
node_network_receive_bytes_total{device="Intel[R] Ethernet Controller I226-V"} 0
node_network_receive_bytes_total{device="Intel[R] Wi-Fi 6E AX211 160MHz"} 1.1350202E+09
# HELP node_network_transmit_bytes_total Network device statistic transmit_bytes
# TYPE node_network_transmit_bytes_total counter
node_network_transmit_bytes_total{device="Intel[R] Ethernet Controller I226-V"} 0
node_network_transmit_bytes_total{device="Intel[R] Wi-Fi 6E AX211 160MHz"} 6.325242E+09
# HELP node_cpu_seconds_total Time that processor spent in different modes (dpc, idle, interrupt, privileged, user)
# TYPE node_cpu_seconds_total counter
node_cpu_seconds_total{cpu="0,9",mode="dpc"} 5.09375
node_cpu_seconds_total{cpu="0,9",mode="idle"} 19704.86
node_cpu_seconds_total{cpu="0,9",mode="interrupt"} 20.109375
node_cpu_seconds_total{cpu="0,9",mode="privileged"} 226.625
node_cpu_seconds_total{cpu="0,9",mode="user"} 705.6406
node_cpu_seconds_total{cpu="0,6",mode="dpc"} 64.96875
node_cpu_seconds_total{cpu="0,6",mode="idle"} 15645.656
node_cpu_seconds_total{cpu="0,6",mode="interrupt"} 88.34375
node_cpu_seconds_total{cpu="0,6",mode="privileged"} 1363.1875
node_cpu_seconds_total{cpu="0,6",mode="user"} 3628.3125
node_cpu_seconds_total{cpu="0,8",mode="dpc"} 63.5
node_cpu_seconds_total{cpu="0,8",mode="idle"} 15754.267
node_cpu_seconds_total{cpu="0,8",mode="interrupt"} 116.03124
node_cpu_seconds_total{cpu="0,8",mode="privileged"} 1386.0781
node_cpu_seconds_total{cpu="0,8",mode="user"} 3496.8125
node_cpu_seconds_total{cpu="0,7",mode="dpc"} 7.1562495
node_cpu_seconds_total{cpu="0,7",mode="idle"} 19578.36
node_cpu_seconds_total{cpu="0,7",mode="interrupt"} 22.28125
node_cpu_seconds_total{cpu="0,7",mode="privileged"} 245.90625
node_cpu_seconds_total{cpu="0,7",mode="user"} 812.8594
node_cpu_seconds_total{cpu="0,17",mode="dpc"} 5.203125
node_cpu_seconds_total{cpu="0,17",mode="idle"} 19770.875
node_cpu_seconds_total{cpu="0,17",mode="interrupt"} 32.875
node_cpu_seconds_total{cpu="0,17",mode="privileged"} 267.9375
node_cpu_seconds_total{cpu="0,17",mode="user"} 598.3125
node_cpu_seconds_total{cpu="0,4",mode="dpc"} 161.09375
node_cpu_seconds_total{cpu="0,4",mode="idle"} 15541.469
node_cpu_seconds_total{cpu="0,4",mode="interrupt"} 210.76562
node_cpu_seconds_total{cpu="0,4",mode="privileged"} 1534.0156
node_cpu_seconds_total{cpu="0,4",mode="user"} 3561.6719
node_cpu_seconds_total{cpu="0,16",mode="dpc"} 7.578125
node_cpu_seconds_total{cpu="0,16",mode="idle"} 19528.64
node_cpu_seconds_total{cpu="0,16",mode="interrupt"} 42.5
node_cpu_seconds_total{cpu="0,16",mode="privileged"} 358.15625
node_cpu_seconds_total{cpu="0,16",mode="user"} 750.3594
node_cpu_seconds_total{cpu="0,10",mode="dpc"} 38.4375
node_cpu_seconds_total{cpu="0,10",mode="idle"} 15497.141
node_cpu_seconds_total{cpu="0,10",mode="interrupt"} 84.046875
node_cpu_seconds_total{cpu="0,10",mode="privileged"} 1393.3438
node_cpu_seconds_total{cpu="0,10",mode="user"} 3746.6719
node_cpu_seconds_total{cpu="0,18",mode="dpc"} 8.140625
node_cpu_seconds_total{cpu="0,18",mode="idle"} 19821.406
node_cpu_seconds_total{cpu="0,18",mode="interrupt"} 30.1875
node_cpu_seconds_total{cpu="0,18",mode="privileged"} 245.78125
node_cpu_seconds_total{cpu="0,18",mode="user"} 569.9375
node_cpu_seconds_total{cpu="0,1",mode="dpc"} 26.0625
node_cpu_seconds_total{cpu="0,1",mode="idle"} 19727.031
node_cpu_seconds_total{cpu="0,1",mode="interrupt"} 24.390625
node_cpu_seconds_total{cpu="0,1",mode="privileged"} 268
node_cpu_seconds_total{cpu="0,1",mode="user"} 642.09375
node_cpu_seconds_total{cpu="0,3",mode="dpc"} 59.890625
node_cpu_seconds_total{cpu="0,3",mode="idle"} 19525.266
node_cpu_seconds_total{cpu="0,3",mode="interrupt"} 68.125
node_cpu_seconds_total{cpu="0,3",mode="privileged"} 347.375
node_cpu_seconds_total{cpu="0,3",mode="user"} 764.5156
node_cpu_seconds_total{cpu="0,0",mode="dpc"} 614.4375
node_cpu_seconds_total{cpu="0,0",mode="idle"} 15555.845
node_cpu_seconds_total{cpu="0,0",mode="interrupt"} 589.375
node_cpu_seconds_total{cpu="0,0",mode="privileged"} 2210.1094
node_cpu_seconds_total{cpu="0,0",mode="user"} 2871.3438
node_cpu_seconds_total{cpu="0,5",mode="dpc"} 22.921875
node_cpu_seconds_total{cpu="0,5",mode="idle"} 19338.61
node_cpu_seconds_total{cpu="0,5",mode="interrupt"} 22.375
node_cpu_seconds_total{cpu="0,5",mode="privileged"} 294.54688
node_cpu_seconds_total{cpu="0,5",mode="user"} 1003.9687
node_cpu_seconds_total{cpu="0,11",mode="dpc"} 5.34375
node_cpu_seconds_total{cpu="0,11",mode="idle"} 19487.078
node_cpu_seconds_total{cpu="0,11",mode="interrupt"} 18.546875
node_cpu_seconds_total{cpu="0,11",mode="privileged"} 238.89062
node_cpu_seconds_total{cpu="0,11",mode="user"} 911.15625
node_cpu_seconds_total{cpu="0,15",mode="dpc"} 5.484375
node_cpu_seconds_total{cpu="0,15",mode="idle"} 19850.11
node_cpu_seconds_total{cpu="0,15",mode="interrupt"} 31.109377
node_cpu_seconds_total{cpu="0,15",mode="privileged"} 241.60938
node_cpu_seconds_total{cpu="0,15",mode="user"} 545.40625
node_cpu_seconds_total{cpu="0,13",mode="dpc"} 5.875
node_cpu_seconds_total{cpu="0,13",mode="idle"} 19840.469
node_cpu_seconds_total{cpu="0,13",mode="interrupt"} 33.265625
node_cpu_seconds_total{cpu="0,13",mode="privileged"} 252.51564
node_cpu_seconds_total{cpu="0,13",mode="user"} 544.15625
node_cpu_seconds_total{cpu="0,14",mode="dpc"} 6.46875
node_cpu_seconds_total{cpu="0,14",mode="idle"} 19742.64
node_cpu_seconds_total{cpu="0,14",mode="interrupt"} 33.90625
node_cpu_seconds_total{cpu="0,14",mode="privileged"} 262.5
node_cpu_seconds_total{cpu="0,14",mode="user"} 632
node_cpu_seconds_total{cpu="0,12",mode="dpc"} 12.8125
node_cpu_seconds_total{cpu="0,12",mode="idle"} 19558.531
node_cpu_seconds_total{cpu="0,12",mode="interrupt"} 43.96875
node_cpu_seconds_total{cpu="0,12",mode="privileged"} 340.57812
node_cpu_seconds_total{cpu="0,12",mode="user"} 738.0469
node_cpu_seconds_total{cpu="0,19",mode="dpc"} 14.921875
node_cpu_seconds_total{cpu="0,19",mode="idle"} 16165.86
node_cpu_seconds_total{cpu="0,19",mode="interrupt"} 39.140625
node_cpu_seconds_total{cpu="0,19",mode="privileged"} 3906.8123
node_cpu_seconds_total{cpu="0,19",mode="user"} 564.4844
node_cpu_seconds_total{cpu="0,2",mode="dpc"} 151.29688
node_cpu_seconds_total{cpu="0,2",mode="idle"} 14439.798
node_cpu_seconds_total{cpu="0,2",mode="interrupt"} 101.84375
node_cpu_seconds_total{cpu="0,2",mode="privileged"} 1684.8594
node_cpu_seconds_total{cpu="0,2",mode="user"} 4512.5
# HELP node_cpu_power_watts CPU power consumption in watts
# TYPE node_cpu_power_watts gauge
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="package"} 47.45076
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cores"} 36.366405
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cpu_memory"} 0
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cpu_platform"} 0
# HELP node_memory_MemTotal_bytes Memory information field MemTotal_bytes
# TYPE node_memory_MemTotal_bytes gauge
node_memory_MemTotal_bytes 68498427904
# HELP node_memory_MemAvailable_bytes Memory information field MemAvailable_bytes
# TYPE node_memory_MemAvailable_bytes gauge
node_memory_MemAvailable_bytes 47100149760
# HELP node_memory_VirtualMemoryTotal_bytes Memory information field VirtualMemoryTotal_bytes
# TYPE node_memory_VirtualMemoryTotal_bytes gauge
node_memory_VirtualMemoryTotal_bytes 72793391104
# HELP node_memory_VirtualMemoryAvailable_bytes Memory information field VirtualMemoryAvailable_bytes
# TYPE node_memory_VirtualMemoryAvailable_bytes gauge
node_memory_VirtualMemoryAvailable_bytes 41939873792
# HELP node_exporter_build_info Build information
# TYPE node_exporter_build_info gauge
node_exporter_build_info{version="1.2.0.0"} 1
# HELP node_exporter_errors_total Errors total
# TYPE node_exporter_errors_total counter
node_exporter_errors_total 0

```

## Contributing

If you encounter any issues or have suggestions for improvements, please feel free to open an issue or submit a pull request on the [GitHub repository](https://github.com/your-username/node-exporter-sharp).