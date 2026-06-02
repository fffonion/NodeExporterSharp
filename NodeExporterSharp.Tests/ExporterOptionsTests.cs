using HardwareExporter;

namespace NodeExporterSharp.Tests;

public class ExporterOptionsTests
{
    [Fact]
    public void ParseDefaultsUpsdEndpointFromEnvironment()
    {
        Dictionary<string, string?> environment = new()
        {
            ["NODE_EXPORTER_UPSD_HOST"] = "192.0.2.10",
            ["NODE_EXPORTER_UPSD_PORT"] = "1234"
        };

        ExporterOptions options = ExporterOptions.Parse(["--collector.upsd"], environment.GetValueOrDefault);

        Assert.True(options.EnabledCollectors["upsd"]);
        Assert.Equal("192.0.2.10", options.Upsd.Host);
        Assert.Equal(1234, options.Upsd.Port);
    }

    [Fact]
    public void ParseUsesDefaultUpsdEndpointWhenEnvironmentIsUnset()
    {
        ExporterOptions options = ExporterOptions.Parse(["--collector.upsd"], _ => null);

        Assert.Equal("127.0.0.1", options.Upsd.Host);
        Assert.Equal(3493, options.Upsd.Port);
    }

    [Fact]
    public void ParseRejectsApcupsAndUpsdWhenBothCollectorsAreEnabled()
    {
        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => ExporterOptions.Parse(["--collector.apcups", "--collector.upsd"], _ => null));

        Assert.Contains("apcups", exception.Message);
        Assert.Contains("upsd", exception.Message);
    }

    [Fact]
    public void ParseDisablesUpsCollectorsByDefault()
    {
        ExporterOptions options = ExporterOptions.Parse([], _ => null);

        Assert.False(options.EnabledCollectors["apcups"]);
        Assert.False(options.EnabledCollectors["upsd"]);
    }
}
