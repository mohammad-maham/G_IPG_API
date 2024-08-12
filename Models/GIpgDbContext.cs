using System;
using System.Collections.Generic;
using G_IPG_API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace G_IPG_API.Models;

public partial class GIpgDbContext : DbContext,IUnitOfWork
{
    public GIpgDbContext()
    {
    }

    public GIpgDbContext(DbContextOptions<GIpgDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<BankResult> BankResults { get; set; }

    public virtual DbSet<BankStatus> BankStatuses { get; set; }

    public virtual DbSet<LinkCall> LinkCalls { get; set; }

    public virtual DbSet<LinkRequest> LinkRequests { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=194.60.231.81:5432;Database=G_IPG_DB;Username=postgres;Password=Maham@7796", x => x.UseNodaTime());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Bank_pkey");

            entity.ToTable("Bank");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LogoPath).HasMaxLength(100);
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<BankResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BANK_RESULT_pkey");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BankInfo).HasColumnType("json");
            entity.Property(e => e.ConfirmInfo).HasColumnType("json");
            entity.Property(e => e.ReferenceNumber).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue((short)0);
        });

        modelBuilder.Entity<BankStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BankStatus_pkey");

            entity.ToTable("BankStatus");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<LinkCall>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("LinkCall_pkey");

            entity.ToTable("LinkCall");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RequestAddress).HasMaxLength(200);
            entity.Property(e => e.TokenInfo).HasColumnType("json");
        });

        modelBuilder.Entity<LinkRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("LinkRequest_pkey");

            entity.ToTable("LinkRequest");

            entity.Property(e => e.RequestId).ValueGeneratedNever();
            entity.Property(e => e.AccLinkReqConf).HasMaxLength(500);
            entity.Property(e => e.CallbackUrl)
                .HasMaxLength(300)
                .HasColumnName("CallbackURL");
            entity.Property(e => e.ClientMobile).HasPrecision(10);
            entity.Property(e => e.FactorDetail).HasColumnType("json");
            entity.Property(e => e.Guid)
                .HasMaxLength(100)
                .HasColumnName("GUID");
            entity.Property(e => e.OrderId).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Status_pkey");

            entity.ToTable("Status");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status1)
                .HasDefaultValue((short)0)
                .HasColumnName("Status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
