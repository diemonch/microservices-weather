using System;
using System.Text.Json;
using Cloudweather.Report.Config;
using Cloudweather.Report.DataAccess;
using Cloudweather.Report.Model;
using Microsoft.Extensions.Options;

namespace Cloudweather.Report.BusinessLogic
{

	public interface IWeatherReportAggregator
	{
		public Task<WeatherReport> BuildReport(string zip, int days);
	}


    public class WeatherReportAggregator : IWeatherReportAggregator
	{
		private readonly IHttpClientFactory _http;
		private readonly ILogger<WeatherReportAggregator> _logger;
		private readonly WeatherDataConfig _weatherDataConfig;
		private readonly WeatherReportDbContext _weatherReportDbContext;

		public WeatherReportAggregator(
			IHttpClientFactory http,
            ILogger<WeatherReportAggregator> logger,
            IOptions<WeatherDataConfig> weatherDataConfig,
            WeatherReportDbContext weatherReportDbContext
            )
		{
			_http = http;
			_logger = logger;
			_weatherDataConfig = weatherDataConfig.Value;
			_weatherReportDbContext = weatherReportDbContext;
		}

        public async Task<WeatherReport> BuildReport(string zip, int days)
        {
			var httpClient = _http.CreateClient();
			
			var perceipdata = await FetchPercipdata(httpClient,zip,days);
			var totalRain = GetTotalRain(perceipdata);
			var totalSnow = GetTotalSnow(perceipdata);
			_logger.LogInformation(

				$"zip:{zip} over last {days} days: " +
				$"total snow: {totalSnow}, rain : {totalRain}"
			);
            var temperaturedata = await FetchTemperatureData(httpClient, zip, days);
			var averageHighTemp = temperaturedata.Average(t => t.TempHighF);
            var averageLowTemp = temperaturedata.Average(t => t.TempLowF);

            _logger.LogInformation(

                $"zip:{zip} over last {days} days: " +
                $"low temp: {averageLowTemp}, high temp : {averageHighTemp}"
            );

			var weatherReport = new WeatherReport
			{
				AverageHighF = Math.Round(averageHighTemp, 1),
				AverageLowF = Math.Round(averageLowTemp, 1),
				RainfallTotalInches = totalRain,
				snowfallTotalInches = totalSnow,
				ZipCode=zip,
				CreatedOn=DateTime.UtcNow

			
			};

			_weatherReportDbContext.Add(weatherReport);
			await _weatherReportDbContext.SaveChangesAsync();

			return weatherReport;
        }

		private static decimal GetTotalRain(IEnumerable<PercipitationModel> percipitations)
		{
			var totalrain = percipitations
				.Where(p => p.WeatherType == "rain")
				.Sum(p => p.AmountInches);
			return Math.Round(totalrain, 1);

		}

        private static decimal GetTotalSnow(IEnumerable<PercipitationModel> percipitations)
        {
            var totalsnow = percipitations
                .Where(p => p.WeatherType == "snow")
                .Sum(p => p.AmountInches);
            return Math.Round(totalsnow, 1);

        }

        private async Task<List<TemperatureModel>> FetchTemperatureData(HttpClient httpClient, string zip, int days)
        {
			var endpoint = BuildTemperatureServiceEndpoint(zip, days);
			var temperaturerecords = await httpClient.GetAsync(endpoint);
            var jsonSerilizerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var temperatureData = await temperaturerecords
                                .Content
								.ReadFromJsonAsync<List<TemperatureModel>>(jsonSerilizerOptions);
			return temperatureData ?? new List<TemperatureModel>();
        }

        private async Task<List<PercipitationModel>> FetchPercipdata(HttpClient httpClient, string zip, int days)
        {
			var endpoint = BuildPercipitationServiceEndpoint(zip, days);
			var perciprecords = await httpClient.GetAsync(endpoint);
			var jsonSerilizerOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
			var percipdata = await perciprecords
					.Content
					.ReadFromJsonAsync<List<PercipitationModel>>(jsonSerilizerOptions);
			return percipdata ?? new List<PercipitationModel>();
        }
        private string BuildTemperatureServiceEndpoint(string zip, int days)
        {
			var tempServiceProtocol = _weatherDataConfig.TempDataProtocol;
			var tempServiceHost = _weatherDataConfig.TempDataHost;
			var tempServicePort = _weatherDataConfig.TempDataPort;
			return $"{tempServiceProtocol}://{tempServiceHost}:{tempServicePort}/observation/{zip}?days={days}";
        }

        private string BuildPercipitationServiceEndpoint(string zip, int days)
        {
            var percipServiceProtocol = _weatherDataConfig.PercipDataProtocol;
            var percipServiceHost = _weatherDataConfig.PercipDataHost;
            var percipServicePort = _weatherDataConfig.PercipDataPort;
            return $"{percipServiceProtocol}://{percipServiceHost}:{percipServicePort}/observation/{zip}?days={days}";
        }

        
    }
}

