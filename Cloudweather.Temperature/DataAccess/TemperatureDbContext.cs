using System;
using Microsoft.EntityFrameworkCore;
namespace Cloudweather.Temperature.DataAccess
{
	public class TemperatureDbContext:DbContext
	{
		public TemperatureDbContext()
		{
		}
        public TemperatureDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Temperature> Temperature { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTablenames(modelBuilder);
        }

        private static void SnakeCaseIdentityTablenames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>(b => { b.ToTable("Temperature"); });
        }
    }
}

