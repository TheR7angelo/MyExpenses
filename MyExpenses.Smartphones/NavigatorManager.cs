using Serilog;

namespace MyExpenses.Smartphones;

public static class NavigatorManager
{
    private static readonly Lock NavigationLock = new();
    private static readonly HashSet<Type> NavigationsInProgress = [];

    /// <summary>
    /// Navigates to the specified page of the given type. If the type cannot be instantiated,
    /// an error is logged and the navigation does not occur. Prevents duplicate navigation
    /// by default unless explicitly overridden.
    /// </summary>
    /// <param name="type">The type of the page to navigate to, which must derive from ContentPage.</param>
    /// <param name="preventDuplicates">
    /// Determines whether duplicate navigation should be prevented. Defaults to true.
    /// </param>
    /// <returns>A task that represents the asynchronous navigation operation.</returns>
    public static async Task NavigateToAsync(this Type type, bool preventDuplicates = true)
    {
        if (Activator.CreateInstance(type) is not ContentPage contentPage)
        {
            Log.Error("Unable to create instance of {TypeFullName}", type.FullName);
            return;
        }

        await contentPage.NavigateToAsync(type, preventDuplicates);
    }

    /// Navigates to the specified page asynchronously.
    /// <param name="contentPage">
    /// The target content page to navigate to.
    /// </param>
    /// <param name="preventDuplicates">
    /// A boolean value to indicate whether to prevent duplicate navigation to the same page.
    /// Defaults to true.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation of the navigation.
    /// </returns>
    public static async Task NavigateToAsync(this ContentPage contentPage, bool preventDuplicates = true)
    {
        var type = contentPage.GetType();
        await NavigateToAsync(contentPage, type, preventDuplicates);
    }

    /// <summary>
    /// Navigates to a specified ContentPage type asynchronously, with optional duplicate prevention.
    /// </summary>
    /// <param name="contentPage">The ContentPage instance to navigate to.</param>
    /// <param name="type">The type of the ContentPage to navigate to.</param>
    /// <param name="preventDuplicates">A boolean indicating whether to prevent navigating to a page that already exists in the navigation stack.</param>
    /// <returns>A task representing the asynchronous navigation operation.</returns>
    private static async Task NavigateToAsync(this ContentPage contentPage, Type type, bool preventDuplicates)
    {
        lock (NavigationLock)
        {
            if (!NavigationsInProgress.Add(type)) return;
        }

        try
        {
            var navigationStack = Shell.Current.Navigation.NavigationStack;

            if (preventDuplicates &&
                navigationStack.OfType<ContentPage>()
                    .Any(page => page.GetType() == type))
            {
                return;
            }

            await Shell.Current.Navigation.PushAsync(contentPage);
        }
        finally
        {
            lock (NavigationLock) NavigationsInProgress.Remove(type);
        }
    }
}