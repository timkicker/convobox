using System;
using System.Drawing;
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
    public SettingsViewModel()
    {
        SelectedTheme = App.ThemeManager.CurrentTheme;
        SelectedColor = Color.FromArgb(App.ThemeManager.CurrentPrimaryColor.R, App.ThemeManager.CurrentPrimaryColor.G,
            App.ThemeManager.CurrentPrimaryColor.B);
        Title = "Settings";
        Icon = MaterialIconKind.Settings;
    }

    public Color SelectedColor
    {
        get => _selectedColor;
        set => _selectedColor = this.RaiseAndSetIfChanged(ref _selectedColor, value);
    }

    public IBaseTheme SelectedTheme
    {
        get => _selectedTheme;
        set => _selectedTheme = this.RaiseAndSetIfChanged(ref _selectedTheme, value);
    }

    public IBaseTheme[] Themes { get; } = { Theme.Dark, Theme.Light };

    public System.Drawing.Color[] ThemeColors
    {
        get
        {
            return Definition.ThemeColors;
        }
    }
}