using MyExpenses.Presentation.ViewModel;

namespace MyExpenses.Wpf.Pages;

/// <summary>
/// Represents the page for managing category types in the application.
/// </summary>
/// <remarks>
/// This page is associated with the <see cref="CategoryTypeManagementViewModel"/> to manage
/// category type-related data and actions. It initializes the data context with the
/// defined view model and executes the command to load all category types upon loading.
/// The page is intended to be navigated to from other parts of the application, such as the dashboard.
/// </remarks>
public partial class CategoryTypeManagementPage
{
    /// <summary>
    /// Represents the page for managing category types in the application.
    /// </summary>
    /// <remarks>
    /// This page is designed to facilitate the interaction with category type management features.
    /// It uses the <see cref="CategoryTypeManagementViewModel"/> as its data context and initializes
    /// functionality to load category type-related data upon being loaded.
    /// </remarks>
    public CategoryTypeManagementPage(CategoryTypeManagementViewModel categoryTypeManagementViewModel)
    {
        InitializeComponent();

        DataContext = categoryTypeManagementViewModel;
        Loaded += async (_, _) => await categoryTypeManagementViewModel.LoadAllCategoryTypeCommand.ExecuteAsync(null);
    }
}