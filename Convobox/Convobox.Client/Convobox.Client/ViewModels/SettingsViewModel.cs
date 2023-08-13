using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using Convobox.Client.Models;
using Material.Icons;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using ReactiveUI;
using SharedDefinitions;

namespace Convobox.Client.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private System.Drawing.Color _selectedColor;
    private IBaseTheme _selectedTheme;
    private bool _selectedNotificationSettings;
    public SettingsViewModel()
    {
        SelectedNotificationSettings = Settings.Current.Notifications;
        SelectedTheme = App.ThemeManager.CurrentTheme;
        SelectedColor = Color.FromArgb(App.ThemeManager.CurrentPrimaryColor.R, App.ThemeManager.CurrentPrimaryColor.G,
            App.ThemeManager.CurrentPrimaryColor.B);
        Title = "Settings";
        Icon = MaterialIconKind.Settings;
        SettingsButtonCommand = ReactiveCommand.CreateFromObservable(SetSettingsCommand);
        Settings.TrySave();
    }

    #region commands

    public ReactiveCommand<Unit, Unit> SettingsButtonCommand { get; }

    private IObservable<Unit> SetSettingsCommand()
    {
        return Observable.Start(() =>
        {
             App.ThemeManager.SetTheme(SelectedTheme);
             App.ThemeManager.SetPrimaryColor(App.ThemeManager.GetAvaloniaColor(SelectedColor));

             Settings.Current.Theme = SelectedTheme;
             Settings.Current.ColorTheme = App.ThemeManager.GetAvaloniaColor(SelectedColor);
             Settings.Current.Notifications = SelectedNotificationSettings;
             OnPropertyChanged(nameof(SaveButtonEnabled));
        });
    }

    #endregion

    public Color SelectedColor
    {
        get => _selectedColor;
        set
        {
            _selectedColor = this.RaiseAndSetIfChanged(ref _selectedColor, value);
            OnPropertyChanged(nameof(SaveButtonEnabled));
        }
    }

    public IBaseTheme SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            _selectedTheme = this.RaiseAndSetIfChanged(ref _selectedTheme, value);
            OnPropertyChanged(nameof(SaveButtonEnabled));
        }
    }

    public bool SelectedNotificationSettings
    {
        get => _selectedNotificationSettings;
        set
        {
            _selectedNotificationSettings = this.RaiseAndSetIfChanged(ref _selectedNotificationSettings, value);
            OnPropertyChanged(nameof(SaveButtonEnabled));
        }
    }

    public IBaseTheme[] Themes { get; } = { Theme.Dark, Theme.Light };

    public System.Drawing.Color[] ThemeColors
    {
        get
        {
            return Definition.ThemeColors;
            
        }
    }

    public bool SaveButtonEnabled
    {
        get
        {
            if (SelectedTheme != App.ThemeManager.CurrentTheme ||
                App.ThemeManager.CurrentPrimaryColor != App.ThemeManager.GetAvaloniaColor(SelectedColor) ||
                SelectedNotificationSettings != Settings.Current.Notifications)
            {
                return true;
            }
            else return false;
        }
    }
}