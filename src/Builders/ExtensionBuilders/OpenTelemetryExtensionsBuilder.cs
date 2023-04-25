namespace Kuzaine.Builders.ExtensionBuilders;

using Domain;
using Helpers;
using Services;

public class OpenTelemetryExtensionsBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public OpenTelemetryExtensionsBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateOTelServiceExtension(string srcDirectory, string projectBaseName, DbProvider dbProvider, int otelAgentPort)
    {
        var classPath = ClassPathHelper.WebApiServiceExtensionsClassPath(srcDirectory, $"OpenTelemetryServiceExtension.cs", projectBaseName);
        var fileText = GetOtelText(classPath.ClassNamespace, dbProvider, otelAgentPort, srcDirectory, projectBaseName);
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetOtelText(string classNamespace, DbProvider dbProvider, int otelAgentPort, string srcDirectory, string projectBaseName)
    {
        var envServiceClassPath = ClassPathHelper.WebApiServicesClassPath(srcDirectory, "", projectBaseName);
        return @$"namespace {classNamespace};

using {envServiceClassPath.ClassNamespace};
using Configurations;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class OpenTelemetryServiceExtension
{{
    public static void OpenTelemetryRegistration(this WebApplicationBuilder builder, IConfiguration configuration, string serviceName)
    {{
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName)
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector();
        
        builder.Logging.AddOpenTelemetry(o =>
        {{
            // TODO: Setup an exporter here
            o.SetResourceBuilder(resourceBuilder);
        }});
        
        builder.Services.AddOpenTelemetryMetrics(metrics =>
        {{
            metrics.SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEventCountersInstrumentation(c =>
                {{
                    // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                    c.AddEventSources(
                        ""Microsoft.AspNetCore.Hosting"",
                        ""Microsoft-AspNetCore-Server-Kestrel"",
                        ""System.Net.Http"",
                        ""System.Net.Sockets"",
                        ""System.Net.NameResolution"",
                        ""System.Net.Security"");
                }});
        }});

        builder.Services.AddOpenTelemetryTracing(builder =>
        {{
            builder.SetResourceBuilder(resourceBuilder)
                .AddSource(""MassTransit"")
                .AddSource(""{dbProvider.OTelSource()}"")
                // The following subscribes to activities from Activity Source
                // named ""MyCompany.MyProduct.MyLibrary"" only.
                // .AddSource(""MyCompany.MyProduct.MyLibrary"")
                .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddJaegerExporter(o =>
                {{
                    o.AgentHost = configuration.GetJaegerHostValue();
                    o.AgentPort = {otelAgentPort};
                    o.MaxPayloadSizeInBytes = 4096;
                    o.ExportProcessorType = ExportProcessorType.Batch;
                    o.BatchExportProcessorOptions = new BatchExportProcessorOptions<System.Diagnostics.Activity>
                    {{
                        MaxQueueSize = 2048,
                        ScheduledDelayMilliseconds = 5000,
                        ExporterTimeoutMilliseconds = 30000,
                        MaxExportBatchSize = 512,
                    }};
                }});
        }});
    }}
}}";
    }
}