using System.Text;
using HardwareExporter;

namespace NodeExporterSharp.Tests;

public class UpsdStatusReaderTests
{
    [Fact]
    public void ParseUpsListDiscoversAllUpsNames()
    {
        string response = """
            BEGIN LIST UPS
            UPS ups "Back-UPS BK650M2_CH"
            UPS office "Office UPS"
            END LIST UPS
            """;

        string[] upsNames = UpsdStatusReader.ParseUpsNames(response).ToArray();

        Assert.Equal(["ups", "office"], upsNames);
    }

    [Fact]
    public void ConvertVariablesToUpsInfoMapsNutFieldsToApcMetricsShape()
    {
        string response = """
            BEGIN LIST VAR ups
            VAR ups battery.charge "11"
            VAR ups battery.runtime "120"
            VAR ups ups.status "OL DISCHRG"
            VAR ups ups.load "18"
            VAR ups input.voltage "224.0"
            VAR ups battery.voltage "13.6"
            VAR ups ups.model "Back-UPS BK650M2_CH"
            END LIST VAR ups
            """;

        Dictionary<string, string> variables = UpsdStatusReader.ParseVariables("ups", response);
        ApcUpsInfo upsInfo = UpsdStatusReader.ToUpsInfo("ups", variables);

        Assert.Equal("Back-UPS BK650M2_CH", upsInfo.Name);
        Assert.True(upsInfo.DeviceOnline);
        Assert.Equal(18, upsInfo.UpsLoad);
        Assert.Equal(2, upsInfo.RuntimeMinutesRemaining);
        Assert.Equal(224.0, upsInfo.InputVoltage);
        Assert.Equal(11, upsInfo.BatteryCharge);
        Assert.Equal(13.6, upsInfo.BatteryVoltage);
    }

    [Fact]
    public void ConvertVariablesToUpsInfoFallsBackToDiscoveredUpsName()
    {
        Dictionary<string, string> variables = new()
        {
            ["ups.status"] = "OB DISCHRG"
        };

        ApcUpsInfo upsInfo = UpsdStatusReader.ToUpsInfo("ups", variables);

        Assert.Equal("ups", upsInfo.Name);
        Assert.False(upsInfo.DeviceOnline);
    }

    [Fact]
    public void AppendUpsMetricsWritesSameMetricNamesAsApcUpsCollector()
    {
        StringBuilder metrics = new();
        ApcUpsInfo upsInfo = new()
        {
            Name = "Back-UPS BK650M2_CH",
            DeviceOnline = true,
            UpsLoad = 18,
            RuntimeMinutesRemaining = 2,
            InputVoltage = 224.0,
            BatteryCharge = 11,
            BatteryVoltage = 13.6
        };

        UpsMetricsWriter.AppendMetrics(metrics, upsInfo);

        string output = metrics.ToString();
        Assert.Contains("node_ups_device_online{name=\"Back-UPS BK650M2_CH\"} 1", output);
        Assert.Contains("node_ups_load_percent{name=\"Back-UPS BK650M2_CH\"} 18", output);
        Assert.Contains("node_ups_runtime_minutes_remaining{name=\"Back-UPS BK650M2_CH\"} 2", output);
        Assert.Contains("node_ups_input_voltage{name=\"Back-UPS BK650M2_CH\"} 224", output);
        Assert.Contains("node_ups_battery_charge{name=\"Back-UPS BK650M2_CH\"} 11", output);
        Assert.Contains("node_ups_battery_voltage{name=\"Back-UPS BK650M2_CH\"} 13.6", output);
    }
}
