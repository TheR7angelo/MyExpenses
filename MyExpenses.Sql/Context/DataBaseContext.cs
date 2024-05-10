using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;

namespace MyExpenses.Sql.Context;

public partial class DataBaseContext : DbContext
{
    private string? FilePath { get; }

    public static string? DataSource { get; set; }

    public DataBaseContext(string? filePath=null)
    {
        FilePath = filePath;
    }

    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAccount> TAccounts { get; set; }

    public virtual DbSet<TAccountType> TAccountTypes { get; set; }

    public virtual DbSet<TCategoryType> TCategoryTypes { get; set; }

    public virtual DbSet<TColor> TColors { get; set; }

    public virtual DbSet<TCurrency> TCurrencies { get; set; }

    public virtual DbSet<THistory> THistories { get; set; }

    public virtual DbSet<TModePayment> TModePayments { get; set; }

    public virtual DbSet<TPlace> TPlaces { get; set; }

    public virtual DbSet<VDetailTotalCategory> VDetailTotalCategories { get; set; }

    public virtual DbSet<VHistory> VHistories { get; set; }

    public virtual DbSet<VTotalByAccount> VTotalByAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (string.IsNullOrEmpty(DataSource))
        {
            string dbPath;
            if (string.IsNullOrEmpty(FilePath))
            {
                // const string dbPath = @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\MyExpenses.Sql\Database Models\Model.sqlite";
                const string dbPathPro = @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\MyExpenses.Sql\Database Models\Model - Using.sqlite";
                const string dbPathPersonnel = @"C:\Users\Rapha\Documents\Programmation\MyExpenses\MyExpenses.Sql\Database Models\Model - Using.sqlite";
                const string dbPathPortable = @"C:\Users\Rapha\RiderProjects\MyExpenses\MyExpenses.Sql\Database Models\Model - Using.sqlite";

                var username = Environment.UserName;
                dbPath = username switch
                {
                    "ZP6177" => dbPathPro,
                    "Rapha" => File.Exists(dbPathPersonnel) ? dbPathPersonnel : dbPathPortable,
                    _ => dbPathPortable
                };
            }
            else dbPath = FilePath;
            DataSource = $"Data Source={dbPath}";
        }

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
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TPlace>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
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
