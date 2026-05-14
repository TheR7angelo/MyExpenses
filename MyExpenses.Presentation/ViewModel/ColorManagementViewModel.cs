using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Domain.Models.Validation;
using Microsoft.Extensions.Logging;
using MyExpenses.Application.Interfaces;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Mappings.Interfaces;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Presentation.ViewModel;

public partial class ColorManagementViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly ISystemDtoViewModelMapper _systemDtoViewModelMapper;
    private readonly ILogger<ColorManagementViewModel> _logger;
    private readonly ISystemActionService _systemActionService;

    [ObservableProperty]
    public partial bool IsEditColor { get; set; }

    public ColorViewModel ColorViewModel { get; } = new();

    public IRelayCommand<IClosable?> CancelCommand { get; }
    public IAsyncRelayCommand<IClosable?> ValidCommand { get; }

    public ColorManagementViewModel(ISystemActionService systemActionService,
        IDialogService dialogService,
        ISystemDtoViewModelMapper systemDtoViewModelMapper,
        ILogger<ColorManagementViewModel> logger)
    {
        _systemActionService = systemActionService;
        _dialogService = dialogService;
        _systemDtoViewModelMapper = systemDtoViewModelMapper;
        _logger = logger;

        CancelCommand = new RelayCommand<IClosable?>(OnCancel);
        ValidCommand = new AsyncRelayCommand<IClosable?>(OnValidAsync);
    }

    private async Task OnValidAsync(IClosable? dialog)
    {
        try
        {
            var result = IsEditColor
                ? await _systemActionService.UpdateColorAsync(ColorViewModel)
                : await _systemActionService.CreateColorAsync(ColorViewModel);

            if (result.IsSuccess) dialog?.Close();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating color");

            // TODO trad
            _dialogService.ShowMessageBox("Error", "An error occurred while creating the color. Please try again.",
                MessageBoxButton.Ok, MsgBoxImage.Error);
            throw;
        }
    }

    /// <summary>
    /// Handles the cancellation of the current operation by closing the dialog
    /// and setting its <see cref="IClosable.DialogResult"/> to false.
    /// </summary>
    /// <param name="dialog">
    /// The instance of <see cref="IClosable"/> representing the dialog to be closed.
    /// If null, no action is performed.
    /// </param>
    private void OnCancel(IClosable? dialog)
    {
        dialog?.DialogResult = false;
        dialog?.Close();
    }

    /// <summary>
    /// Loads the specified <see cref="ColorViewModel"/> into the current instance,
    /// setting the appropriate state for color editing and merging the properties
    /// from the source view model into the target view model.
    /// </summary>
    /// <param name="colorViewModel">
    /// The source <see cref="ColorViewModel"/> containing the color properties to load.
    /// </param>
    public void LoadColorViewModel(ColorViewModel colorViewModel)
    {
        IsEditColor = true;
        _systemDtoViewModelMapper.Merge(colorViewModel, ColorViewModel);

        ColorViewModel.AcceptChanges();
    }
}