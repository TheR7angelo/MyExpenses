using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents the color management page in the application, responsible for managing
/// color-related functionalities and providing a visual interface for users. This page
/// is linked to the <see cref="ColorManagementViewModel"/> and initializes its data
/// context on a load. Additionally, it ensures asynchronous execution of the ViewModel's
/// load command upon page load.
/// </summary>
public partial class ColorManagementPage
{
    /// <summary>
    /// Represents the color management page in the application. This page is designed to
    /// provide users with a visual interface for managing colors, offering functionalities
    /// for viewing, editing, and organizing color-related data. It serves as an entry point
    /// to interact with the corresponding ViewModel, <see cref="ColorManagementViewModel"/>.
    /// </summary>
    public ColorManagementPage(ColorManagementViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
        Loaded += async (_, _) => await vm.LoadCommand.ExecuteAsync(null);
    }
}