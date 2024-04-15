using Microsoft.EntityFrameworkCore;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;

namespace MyExpenses.Sql.Context;

public partial class DataBaseContext : DbContext
{
    public DataBaseContext()
    {
    }

    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TAccount> TAccounts { get; set; }

    public virtual DbSet<TAccountType> TAccountTypes { get; set; }

    public virtual DbSet<TCategoryType> TCategoryTypes { get; set; }

    public virtual DbSet<THistory> THistories { get; set; }

    public virtual DbSet<TModePayment> TModePayments { get; set; }

    public virtual DbSet<TPlace> TPlaces { get; set; }

    public virtual DbSet<VValueByMonthYear> VValueByMonthYears { get; set; }

    public virtual DbSet<VValueByMonthYearCategory> VValueByMonthYearCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // const string dbPath = @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\MyExpenses.Sql\Database Models\Model.sqlite";
        // const string dbPath = @"C:\Users\ZP6177\Documents\Programmation\C#\MyExpenses\MyExpenses.Sql\Database Models\Model - Using.sqlite";
        const string dbPath = @"C:\Users\Rapha\RiderProjects\MyExpenses\MyExpenses.Sql\Database Models\Model - Using.sqlite";
        const string dataSource = $"Data Source={dbPath}";
        
        optionsBuilder.UseSqlite(dataSource);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<THistory>(entity =>
        {
            entity.Property(e => e.Pointed).HasDefaultValueSql("FALSE");
        });

        modelBuilder.Entity<TPlace>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<VValueByMonthYear>(entity =>
        {
            entity.ToView("v_value_by_month_year");
        });

        modelBuilder.Entity<VValueByMonthYearCategory>(entity =>
        {
            entity.ToView("v_value_by_month_year_category");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
