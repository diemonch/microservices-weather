using System;
namespace Cloudweather.Report.Config
{
	public class WeatherDataConfig
	{
		public string PercipDataProtocol { get; set; } = string.Empty;
        public string PercipDataHost { get; set; } = string.Empty;
        public string PercipDataPort { get; set; } = string.Empty;
        public string TempDataProtocol { get; set; } = string.Empty;
        public string TempDataHost { get; set; } = string.Empty;
        public string TempDataPort { get; set; } = string.Empty;
    }
}

