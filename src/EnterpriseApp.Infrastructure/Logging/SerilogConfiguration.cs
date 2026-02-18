using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace EnterpriseApp.Infrastructure.Logging;

/// <summary>
/// Configuration helper for Serilog structured logging.
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Creates and configures a Serilog logger.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>A configured logger.</returns>
    public static ILogger CreateLogger(IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", "EnterpriseApp");

        // Console sink (always enabled)
        loggerConfig.WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");

        // File sink
        var logPath = configuration["Logging:FilePath"] ?? "logs/enterpriseapp-.log";
        loggerConfig.WriteTo.File(
            path: logPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}");

        // Seq sink (if configured)
        var seqUrl = configuration["Seq:ServerUrl"];
        if (!string.IsNullOrEmpty(seqUrl))
        {
            var seqApiKey = configuration["Seq:ApiKey"];
            loggerConfig.WriteTo.Seq(seqUrl, apiKey: seqApiKey);
        }

        return loggerConfig.CreateLogger();
    }

    /// <summary>
    /// Configures Serilog from appsettings.json.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>A configured LoggerConfiguration.</returns>
    public static LoggerConfiguration ConfigureFromSettings(IConfiguration configuration)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration);
    }
}
