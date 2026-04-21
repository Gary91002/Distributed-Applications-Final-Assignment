using System;
using System.Collections.Generic;
using G2CarRentalCustomers.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalPlatform.Models;

public partial class G2CustomerProfileContext : DbContext
{
    public G2CustomerProfileContext()
    {
    }

    public G2CustomerProfileContext(DbContextOptions<G2CustomerProfileContext> options)
        : base(options)
    {
    }

    public virtual DbSet<G2Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=/app/data/customers.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<G2Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC070DC399EC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}