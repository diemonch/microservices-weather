using System;
namespace Cloudweather.Percipitation.DataAccess
{
	public class Percipitation
	{
		public Guid Id { get; set; }
		public DateTime CreatedOn { get; set; }
		public decimal AmountInches { get; set; }
		public string WeatherType { get; set; }
        public string Zip { get; set; }



    }
}

