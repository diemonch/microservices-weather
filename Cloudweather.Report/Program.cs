using Cloudweather.Report.BusinessLogic;
using Cloudweather.Report.Config;
using Cloudweather.Report.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IWeatherReportAggregator, WeatherReportAggregator>();
builder.Services.AddOptions();
builder.Services.Configure<WeatherDataConfig>(builder.Configuration.GetSection("WeatherDataConfig"));


builder.Services.AddDbContext<WeatherReportDbContext>(
    opts =>
    {
        opts.EnableSensitiveDataLogging();
        opts.EnableDetailedErrors();
        opts.UseNpgsql(builder.Configuration.GetConnectionString("WeatherReportDb"));

    }, ServiceLifetime.Transient

    );

var app = builder.Build();

app.MapGet(
    "/weather-report/{zip}",
    async (string zip, [FromQuery] int? days, IWeatherReportAggregator weatheragg) =>
{

    if (days == null || days > 30 && days <1 )
    {
        return Results.BadRequest("Days must be between 1 and 30");
    }

    var startdate = DateTime.UtcNow - TimeSpan.FromDays(days.Value);

    var report = await weatheragg.BuildReport(zip, days.Value);

    return Results.Ok(report);

   

});


app.Run();
