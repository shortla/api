﻿using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shortha.Infrastructre.DI;

internal static class OpenTelemetry
{

    public static IServiceCollection AddTracing(this IServiceCollection services)
    {
        var otel = services.AddOpenTelemetry();

// Configure OpenTelemetry Resources with the application name
        otel.ConfigureResource(resource =>
        {
            resource.AddService(serviceName: $"{Config.appName}");
            var globalOpenTelemetryAttributes = new List<KeyValuePair<string, object>>();
            globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("env", Config.env));
            globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("appId", Config.appId));
            globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("appName", Config.appName));
            resource.AddAttributes(globalOpenTelemetryAttributes);
        });

// Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        otel.WithMetrics(metrics => metrics
                                    .AddOtlpExporter(otlpOptions =>
                                    {
                                        otlpOptions.Endpoint = new Uri("https://otl.gitnasr.com:18889");
                                    })
                                    // Metrics provider from OpenTelemetry
                                    .AddAspNetCoreInstrumentation()
                                    .AddMeter(Config.appName)
                                    // Metrics provides by ASP.NET Core in .NET 8
                                    .AddMeter("Microsoft.AspNetCore.Hosting")
                                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                                    .AddPrometheusExporter());

// Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource(Config.appName);
            tracing.AddOtlpExporter(otlpOptions => { otlpOptions.Endpoint = new Uri("https://otl.gitnasr.com:18889"); });
       //     tracing.AddConsoleExporter();
        });

        return services;
    }

}