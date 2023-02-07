using System;
using Microsoft.EntityFrameworkCore;

namespace Cloudweather.Percipitation.DataAccess
{
	//EF is an abstraction on database
	//DbContext is representing database
	//Creating DB set for Percipitation

	public class PercipDbContext : DbContext
	{
		public PercipDbContext()
		{
		}

        public PercipDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

		public DbSet<Percipitation> Percipitation { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			SnakeCaseIdentityTablenames(modelBuilder);
		}

        private static void SnakeCaseIdentityTablenames(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<Percipitation>(b => { b.ToTable("Percipitation"); });
        }
    }
}

