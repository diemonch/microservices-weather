using System;
namespace Cloudweather.Dataloader.Model
{
	internal class PercipitationModel
	{
       
        public DateTime CreatedOn { get; set; }
        public decimal AmountInches { get; set; }
        public string WeatherType { get; set; }
        public string ZipCode { get; set; }

        //public PercipitationModel(decimal AmountInches, string WeatherType, string ZipCode, DateTime CreatedOn)
        //{
        //    this.CreatedOn = CreatedOn;
        //    this.AmountInches = AmountInches;
        //    this.WeatherType = WeatherType;
        //    this.ZipCode = ZipCode;
        //}
    }
}

