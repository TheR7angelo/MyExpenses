using System.Collections.ObjectModel;
using System.Windows;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Sql.Context;

namespace MyExpenses.Wpf.Pages;

public partial class ModePaymentManagementPage
{
    public required DashBoardPage DashBoardPage { get; init; }

    public ObservableCollection<TModePayment> ModePayments { get; }

    public ModePaymentManagementPage()
    {
        using var context = new DataBaseContext();
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    private void ButtonAddNewModePayment_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO work
        Console.WriteLine("Add new mode payment");
    }

    private void ButtonEditModePayment_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO work
        Console.WriteLine("Edit mode payment");
    }
}