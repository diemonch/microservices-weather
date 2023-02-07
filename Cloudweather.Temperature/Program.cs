using Cloudweather.Temperature.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TemperatureDbContext>(
    opts =>
    {
        opts.EnableSensitiveDataLogging();
        opts.EnableDetailedErrors();
        opts.UseNpgsql(builder.Configuration.GetConnectionString("TemperatureDb"));

    }, ServiceLifetime.Transient

    );
var app = builder.Build();

app.Map("/observation/{zip}", async (string zip, [FromQuery] int? days, TemperatureDbContext dbContext) =>
{

    if (days == null)
    {
        return Results.BadRequest("Days cant be null");
    }

    var startdate = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = await dbContext.Temperature
                    .Where(temperature => temperature.ZipCode == zip && temperature.CreatedOn > startdate)
                    .ToListAsync();


    return Results.Ok(results);

});

//More prudent to create a Resouce model than a data model - to essentially protect the abstraction 

app.MapPost("/observation", async (Temperature temperature, TemperatureDbContext temperatureDbContext) =>
{
    temperature.CreatedOn = temperature.CreatedOn.ToUniversalTime();
    await temperatureDbContext.AddAsync(temperature);
    await temperatureDbContext.SaveChangesAsync();
});


app.Run();
