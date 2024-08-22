using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace G_IPG_API.Models.Wallet;

public partial class GWalletDbContext : DbContext
{
    public GWalletDbContext()
    {
    }

    public GWalletDbContext(DbContextOptions<GWalletDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionConfirmation> TransactionConfirmations { get; set; }

    public virtual DbSet<TransactionMode> TransactionModes { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<WalletBankAccount> WalletBankAccounts { get; set; }

    public virtual DbSet<WalletCurrency> WalletCurrencies { get; set; }

    public virtual DbSet<Xchenger> Xchengers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=194.60.231.81:5432;Database=G_Wallet_DB;Username=postgres;Password=Maham@7796", x => x.UseNodaTime());

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

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UnitType_pkey");

            entity.ToTable("Currency");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Status_pkey");

            entity.ToTable("Status");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Transaction_pkey");

            entity.ToTable("Transaction");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Info).HasColumnType("json");
            entity.Property(e => e.OrderId).HasColumnType("character varying");
            entity.Property(e => e.TrackingCode).HasColumnType("character varying");
        });

        modelBuilder.Entity<TransactionConfirmation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TRansactionConfirmation_pkey");

            entity.ToTable("TransactionConfirmation");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RequestDescription).HasMaxLength(300);
            entity.Property(e => e.ResponceDescription).HasMaxLength(300);
            entity.Property(e => e.TransactionInfo).HasColumnType("json");
        });

        modelBuilder.Entity<TransactionMode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TransactionMode_pkey");

            entity.ToTable("TransactionMode");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TransactionType_pkey");

            entity.ToTable("TransactionType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Unit_pkey");

            entity.ToTable("Unit");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Wallet_pkey");

            entity.ToTable("Wallet");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<WalletBankAccount>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("WalletBankAccount");

            entity.Property(e => e.BankAccountNumber).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Shaba).HasMaxLength(20);
            entity.Property(e => e.ValidationInfo).HasMaxLength(20);
        });

        modelBuilder.Entity<WalletCurrency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("WalletCurrency_pkey");

            entity.ToTable("WalletCurrency");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.WcAddress)
                .HasMaxLength(64)
                .HasColumnName("WC_Address");
        });

        modelBuilder.Entity<Xchenger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("XChenger_pkey");

            entity.ToTable("XChenger");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DestinationAddress).HasMaxLength(64);
            entity.Property(e => e.SourceAddress).HasMaxLength(64);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
