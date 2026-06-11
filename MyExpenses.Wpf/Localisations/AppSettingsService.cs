using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using MyExpenses.Presentation.Mappings;
using MyExpenses.Presentation.ViewModels.Systems;

namespace MyExpenses.Wpf.Localisations;

/// <summary>
/// Service singleton that holds application settings (view model) in memory,
/// notifies the UI of property changes and persists the settings to disk
/// when a dirty change is detected.
///
/// Usage:
/// - Call <see cref="Initialize"/> at startup with the view model loaded from the application layer.
/// - Optionally pass a custom persist function to control how settings are written.
/// </summary>
public sealed partial class AppSettingsService : ObservableObject, IDisposable
{
    public static AppSettingsService Instance { get; } = new();

    private readonly object _sync = new();
    private readonly HashSet<INotifyPropertyChanged> _subscriptions = new();
    private bool _saving;
    private CancellationTokenSource? _debounceCts;
    private int _debounceMilliseconds = 1000;
    private ILogger? _logger;

    [ObservableProperty]
    private AppSettingsViewModel _settings = new();

    /// <summary>Raised when any property of a tracked settings object changes. The event args come from the underlying PropertyChanged.</summary>
    public event EventHandler<PropertyChangedEventArgs?>? SettingsPropertyChanged;

    /// <summary>Raised after settings have been persisted to disk and AcceptChanges() has been called.</summary>
    public event EventHandler? SettingsSaved;

    /// <summary>
    /// Optional custom persist function. If not provided, a default writer will map the view model to DTO
    /// and write a pretty-printed JSON file to AppDomain.CurrentDomain.BaseDirectory/appsettings.json.
    /// </summary>
    private Func<AppSettingsViewModel, Task>? _persistFunc;

    private AppSettingsService() { }

    /// <summary>
    /// Initialize the service with the in-memory settings instance and an optional persist function.
    /// This will subscribe to property changes recursively and start monitoring dirty state.
    /// </summary>
    public void Initialize(AppSettingsViewModel settings, Func<AppSettingsViewModel, Task>? persistFunc = null,
        ILogger? logger = null, int debounceMilliseconds = 1000)
    {
        ArgumentNullException.ThrowIfNull(settings);

        lock (_sync)
        {
            UnsubscribeAll();

            Settings = settings;
            _persistFunc = persistFunc;

            _logger = logger;
            _debounceMilliseconds = debounceMilliseconds;

            // subscribe to known settings tree (no reflection)
            SubscribeSettingsTree(settings);
        }
    }

    // Subscribe only to the known structure of AppSettingsViewModel to avoid reflection.
    private void SubscribeSettingsTree(AppSettingsViewModel? settings)
    {
        if (settings is null) return;

        SubscribeIfNeeded(settings);

        // SystemSettings
        if (settings.SystemSettings is INotifyPropertyChanged sys)
        {
            SubscribeIfNeeded(sys);
        }

        // InterfaceSettings and nested Theme/Clock
        if (settings.InterfaceSettings is not INotifyPropertyChanged iface) return;

        SubscribeIfNeeded(iface);

        var ifaceType = settings.InterfaceSettings;
        if (ifaceType.Theme is INotifyPropertyChanged theme) SubscribeIfNeeded(theme);
        if (ifaceType.Clock is INotifyPropertyChanged clock) SubscribeIfNeeded(clock);
    }

    private void SubscribeIfNeeded(INotifyPropertyChanged? npc)
    {
        if (npc is null) return;
        if (_subscriptions.Add(npc)) npc.PropertyChanged += OnAnyPropertyChanged;
    }

    private void UnsubscribeAll()
    {
        foreach (var sub in _subscriptions)
        {
            try
            {
                sub.PropertyChanged -= OnAnyPropertyChanged;
            }
            catch
            {
                // ignored
            }
        }

        _subscriptions.Clear();
    }

    private async void OnAnyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Forward the notification to listeners (UI)
        SettingsPropertyChanged?.Invoke(this, e);

        // Log the property change
        try
        {
            _logger?.LogDebug("AppSettings property changed: {Property}", e?.PropertyName);
        }
        catch { }

        // debounce SaveAsync to coalesce rapid changes
        lock (_sync)
        {
            try
            {
                _debounceCts?.Cancel();
            }
            catch { }

            _debounceCts = new CancellationTokenSource();
            var token = _debounceCts.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(_debounceMilliseconds, token).ConfigureAwait(false);

                    // after debounce delay, if still dirty, save
                    if (Settings is TheR7angelo.DirtyTracking.Abstractions.IDirtyTrackable { IsDirty: true } dirtyRoot)
                    {
                        // log what will be saved
                        try
                        {
                            var pending = dirtyRoot.PendingChanges;
                            if (pending.Count > 0)
                            {
                                var summary = string.Join(", ", pending.Select(p => $"{p.PropertyName}: '{p.OldValueDisplay}' → '{p.NewValueDisplay}'"));
                                _logger?.LogInformation("Persisting AppSettings after debounce. Changes: {Summary}", summary);
                            }
                            else
                            {
                                _logger?.LogInformation("Persisting AppSettings after debounce (no pending changes details).");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, "Failed to build pending changes summary");
                        }

                        await SaveAsync().ConfigureAwait(false);
                    }
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Error during AppSettings debounce save");
                }
            }, token);
        }
    }

    /// <summary>
    /// Persist settings to disk (using the provided persist function or the default writer)
    /// and call AcceptChanges() on the view model to reset dirty state.
    /// </summary>
    public async Task SaveAsync()
    {
        lock (_sync)
        {
            if (_saving) return;
            _saving = true;
        }

        try
        {
            if (_persistFunc is not null)
            {
                _logger?.LogInformation("Persisting AppSettings via custom persist function");
                await _persistFunc(Settings);
            }
            else
            {
                // Default behavior: map to DTO and write appsettings.json in base dir
                var mapper = new SystemDtoViewModelMapper();
                var dto = mapper.MapToDto(Settings);

                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var appSettingsPath = Path.Combine(baseDirectory, "appsettings.json");
                _logger?.LogInformation("Persisting AppSettings to {Path}", appSettingsPath);

                var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(appSettingsPath, json);
            }

            // Use IDirtyTrackable.AcceptChanges when available to reset dirty flags
            try
            {
                if (Settings is TheR7angelo.DirtyTracking.Abstractions.IDirtyTrackable dt)
                {
                    dt.AcceptChanges();
                }
            }
            catch
            {
                // ignore
            }

            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            lock (_sync)
            {
                _saving = false;
            }
        }
    }

    public void Dispose()
    {
        UnsubscribeAll();
        try { _debounceCts?.Cancel(); } catch { }
        _debounceCts?.Dispose();
        GC.SuppressFinalize(this);
    }
}

