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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=C:/Users/ZP6177/Documents/Programmation/C#/Myexpenses/MyExpenses.Sql/Database Models/Model.sqlite");

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
