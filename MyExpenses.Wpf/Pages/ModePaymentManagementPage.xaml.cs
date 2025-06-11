using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.ModePaymentManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Resources.Resx.Pages.ModePaymentManagementPage;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class ModePaymentManagementPage
{
    public ObservableCollection<TModePayment> ModePayments { get; }

    public ModePaymentManagementPage()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];

        InitializeComponent();
    }

    private void ButtonAddNewModePayment_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditModePaymentWindow is created locally within this method and is used temporarily.
        // Since there are no references to it after this scope and the Garbage Collector will handle
        // its cleanup efficiently, this allocation is intentional and does not require further optimization.
        var addEditModePaymentWindow = new AddEditModePaymentWindow();
        var result = addEditModePaymentWindow.ShowDialog();
        if (result is not true) return;

        var newModePayment = addEditModePaymentWindow.ModePayment;

        Log.Information("Attempting to inject the new mode payment \"{NewModePaymentName}\"", newModePayment.Name);
        var (success, exception) = newModePayment.AddOrEdit();
        if (success)
        {
            ModePayments.AddAndSort(newModePayment, s => s.Name!);

            Log.Information("New mode payment was successfully added");
            var json = newModePayment.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(ModePaymentManagementResources.MessageBoxAddModePaymentSuccessTitle,
                ModePaymentManagementResources.MessageBoxAddModePaymentSuccessMessage,
                MessageBoxButton.OK, MsgBoxImage.Check);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(ModePaymentManagementResources.MessageBoxAddModePaymentErrorTitle,
                ModePaymentManagementResources.MessageBoxAddModePaymentErrorMessage,
                MessageBoxButton.OK, MsgBoxImage.Error);
        }
    }

    private void ButtonEditModePayment_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not TModePayment modePaymentToEdit) return;

        if (modePaymentToEdit.CanBeDeleted is false)
        {
            MsgBox.Show(ModePaymentManagementPageResources.MessageBoxEditModePaymentNoEditOrDelete, MsgBoxImage.Error);
            return;
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The instance of AddEditModePaymentWindow is created locally within this method and is used temporarily.
        // Since there are no references to it after this scope and the Garbage Collector will handle
        // its cleanup efficiently, this allocation is intentional and does not require further optimization.
        var addEditModePaymentWindow = new AddEditModePaymentWindow();
        addEditModePaymentWindow.SetTModePayment(modePaymentToEdit);

        var result = addEditModePaymentWindow.ShowDialog();
        if (result is not true) return;

        if (addEditModePaymentWindow.ModePaymentDeleted) ModePayments.Remove(modePaymentToEdit);
        else
        {
            var updatedModePayment = addEditModePaymentWindow.ModePayment;

            Log.Information("Attempting to update mode payment id:\"{UpdatedModePaymentId}\", name:\"{UpdatedModePaymentName}\"",updatedModePayment.Id, updatedModePayment.Name);
            var (success, exception) = updatedModePayment.AddOrEdit();
            if (success)
            {
                ModePayments.AddAndSort(modePaymentToEdit, updatedModePayment, s => s.Name!);

                Log.Information("Mode payment was successfully edited");
                var json = updatedModePayment.ToJsonString();
                Log.Information("{Json}", json);

                MsgBox.Show(ModePaymentManagementResources.MessageBoxEditModePaymentSuccessTitle,
                    ModePaymentManagementResources.MessageBoxEditModePaymentSuccessMessage,
                    MsgBoxImage.Check);
            }
            else
            {
                Log.Error(exception, "An error occurred please retry");
                MsgBox.Show(ModePaymentManagementResources.MessageBoxEditModePaymentErrorTitle,
                    ModePaymentManagementResources.MessageBoxEditModePaymentErrorMessage,
                    MsgBoxImage.Error);
            }
        }
    }
}