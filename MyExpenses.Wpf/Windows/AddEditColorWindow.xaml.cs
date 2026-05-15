using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Wpf.Windows;

/// <summary>
/// Represents a window that facilitates the addition or modification of color configurations
/// in the application. This window interacts with the color management workflow by binding
/// to the <see cref="ColorManagementViewModel"/>.
/// </summary>
/// <remarks>
/// The <see cref="AddEditColorWindow"/> is designed for managing the UI logic
/// for creating or editing colors, leveraging the <see cref="ColorManagementViewModel"/>
/// for the underlying data operations and workflows. It implements the <see cref="IClosable"/>
/// interface, allowing it to interact seamlessly with commands that require closing or returning
/// dialog results.
/// </remarks>
public partial class AddEditColorWindow : IClosable
{
    /// <summary>
    /// Represents the ViewModel instance used for managing color configurations in the
    /// AddEditColorWindow. This field provides data-binding to the window and facilitates
    /// operations such as loading, editing, validating, and deleting color-related data.
    /// </summary>
    private readonly ColorManagementViewModel _colorManagementViewModel;

    /// <summary>
    /// Represents a window for adding or editing color configurations within the application.
    /// Provides the user interface layer to interact with the color management process, including
    /// binding to the associated ViewModel to enable data manipulation and workflow execution.
    /// </summary>
    public AddEditColorWindow(ColorManagementViewModel vm)
    {
        _colorManagementViewModel = vm;

        InitializeComponent();

        DataContext = _colorManagementViewModel;
    }

    /// <summary>
    /// Loads the provided <paramref name="colorViewModel"/> into the current color management context.
    /// </summary>
    /// <param name="colorViewModel">
    /// The instance of <see cref="ColorViewModel"/> to load into the color management workflow.
    /// </param>
    public void LoadColorViewModel(ColorViewModel colorViewModel)
        => _colorManagementViewModel.LoadColorViewModel(colorViewModel);
}