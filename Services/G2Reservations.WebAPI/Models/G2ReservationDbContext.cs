using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace G2Reservations.WebAPI.Models
{
	public partial class G2ReservationDbContext : DbContext
	{
		public G2ReservationDbContext()
		{
		}

		public G2ReservationDbContext(DbContextOptions<G2ReservationDbContext> options)
			: base(options)
		{
		}

		public virtual DbSet<G2Reservation> Reservations { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=G2ReservationsDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<G2Reservation>(entity =>
			{
				entity.ToTable("Reservations");
				entity.HasKey(e => e.Id);
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}