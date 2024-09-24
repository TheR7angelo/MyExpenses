using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Models.Sql.Bases.Views.Exports;
using Serilog.Extensions.Logging;

namespace MyExpenses.Sql.Context;

public partial class DataBaseContext : DbContext
{
    private static readonly ILoggerFactory LoggerFactory = new SerilogLoggerFactory();

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

    public virtual DbSet<AnalysisVAccountCategoryMonthlySum> AnalysisVAccountCategoryMonthlySums { get; set; }

    public virtual DbSet<AnalysisVAccountCategoryMonthlySumPositiveNegative> AnalysisVAccountCategoryMonthlySumPositiveNegatives { get; set; }

    public virtual DbSet<AnalysisVAccountModePaymentCategoryMonthlySum> AnalysisVAccountModePaymentCategoryMonthlySums { get; set; }

    public virtual DbSet<AnalysisVAccountMonthlyCumulativeSum> AnalysisVAccountMonthlyCumulativeSums { get; set; }

    public virtual DbSet<AnalysisVBudgetMonthly> AnalysisVBudgetMonthlies { get; set; }

    public virtual DbSet<AnalysisVBudgetMonthlyGlobal> AnalysisVBudgetMonthlyGlobals { get; set; }

    public virtual DbSet<AnalysisVBudgetPeriodAnnual> AnalysisVBudgetPeriodAnnuals { get; set; }

    public virtual DbSet<AnalysisVBudgetPeriodAnnualGlobal> AnalysisVBudgetPeriodAnnualGlobals { get; set; }

    public virtual DbSet<AnalysisVBudgetTotalAnnual> AnalysisVBudgetTotalAnnuals { get; set; }

    public virtual DbSet<AnalysisVBudgetTotalAnnualGlobal> AnalysisVBudgetTotalAnnualGlobals { get; set; }

    public virtual DbSet<ExportVAccount> ExportVAccounts { get; set; }

    public virtual DbSet<ExportVAccountType> ExportVAccountTypes { get; set; }

    public virtual DbSet<ExportVCurrency> ExportVCurrencies { get; set; }

    public virtual DbSet<ExportVRecursiveFrequency> ExportVRecursiveFrequencies { get; set; }

    public virtual DbSet<TAccount> TAccounts { get; set; }

    public virtual DbSet<TAccountType> TAccountTypes { get; set; }

    public virtual DbSet<TBankTransfer> TBankTransfers { get; set; }

    public virtual DbSet<TCategoryType> TCategoryTypes { get; set; }

    public virtual DbSet<TColor> TColors { get; set; }

    public virtual DbSet<TCurrency> TCurrencies { get; set; }

    public virtual DbSet<TGeometryColumn> TGeometryColumns { get; set; }

    public virtual DbSet<THistory> THistories { get; set; }

    public virtual DbSet<TModePayment> TModePayments { get; set; }

    public virtual DbSet<TPlace> TPlaces { get; set; }

    public virtual DbSet<TRecursiveExpense> TRecursiveExpenses { get; set; }

    public virtual DbSet<TRecursiveFrequency> TRecursiveFrequencies { get; set; }

    public virtual DbSet<TSpatialRefSy> TSpatialRefSys { get; set; }

    public virtual DbSet<TSupportedLanguage> TSupportedLanguages { get; set; }

    public virtual DbSet<TVersion> TVersions { get; set; }

    public virtual DbSet<VBankTransfer> VBankTransfers { get; set; }

    public virtual DbSet<VDetailTotalCategory> VDetailTotalCategories { get; set; }

    public virtual DbSet<VHistory> VHistories { get; set; }

    public virtual DbSet<VRecursiveExpense> VRecursiveExpenses { get; set; }

    public virtual DbSet<VTotalByAccount> VTotalByAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DataSource = !string.IsNullOrEmpty(TempFilePath)
            ? $"Data Source={TempFilePath}"
            : $"Data Source={FilePath}";
        DataSource = $"{DataSource};Pooling=False";

        var dataSource = DataSource;

        optionsBuilder.UseSqlite(dataSource,
                builder => builder.UseNetTopologySuite())
            // .UseLoggerFactory(LoggerFactory)
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalysisVAccountCategoryMonthlySum>(entity =>
        {
            entity.ToView("analysis_v_account_category_monthly_sum");
        });

        modelBuilder.Entity<AnalysisVAccountCategoryMonthlySumPositiveNegative>(entity =>
        {
            entity.ToView("analysis_v_account_category_monthly_sum_positive_negative");
        });

        modelBuilder.Entity<AnalysisVAccountModePaymentCategoryMonthlySum>(entity =>
        {
            entity.ToView("analysis_v_account_mode_payment_category_monthly_sum");
        });

        modelBuilder.Entity<AnalysisVAccountMonthlyCumulativeSum>(entity =>
        {
            entity.ToView("analysis_v_account_monthly_cumulative_sum");
        });

        modelBuilder.Entity<AnalysisVBudgetMonthly>(entity =>
        {
            entity.ToView("analysis_v_budget_monthly");
        });

        modelBuilder.Entity<AnalysisVBudgetMonthlyGlobal>(entity =>
        {
            entity.ToView("analysis_v_budget_monthly_global");
        });

        modelBuilder.Entity<AnalysisVBudgetPeriodAnnual>(entity =>
        {
            entity.ToView("analysis_v_budget_period_annual");
        });

        modelBuilder.Entity<AnalysisVBudgetPeriodAnnualGlobal>(entity =>
        {
            entity.ToView("analysis_v_budget_period_annual_global");
        });

        modelBuilder.Entity<AnalysisVBudgetTotalAnnual>(entity =>
        {
            entity.ToView("analysis_v_budget_total_annual");
        });

        modelBuilder.Entity<AnalysisVBudgetTotalAnnualGlobal>(entity =>
        {
            entity.ToView("analysis_v_budget_total_annual_global");
        });

        modelBuilder.Entity<ExportVAccount>(entity =>
        {
            entity.ToView("export_v_account");
        });

        modelBuilder.Entity<ExportVAccountType>(entity =>
        {
            entity.ToView("export_v_account_type");
        });

        modelBuilder.Entity<ExportVCurrency>(entity =>
        {
            entity.ToView("export_v_currency");
        });

        modelBuilder.Entity<ExportVRecursiveFrequency>(entity =>
        {
            entity.ToView("export_v_recursive_frequency");
        });

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

        modelBuilder.Entity<TGeometryColumn>(entity =>
        {
            entity.HasOne(d => d.Sr).WithMany(p => p.TGeometryColumns).OnDelete(DeleteBehavior.ClientSetNull);
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

        modelBuilder.Entity<TRecursiveExpense>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ForceDeactivate).HasDefaultValueSql("FALSE");
            entity.Property(e => e.IsActive).HasDefaultValueSql("TRUE");
            entity.Property(e => e.PlaceFk).HasDefaultValue(0);

            entity.HasOne(d => d.FrequencyFkNavigation).WithMany(p => p.TRecursiveExpenses).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TSpatialRefSy>(entity =>
        {
            entity.Property(e => e.Srid).ValueGeneratedNever();
        });

        modelBuilder.Entity<TSupportedLanguage>(entity =>
        {
            entity.Property(e => e.DateAdded).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.DefaultLanguage).HasDefaultValueSql("FALSE");
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

        modelBuilder.Entity<VRecursiveExpense>(entity =>
        {
            entity.ToView("v_recursive_expense");
        });

        modelBuilder.Entity<VTotalByAccount>(entity =>
        {
            entity.ToView("v_total_by_account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
