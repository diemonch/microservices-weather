//Build temperature observations

using System.Net.Http.Json;
using Cloudweather.Dataloader.Model;
using Microsoft.Extensions.Configuration;

IConfiguration configuraton = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var serviceConfig = configuraton.GetSection("Services");

var tempServiceConfig = serviceConfig.GetSection("Temperature");
var tempServiceHost = tempServiceConfig["Host"];
var tempServicePort = tempServiceConfig["Port"];

var percipServiceConfig = serviceConfig.GetSection("Percipitation");
var percipServiceHost = percipServiceConfig["Host"];
var percipServicePort = percipServiceConfig["Port"];

var zipCodes = new List<string>
{
    "560035",
    "641006",
    "560108",
    "73026",
    "19717"

};

Console.WriteLine("Starting data load");

var temperatureHttpclient = new HttpClient();
temperatureHttpclient.BaseAddress = new Uri($"http://{tempServiceHost}:{tempServicePort}");

var percipHttpclient = new HttpClient();
percipHttpclient.BaseAddress = new Uri($"http://{percipServiceHost}:{percipServicePort}");


foreach (var zip in zipCodes)
{

    Console.WriteLine($"processing zip code {zip}");
    var from = DateTime.Now.AddYears(-2);
    var thrugh = DateTime.Now;

    for(var day=from.Date;day<thrugh.Date;day=day.AddDays(1))
    {
        var temps = PostTemp(zip, day, temperatureHttpclient);
        PostPercip(temps[0], zip, day, percipHttpclient);
    }
}



void PostPercip(int lowTemp, string zip, DateTime day, HttpClient percipHttpclient)
{
    var rand = new Random();
    var isPercip = rand.Next(2) < 1;

    PercipitationModel percipitaion;

    if (isPercip)
    {
        var percipInches = rand.Next(1, 16);

        if(lowTemp < 32)
        {
            percipitaion = new PercipitationModel
            {
                AmountInches = percipInches,
                WeatherType = "Snow",
                ZipCode = zip,
                CreatedOn = day
            };
        } else
        {
            percipitaion = percipitaion = new PercipitationModel
            {
                AmountInches = percipInches,
                WeatherType = "Rain",
                ZipCode = zip,
                CreatedOn = day
            };

        }
    } else
    {
        percipitaion = new PercipitationModel
        {
            AmountInches = 0,
            WeatherType = "None",
            ZipCode = zip,
            CreatedOn = day
        };
    }


    var percipResponse = percipHttpclient
        .PostAsJsonAsync("observatiion", percipitaion)
        .Result;

    if(percipResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted Percipitation: Date {day:d}"+
            $"Zip {zip}" +
            $"Type {percipitaion.WeatherType}"+
            $"Amoint in {percipitaion.AmountInches}"
            );

    }
}

List<int> PostTemp(string zip, DateTime day, HttpClient temperatureHttpclient)
{
    var rand = new Random();
    var t1 = rand.Next(0, 100);
    var t2 = rand.Next(0, 100);
    var hiLoTemp = new List<int> { t1, t2 };
    hiLoTemp.Sort();

    var temperatureObservation = new TemperatureModel
    {
        TempLowF = hiLoTemp[0],
        TempHighF = hiLoTemp[1],
        ZipCode = zip,
        CreatedOn = day

    };

    var tempResponse = temperatureHttpclient
        .PostAsJsonAsync("observation", temperatureObservation)
        .Result;

    if (tempResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted Temperature: Date {day:d}" +
            $"Zip {zip}" +
            $"Lo {hiLoTemp[0]}" +
            $"Hi {hiLoTemp[1]}"
            );
    } else
    {
        Console.WriteLine(tempResponse.ToString());
    }

        return hiLoTemp;

}