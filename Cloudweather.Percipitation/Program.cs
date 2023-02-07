using Cloudweather.Percipitation.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PercipDbContext>(
    opts =>
    {
        opts.EnableSensitiveDataLogging();
        opts.EnableDetailedErrors();
        opts.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
      
    }, ServiceLifetime.Transient

    );

var app = builder.Build();

app.Map("/observation/{zip}", async (string zip, [FromQuery] int ? days, PercipDbContext dbContext) =>
{

    if(days == null)
    {
        return Results.BadRequest("Days cant be null");
    }

    var startdate = DateTime.UtcNow - TimeSpan.FromDays(days.Value);
    var results = await dbContext.Percipitation
                    .Where(percip => percip.Zip == zip && percip.CreatedOn>startdate)
                    .ToListAsync();


    return Results.Ok(results);

});

app.MapPost("/observation", async (Percipitation percip, PercipDbContext percipdbContext) =>
{
    percip.CreatedOn = percip.CreatedOn.ToUniversalTime();
    await percipdbContext.AddAsync(percip);
    await percipdbContext.SaveChangesAsync();
});

app.Run();
