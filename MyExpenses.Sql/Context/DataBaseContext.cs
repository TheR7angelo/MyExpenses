using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;

namespace MyExpenses.Sql.Context;

public partial class DataBaseContext : DbContext
{
    public static string? FilePath { get; set; }

    private string? TempFilePath { get; set; }

    private string? DataSource { get; set; }

    public DataBaseContext(string? filePath=null)
    {
        if (!string.IsNullOrEmpty(filePath)) TempFilePath = filePath;
    }

    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAccount> TAccounts { get; set; }

    public virtual DbSet<TAccountType> TAccountTypes { get; set; }

    public virtual DbSet<TBankTransfer> TBankTransfers { get; set; }

    public virtual DbSet<TCategoryType> TCategoryTypes { get; set; }

    public virtual DbSet<TColor> TColors { get; set; }

    public virtual DbSet<TCurrency> TCurrencies { get; set; }

    public virtual DbSet<THistory> THistories { get; set; }

    public virtual DbSet<TModePayment> TModePayments { get; set; }

    public virtual DbSet<TPlace> TPlaces { get; set; }

    public virtual DbSet<TSupportedLanguage> TSupportedLanguages { get; set; }

    public virtual DbSet<TVersion> TVersions { get; set; }

    public virtual DbSet<VAccountMonthlyCumulativeSum> VAccountMonthlyCumulativeSums { get; set; }

    public virtual DbSet<VBankTransfer> VBankTransfers { get; set; }

    public virtual DbSet<VDetailTotalCategory> VDetailTotalCategories { get; set; }

    public virtual DbSet<VHistory> VHistories { get; set; }

    public virtual DbSet<VTotalByAccount> VTotalByAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DataSource = !string.IsNullOrEmpty(TempFilePath)
            ? $"Data Source={TempFilePath};Pooling=False"
            : $"Data Source={FilePath};Pooling=False";

        var dataSource = DataSource;

        optionsBuilder.UseSqlite(dataSource)
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TAccount>(entity =>
        {
            entity.Property(e => e.Active).HasDefaultValueSql("TRUE");
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TAccountType>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TBankTransfer>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TCategoryType>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TColor>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TCurrency>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<THistory>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Pointed).HasDefaultValueSql("FALSE");
        });

        modelBuilder.Entity<TModePayment>(entity =>
        {
            entity.Property(e => e.CanBeDeleted).HasDefaultValue(true);
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TPlace>(entity =>
        {
            entity.Property(e => e.CanBeDeleted).HasDefaultValueSql("TRUE");
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsOpen).HasDefaultValueSql("TRUE");
        });

        modelBuilder.Entity<TSupportedLanguage>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<VAccountMonthlyCumulativeSum>(entity =>
        {
            entity.ToView("v_account_monthly_cumulative_sum");
        });

        modelBuilder.Entity<VBankTransfer>(entity =>
        {
            entity.ToView("v_bank_transfer");
        });

        modelBuilder.Entity<VDetailTotalCategory>(entity =>
        {
            entity.ToView("v_detail_total_category");
        });

        modelBuilder.Entity<VHistory>(entity =>
        {
            entity.ToView("v_history");
        });

        modelBuilder.Entity<VTotalByAccount>(entity =>
        {
            entity.ToView("v_total_by_account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
