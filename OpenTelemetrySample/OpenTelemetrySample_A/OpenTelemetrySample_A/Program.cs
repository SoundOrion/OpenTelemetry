using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "roll-dice";

builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        .AddConsoleExporter();
});
//builder.Services.AddOpenTelemetry()
//      .ConfigureResource(resource => resource.AddService(serviceName))
//      .WithTracing(tracing => tracing
//          .AddAspNetCoreInstrumentation()
//          .AddConsoleExporter())
//      .WithMetrics(metrics => metrics
//          .AddAspNetCoreInstrumentation()
//          .AddConsoleExporter());

// AddAsp***Instrumentation()：計測したい対象

Action<ResourceBuilder> configureResource = resource => resource.AddService(
    serviceName: serviceName, //builder.Environment.ApplicationName,
    serviceVersion: "1.0.0",
    serviceInstanceId: Environment.MachineName);
builder.Services.AddOpenTelemetry()
      .ConfigureResource(configureResource)
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddHttpClientInstrumentation() //HttpClientによる外部通信処理を計測する ⇒ WebApiとかってことだろう
          .AddConsoleExporter()
          .AddOtlpExporter(configure: options => options.Endpoint = new Uri("http://jaeger:4317"))
          .AddZipkinExporter(configure: options => options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans")))
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddPrometheusExporter());

var app = builder.Build();


string HandleRollDice([FromServices] ILogger<Program> logger, string? player)
{
    var result = RollDice();

    if (string.IsNullOrEmpty(player))
    {
        logger.LogInformation("Anonymous player is rolling the dice: {result}", result);
    }
    else
    {
        logger.LogInformation("{player} is rolling the dice: {result}", player, result);
    }

    return result.ToString(CultureInfo.InvariantCulture);
}

int RollDice()
{
    return Random.Shared.Next(1, 7);
}

// enable the Prometheus endpoint
app.MapPrometheusScrapingEndpoint();
//app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/rolldice/{player?}", HandleRollDice);

app.Run();




//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;


//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddOpenTelemetry()
//  .WithTracing(b =>
//  {
//      b
//      .AddHttpClientInstrumentation()
//      .AddAspNetCoreInstrumentation();
//  });

//var app = builder.Build();

//var httpClient = new HttpClient();

//app.MapGet("/hello", async () =>
//{
//    var html = await httpClient.GetStringAsync("https://example.com/");
//    if (string.IsNullOrWhiteSpace(html))
//    {
//        return "Hello, World!";
//    }
//    else
//    {
//        return "Hello, World!";
//    }
//});

//app.Run();
