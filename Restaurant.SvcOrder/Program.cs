using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Events;
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0005
// ReSharper disable once RedundantUsingDirective
using Serilog.Formatting.Compact;
#pragma warning restore IDE0005
#pragma warning restore IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0005
// ReSharper disable once RedundantUsingDirective
using Serilog.Sinks.SystemConsole.Themes;
#pragma warning restore IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning restore IDE0005
#pragma warning restore IDE0079 // Remove unnecessary suppression

namespace Restaurant.SvcOrder;

public class Program
{
    private static void ConfigureDependencyInjection(IServiceCollection services, ConfigurationManager configurationManager)
    {
        ConfigureDatabaseConnection();
        ConfigureMetrics();
        ConfigureMetricsEndpoint();
        ConfigureUseCases();

        void ConfigureUseCases()
        {
            services.AddTransient<Repositories.Orders.OrderSourceEventMapping>();
            services.AddTransient<Domain.Orders.IOrderRepository, Repositories.Orders.OrderRepository>();
            services.AddTransient<Domain.Orders.Order.PersistenceContext>();
        }

        void ConfigureDatabaseConnection()
        {
            services.Configure<Repositories.DatabaseConnectionConfiguration>(configurationManager.GetSection(nameof(Repositories.DatabaseConnectionConfiguration)));
            services.AddSingleton<Repositories.DatabaseConnectionProvider>(); // singleton because the the connection string is only created once
            services.AddHostedService<Repositories.DatabaseConfigurationCheck>();
        }

        void ConfigureMetrics()
        {
            services.AddSingleton<Operations.Metrics.Metric>();
        }

        void ConfigureMetricsEndpoint()
        {
            // TODO Configuration must be renamed based on if pull or push is used
            //var metricSection = configurationManager.GetSection(nameof(Operations.Metrics.MetricConfiguration));
            // var metricConfiguration = metricSection.Get<Operations.Metrics.MetricConfiguration>() ?? throw new InvalidOperationException($"{nameof(Operations.Metrics.MetricConfiguration)} can not be null");
            // services.Configure<Operations.Metrics.MetricConfiguration>(metricSection);

            services.AddOpenTelemetry()
                .WithMetrics(metricBuilder => metricBuilder
                    .AddMeter(Operations.Metrics.Metric.ApplicationName)
                // TODO RuntimeInstrumentation works but spams the console. reactivate after every thing is done
                // .AddRuntimeInstrumentation()
                // TODO find out which package should be used for AspNetCoreInstrumentation
                // .AddAspNetCoreInstrumentation()
#if DEBUG
                    .AddConsoleExporter(builder => builder.Targets = ConsoleExporterOutputTargets.Console)
#endif
                /* Exporter endpoint where you find the open telemetry collector (push) or where to expose (pull)
                .AddOtlpExporter(
                    otlpConfig =>
                    {
                        otlpConfig.Endpoint = new Uri(metricConfiguration.BuildUri());
                        otlpConfig.Protocol = OtlpExportProtocol.Grpc;
                    })
                */
                )
                .StartWithHost();
        }
    }

    private static void ConfigureServices(IServiceCollection services, ConfigurationManager configurationManager)
    {
        ConfigureDependencyInjection(services, configurationManager);

        services.AddHealthChecks()
            .AddCheck<Operations.HealthChecks.Diagnostics.HttpHealthCheck>(
                "http_health_check",
                HealthStatus.Unhealthy,
                new[] { "ready" });

        services.AddControllers().AddControllersAsServices();

        services.AddGrpc();
    }

    public static async Task<int> Main(string[] _)
    {
        try
        {
            SetSerilogDefaultLogger();
            Log.ForContext<Program>().Information("Starting application.");

            var builder = CreateWebApplication();
            var app = builder.Build();

            ConfigureApp(app);

            await app.RunAsync();

            return 0;
        }
        catch (OperationCanceledException)
        {
            return 0;
        }
        catch (Exception ex)
        {
            SetSerilogDefaultLogger();
            Log.ForContext<Program>().Fatal(ex, "Application terminated unexpectedly.");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
            SetSerilogDefaultLogger();
            Log.ForContext<Program>().Information("Application stopped.");
            Log.CloseAndFlush();
        }
    }

    public static WebApplicationBuilder CreateWebApplication()
    {
        var builder = WebApplication.CreateBuilder();
        ConfigureConfiguration(builder.Configuration);
        ConfigureHost(builder.Host);
        ConfigureWebHost(builder.WebHost);
        ConfigureServices(builder.Services, builder.Configuration);
        return builder;
    }

    private static void ConfigureConfiguration(IConfigurationBuilder configuration)
    {
        configuration.AddEnvironmentVariables();
        LogConfiguration();

        void LogConfiguration()
        {
            foreach (IConfigurationSource source in configuration.Sources)
            {
                if (source is JsonConfigurationSource jsonConfigurationSource)
                {
                    Log.ForContext<Program>().Debug("Configuration loaded from json file: {jsonConfigurationSource.Path}",
                        jsonConfigurationSource.Path);
                }
                else
                {
                    Log.ForContext<Program>().Debug("Configuration loaded from source type: {source}", source);
                }
            }
        }
    }

    private static void ConfigureApp(WebApplication app)
    {
        app.UseRouting();

        app.MapHealthChecks("/health/live", new HealthCheckOptions() { Predicate = _ => false }); // runs no checks, just to test if application is live
        app.MapHealthChecks("/health/ready", new HealthCheckOptions()); // run all health checks
        app.MapGrpcService<Operations.HealthChecks.Grpc.GrpcHealthCheck>();
        app.MapControllers();
        /* Use for open telemetry manual testing
        app.MapGet("/hello", () =>
        {
            using var activity = Activity.Current;
            activity?.SetTag("foo", 1);
            activity?.SetTag("bar", "Hello, World!");
            activity?.SetTag("baz", new int[] { 1, 2, 3 });

            // Up a counter for each request
            app.Services.GetRequiredService<Operations.Metrics.Metric>().TestRequestStarted();

            return "Hello, World!";
        });
        */
    }

    private static void ConfigureHost(ConfigureHostBuilder host)
    {
        host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            string[] excludedRequestPaths =
            {
                "/health",
            };

            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.WithProperty("AssemblyVersion", typeof(Program).Assembly.GetName().Version)
                .Filter.ByExcluding( // removes logs from requests to excluded services
                    logEvent => FilterByRequestPath(logEvent,
                        excludedRequestPaths,
                        LogEventLevel.Warning
                    ));
        });
    }

    private static bool FilterByRequestPath(LogEvent logEvent, string[] requestPathStartsWith, LogEventLevel minimumLevel)
    {
        if (logEvent.Exception == null && logEvent.Level < minimumLevel)
        {
            if (logEvent.Properties.TryGetValue("RequestPath", out LogEventPropertyValue? requestPathValue))
            {
                string? requestPath = (requestPathValue as ScalarValue)?.Value?.ToString();
                if (requestPath != null)
                {
                    foreach (string filter in requestPathStartsWith)
                    {
                        if (requestPath.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private static void ConfigureWebHost(ConfigureWebHostBuilder webHost)
    {
        webHost.ConfigureKestrel((context, serverOptions) =>
        {
            serverOptions.ListenAnyIP(8080, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
            serverOptions.ListenAnyIP(3118, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
            // serverOptions.ListenAnyIP(7106, listenOptions => { listenOptions.Protocols = HttpProtocols.Http3; });
        });
    }

    /// <summary>
    ///     Creates a default logger that is only used until the application configuration was loaded.
    /// </summary>
    private static void SetSerilogDefaultLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("AssemblyVersion", typeof(Program).Assembly.GetName().Version)
#if DEBUG
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: "[{Timestamp:o}] [{Level:u3}] [{Application}] [{Message}] [{Exception}] [{Properties:j}] {NewLine}"
            )
#else
            .WriteTo.Console(new RenderedCompactJsonFormatter())
#endif
            .CreateLogger();
    }
}