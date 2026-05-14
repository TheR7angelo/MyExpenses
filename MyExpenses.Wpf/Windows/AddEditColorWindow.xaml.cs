using System.Windows;
using System.Windows.Input;
using MyExpenses.Application.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Wpf.Windows;

public partial class AddEditColorWindow : IClosable
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public TColor Color { get; } = new();

    public bool DeleteColor { get; private set; }

    private readonly ColorManagementViewModel _colorManagementViewModel;

    public AddEditColorWindow(ColorManagementViewModel vm)
    {
        _colorManagementViewModel = vm;

        InitializeComponent();

        DataContext = _colorManagementViewModel;
    }

    #region Action

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        // if (string.IsNullOrWhiteSpace(Color.Name))
        // {
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxCannotAddEmptyColorNameErrorTitle,
        //         ColorManagementResources.MessageBoxCannotAddEmptyColorNameErrorMessage, MsgBoxImage.Error);
        //     return;
        // }
        //
        // if (string.IsNullOrWhiteSpace(Color.HexadecimalColorCode))
        // {
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxCannotAddEmptyColorHexErrorTitle,
        //         ColorManagementResources.MessageBoxCannotAddEmptyColorHexErrorMessage, MsgBoxImage.Error);
        //     return;
        // }
        //
        // var nameAlreadyExist = CheckColorName(Color.Name);
        // if (nameAlreadyExist)
        // {
        //     ShowErrorMessageDuplicateName();
        //     return;
        // }
        //
        // // ReSharper disable once HeapView.DelegateAllocation
        // var colorAlreadyExist = Colors.FirstOrDefault(s => s.HexadecimalColorCode == Color.HexadecimalColorCode);
        // if (colorAlreadyExist is not null)
        // {
        //     var message = string.Format(ColorManagementResources.MessageBoxCannotAddDuplicateColorHexErrorMessage,
        //         colorAlreadyExist.Name);
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxCannotAddDuplicateColorHexErrorTitle,
        //         message, MsgBoxImage.Error);
        //     return;
        // }
        //
        // DialogResult = true;
        // Close();
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        // var message = string.Format(ColorManagementResources.MessageBoxDeleteColorQuestionMessage, Color.Name);
        // var response = Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorQuestionTitle, message,
        //         MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
        // if (response is not MessageBoxResult.Yes) return;
        //
        // Log.Information("Attempting to remove the color \"{ColorToDeleteName}\"", Color.Name);
        // var (success, exception) = Color.Delete();
        //
        // if (success)
        // {
        //     Log.Information("Color was successfully removed");
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorNoUseSuccessTitle,
        //         ColorManagementResources.MessageBoxDeleteColorNoUseSuccessMessage, MsgBoxImage.Check);
        //
        //     DeleteColor = true;
        //     DialogResult = true;
        //     Close();
        //     return;
        // }
        //
        // if (exception!.InnerException is SqliteException
        //     {
        //         SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
        //     })
        // {
        //     Log.Error("Foreign key constraint violation");
        //
        //     response = Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorUseQuestionTitle,
        //         ColorManagementResources.MessageBoxDeleteColorUseQuestionMessage,
        //         MessageBoxButton.YesNoCancel, MsgBoxImage.Question);
        //
        //     if (response is not MessageBoxResult.Yes) return;
        //
        //     Log.Information("Attempting to remove the color \"{ColorToDeleteName}\" with all relative element",
        //         Color.Name);
        //     Color.Delete(true);
        //     Log.Information("Account and all relative element was successfully removed");
        //     Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorUseSuccessTitle,
        //         ColorManagementResources.MessageBoxDeleteColorUseSuccessMessage, MsgBoxImage.Check);
        //
        //     DeleteColor = true;
        //     DialogResult = true;
        //     Close();
        //
        //     return;
        // }
        //
        // Log.Error(exception, "An error occurred please retry");
        // Dialogs.MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorErrorTitle,
        //     ColorManagementResources.MessageBoxDeleteColorErrorMessage, MsgBoxImage.Error);
    }

    #endregion

    /// <summary>
    /// Loads the provided <paramref name="colorViewModel"/> into the current color management context.
    /// </summary>
    /// <param name="colorViewModel">
    /// The instance of <see cref="ColorViewModel"/> to load into the color management workflow.
    /// </param>
    public void LoadColorViewModel(ColorViewModel colorViewModel)
        => _colorManagementViewModel.LoadColorViewModel(colorViewModel);
}