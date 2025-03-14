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
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/0",hardware="/lpc/nct6798d/0"} 45
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/1",hardware="/lpc/nct6798d/0"} 42.5
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/2",hardware="/lpc/nct6798d/0"} 39
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/3",hardware="/lpc/nct6798d/0"} 43.5
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/4",hardware="/lpc/nct6798d/0"} 7
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/5",hardware="/lpc/nct6798d/0"} -13
node_hwmon_temp_celsius{chip="/lpc/nct6798d/0/temperature/6",hardware="/lpc/nct6798d/0"} 25
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/0",hardware="/intelcpu/0"} 43
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/1",hardware="/intelcpu/0"} 39.92857
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/2",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/3",hardware="/intelcpu/0"} 42
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/4",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/5",hardware="/intelcpu/0"} 42
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/6",hardware="/intelcpu/0"} 43
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/7",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/8",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/9",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/10",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/11",hardware="/intelcpu/0"} 38
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/12",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/13",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/14",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/15",hardware="/intelcpu/0"} 40
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/16",hardware="/intelcpu/0"} 41
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/17",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/18",hardware="/intelcpu/0"} 58
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/19",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/20",hardware="/intelcpu/0"} 58
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/21",hardware="/intelcpu/0"} 57
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/22",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/23",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/24",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/25",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/26",hardware="/intelcpu/0"} 62
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/27",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/28",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/29",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/intelcpu/0/temperature/30",hardware="/intelcpu/0"} 60
node_hwmon_temp_celsius{chip="/gpu-nvidia/0/temperature/0",hardware="/gpu-nvidia/0"} 46
node_hwmon_temp_celsius{chip="/gpu-nvidia/0/temperature/2",hardware="/gpu-nvidia/0"} 55.75
node_hwmon_temp_celsius{chip="/nvme/0/temperature/0",hardware="/nvme/0"} 52
node_hwmon_temp_celsius{chip="/nvme/0/temperature/6",hardware="/nvme/0"} 52
node_hwmon_temp_celsius{chip="/nvme/0/temperature/7",hardware="/nvme/0"} 60
node_hwmon_temp_celsius{chip="/nvme/1/temperature/0",hardware="/nvme/1"} 54
node_hwmon_temp_celsius{chip="/nvme/1/temperature/6",hardware="/nvme/1"} 54
node_hwmon_temp_celsius{chip="/nvme/1/temperature/7",hardware="/nvme/1"} 59
# HELP node_hwmon_pwm Hardware monitoring fan PWM
# TYPE node_hwmon_pwm gauge
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 95
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 95
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 153
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 255
node_hwmon_pwm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 255
node_hwmon_pwm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
node_hwmon_pwm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
# HELP node_hwmon_fan_rpm Hardware monitoring fan RPM
# TYPE node_hwmon_fan_rpm gauge
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 1154.8331
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 965.6652
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/lpc/nct6798d/0",hardware="/lpc/nct6798d/0"} 0
node_hwmon_fan_rpm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
node_hwmon_fan_rpm{chip="/gpu-nvidia/0",hardware="/gpu-nvidia/0"} 0
# HELP node_network_receive_bytes_total Network device statistic receive_bytes
# TYPE node_network_receive_bytes_total counter
node_network_receive_bytes_total{device="Intel[R] Wi-Fi 6E AX211 160MHz"} 4.5345777E+09
node_network_receive_bytes_total{device="Intel[R] Ethernet Controller I226-V"} 0
# HELP node_network_transmit_bytes_total Network device statistic transmit_bytes
# TYPE node_network_transmit_bytes_total counter
node_network_transmit_bytes_total{device="Intel[R] Wi-Fi 6E AX211 160MHz"} 872547000
node_network_transmit_bytes_total{device="Intel[R] Ethernet Controller I226-V"} 0
# HELP node_cpu_seconds_total Time that processor spent in different modes (dpc, idle, interrupt, privileged, user)
# TYPE node_cpu_seconds_total counter
node_cpu_seconds_total{cpu="0,10",mode="dpc"} 277187500
node_cpu_seconds_total{cpu="0,10",mode="idle"} 112604218750
node_cpu_seconds_total{cpu="0,10",mode="interrupt"} 636718750
node_cpu_seconds_total{cpu="0,10",mode="privileged"} 10445156250
node_cpu_seconds_total{cpu="0,10",mode="user"} 28782031250
node_cpu_seconds_total{cpu="0,16",mode="dpc"} 62187500
node_cpu_seconds_total{cpu="0,16",mode="idle"} 142234062500
node_cpu_seconds_total{cpu="0,16",mode="interrupt"} 361718750
node_cpu_seconds_total{cpu="0,16",mode="privileged"} 2998437500
node_cpu_seconds_total{cpu="0,16",mode="user"} 6598906250
node_cpu_seconds_total{cpu="0,12",mode="dpc"} 112968750
node_cpu_seconds_total{cpu="0,12",mode="idle"} 142328593750
node_cpu_seconds_total{cpu="0,12",mode="interrupt"} 385937500
node_cpu_seconds_total{cpu="0,12",mode="privileged"} 2917656250
node_cpu_seconds_total{cpu="0,12",mode="user"} 6585000000
node_cpu_seconds_total{cpu="0,3",mode="dpc"} 444218750
node_cpu_seconds_total{cpu="0,3",mode="idle"} 142649218750
node_cpu_seconds_total{cpu="0,3",mode="interrupt"} 505468750
node_cpu_seconds_total{cpu="0,3",mode="privileged"} 2730156250
node_cpu_seconds_total{cpu="0,3",mode="user"} 6451875000
node_cpu_seconds_total{cpu="0,19",mode="dpc"} 115781250
node_cpu_seconds_total{cpu="0,19",mode="idle"} 117387187500
node_cpu_seconds_total{cpu="0,19",mode="interrupt"} 324843750
node_cpu_seconds_total{cpu="0,19",mode="privileged"} 29527656250
node_cpu_seconds_total{cpu="0,19",mode="user"} 4916562500
node_cpu_seconds_total{cpu="0,11",mode="dpc"} 42656250
node_cpu_seconds_total{cpu="0,11",mode="idle"} 141864062500
node_cpu_seconds_total{cpu="0,11",mode="interrupt"} 157968750
node_cpu_seconds_total{cpu="0,11",mode="privileged"} 1969843750
node_cpu_seconds_total{cpu="0,11",mode="user"} 7997343750
node_cpu_seconds_total{cpu="0,7",mode="dpc"} 55312500
node_cpu_seconds_total{cpu="0,7",mode="idle"} 142707187500
node_cpu_seconds_total{cpu="0,7",mode="interrupt"} 189062500
node_cpu_seconds_total{cpu="0,7",mode="privileged"} 2030781250
node_cpu_seconds_total{cpu="0,7",mode="user"} 7093281250
node_cpu_seconds_total{cpu="0,0",mode="dpc"} 4733906250
node_cpu_seconds_total{cpu="0,0",mode="idle"} 112513281250
node_cpu_seconds_total{cpu="0,0",mode="interrupt"} 4477500000
node_cpu_seconds_total{cpu="0,0",mode="privileged"} 16848906250
node_cpu_seconds_total{cpu="0,0",mode="user"} 22470781250
node_cpu_seconds_total{cpu="0,14",mode="dpc"} 55156250
node_cpu_seconds_total{cpu="0,14",mode="idle"} 143838906250
node_cpu_seconds_total{cpu="0,14",mode="interrupt"} 298281250
node_cpu_seconds_total{cpu="0,14",mode="privileged"} 2248906250
node_cpu_seconds_total{cpu="0,14",mode="user"} 5743750000
node_cpu_seconds_total{cpu="0,5",mode="dpc"} 171875000
node_cpu_seconds_total{cpu="0,5",mode="idle"} 140360781250
node_cpu_seconds_total{cpu="0,5",mode="interrupt"} 191250000
node_cpu_seconds_total{cpu="0,5",mode="privileged"} 2450156250
node_cpu_seconds_total{cpu="0,5",mode="user"} 9020468750
node_cpu_seconds_total{cpu="0,15",mode="dpc"} 46406250
node_cpu_seconds_total{cpu="0,15",mode="idle"} 144872187500
node_cpu_seconds_total{cpu="0,15",mode="interrupt"} 272343750
node_cpu_seconds_total{cpu="0,15",mode="privileged"} 2089531250
node_cpu_seconds_total{cpu="0,15",mode="user"} 4869843750
node_cpu_seconds_total{cpu="0,2",mode="dpc"} 1138125000
node_cpu_seconds_total{cpu="0,2",mode="idle"} 103644062500
node_cpu_seconds_total{cpu="0,2",mode="interrupt"} 774218750
node_cpu_seconds_total{cpu="0,2",mode="privileged"} 12774375000
node_cpu_seconds_total{cpu="0,2",mode="user"} 35413125000
node_cpu_seconds_total{cpu="0,9",mode="dpc"} 38750000
node_cpu_seconds_total{cpu="0,9",mode="idle"} 143952968750
node_cpu_seconds_total{cpu="0,9",mode="interrupt"} 170937500
node_cpu_seconds_total{cpu="0,9",mode="privileged"} 1857968750
node_cpu_seconds_total{cpu="0,9",mode="user"} 6020312500
node_cpu_seconds_total{cpu="0,8",mode="dpc"} 437812500
node_cpu_seconds_total{cpu="0,8",mode="idle"} 114633437500
node_cpu_seconds_total{cpu="0,8",mode="interrupt"} 845312500
node_cpu_seconds_total{cpu="0,8",mode="privileged"} 10436718750
node_cpu_seconds_total{cpu="0,8",mode="user"} 26761406250
node_cpu_seconds_total{cpu="0,4",mode="dpc"} 1124843750
node_cpu_seconds_total{cpu="0,4",mode="idle"} 112737187500
node_cpu_seconds_total{cpu="0,4",mode="interrupt"} 1531562500
node_cpu_seconds_total{cpu="0,4",mode="privileged"} 11430625000
node_cpu_seconds_total{cpu="0,4",mode="user"} 27663750000
node_cpu_seconds_total{cpu="0,18",mode="dpc"} 71562500
node_cpu_seconds_total{cpu="0,18",mode="idle"} 144635312500
node_cpu_seconds_total{cpu="0,18",mode="interrupt"} 269375000
node_cpu_seconds_total{cpu="0,18",mode="privileged"} 2130312500
node_cpu_seconds_total{cpu="0,18",mode="user"} 5065781250
node_cpu_seconds_total{cpu="0,6",mode="dpc"} 463281250
node_cpu_seconds_total{cpu="0,6",mode="idle"} 113552500000
node_cpu_seconds_total{cpu="0,6",mode="interrupt"} 672031250
node_cpu_seconds_total{cpu="0,6",mode="privileged"} 10339218750
node_cpu_seconds_total{cpu="0,6",mode="user"} 27939843750
node_cpu_seconds_total{cpu="0,13",mode="dpc"} 50781250
node_cpu_seconds_total{cpu="0,13",mode="idle"} 144806093750
node_cpu_seconds_total{cpu="0,13",mode="interrupt"} 288281250
node_cpu_seconds_total{cpu="0,13",mode="privileged"} 2159531250
node_cpu_seconds_total{cpu="0,13",mode="user"} 4865937500
node_cpu_seconds_total{cpu="0,1",mode="dpc"} 209218750
node_cpu_seconds_total{cpu="0,1",mode="idle"} 144080937500
node_cpu_seconds_total{cpu="0,1",mode="interrupt"} 210937500
node_cpu_seconds_total{cpu="0,1",mode="privileged"} 2197812500
node_cpu_seconds_total{cpu="0,1",mode="user"} 5552656250
node_cpu_seconds_total{cpu="0,17",mode="dpc"} 44531250
node_cpu_seconds_total{cpu="0,17",mode="idle"} 144232343750
node_cpu_seconds_total{cpu="0,17",mode="interrupt"} 287031250
node_cpu_seconds_total{cpu="0,17",mode="privileged"} 2298125000
node_cpu_seconds_total{cpu="0,17",mode="user"} 5301093750
# HELP node_cpu_power_watts CPU power consumption in watts
# TYPE node_cpu_power_watts gauge
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="package"} 45.94235
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cores"} 35.34013
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cpu_memory"} 0
node_cpu_power_watts{cpu="13th Gen Intel Core i5-13600KF", type="cpu_platform"} 0
# HELP node_memory_MemTotal_bytes Memory information field MemTotal_bytes
# TYPE node_memory_MemTotal_bytes gauge
node_memory_MemTotal_bytes 66892996
# HELP node_memory_MemAvailable_bytes Memory information field MemAvailable_bytes
# TYPE node_memory_MemAvailable_bytes gauge
node_memory_MemAvailable_bytes 45503268
# HELP node_memory_VirtualMemoryTotal_bytes Memory information field VirtualMemoryTotal_bytes
# TYPE node_memory_VirtualMemoryTotal_bytes gauge
node_memory_VirtualMemoryTotal_bytes 71087296
# HELP node_memory_VirtualMemoryAvailable_bytes Memory information field VirtualMemoryAvailable_bytes
# TYPE node_memory_VirtualMemoryAvailable_bytes gauge
node_memory_VirtualMemoryAvailable_bytes 40134200
# HELP node_exporter_build_info Build information
# TYPE node_exporter_build_info gauge
node_exporter_build_info{version="1.0.0.0"} 1
# HELP node_exporter_errors_total Errors total
# TYPE node_exporter_errors_total counter
node_exporter_errors_total 0

```

## Contributing

If you encounter any issues or have suggestions for improvements, please feel free to open an issue or submit a pull request on the [GitHub repository](https://github.com/your-username/node-exporter-sharp).