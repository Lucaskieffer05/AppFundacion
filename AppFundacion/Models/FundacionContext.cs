using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppFundacion.Models;

public partial class FundacionContext : DbContext
{
    private readonly string? stringconnection;
    public FundacionContext()
    {
        stringconnection = Preferences.Get("stringConnection", defaultValue: null);
    }

    public FundacionContext(DbContextOptions<FundacionContext> options)
        : base(options)
    {
        stringconnection = Preferences.Get("stringConnection", defaultValue: null);
    }

    public virtual DbSet<Cobrador> Cobradores { get; set; }

    public virtual DbSet<Donante> Donantes { get; set; }

    public virtual DbSet<Zona> Zonas { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(stringconnection);

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cobrador>(entity =>
        {
            entity.Property(e => e.Codigo).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);


            entity.HasOne(d => d.IdZonaNavigation).WithMany(p => p.Cobradores)
                .HasForeignKey(d => d.IdZona)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Cobradores_Zona");
        });

        modelBuilder.Entity<Donante>(entity =>
        {
            entity.ToTable("Donante");

            entity.Property(e => e.Ciudad).HasMaxLength(100).HasColumnName("Ciudad");
            entity.Property(e => e.Dni)
                .HasMaxLength(50)
                .HasColumnName("DNI");
            entity.Property(e => e.Domicilio).HasMaxLength(50).HasColumnName("Domicilio");
            entity.Property(e => e.FechaIngreso).HasColumnType("datetime");
            entity.Property(e => e.NombreApellido).HasMaxLength(100).HasColumnName("NombreApellido");
            entity.Property(e => e.Pais).HasMaxLength(50).HasColumnName("Pais");
            entity.Property(e => e.Provincia).HasMaxLength(100).HasColumnName("Provincia");

            entity.HasOne(d => d.IdCobradorNavigation).WithMany(p => p.Donantes)
                .HasForeignKey(d => d.IdCobrador)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Donante_Cobradores");
        });

        modelBuilder.Entity<Zona>(entity =>
        {
            entity.ToTable("Zona");

            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
