using System;
using Microsoft.EntityFrameworkCore;

namespace Cloudweather.Report.DataAccess
{
	public class WeatherReportDbContext:DbContext
	{
		public WeatherReportDbContext()
		{
		}
        public WeatherReportDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

		public DbSet<WeatherReport> WeatherReport { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTablenames(modelBuilder);
        }

        private static void SnakeCaseIdentityTablenames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherReport>(b => { b.ToTable("WeatherReport"); });
        }
    }
}

