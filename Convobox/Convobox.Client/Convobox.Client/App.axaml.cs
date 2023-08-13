using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Convobox.Client.Models;
using Convobox.Client.ViewModels;
using Convobox.Client.Views;
using DesktopNotifications;
using DesktopNotifications.FreeDesktop;
using DesktopNotifications.Windows;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using SharedDefinitions;

namespace Convobox.Client;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    


    public override void OnFrameworkInitializationCompleted()
    {
        var login = new LoginViewModel();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(login)
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel(login)
            };
        }
        
        Notifier.CreateManager();

        // Settings
        ThemeManager.App = this;
        try
        {
            Settings.Load();
            Settings.Current.LoadTheme();
            Settings.Current.LoadPrimaryColor();
            
            ThemeManager.SetTheme(Settings.Current.Theme);
            ThemeManager.SetPrimaryColor(Settings.Current.ColorTheme);
            
             login.Username = Settings.Current.Username;
             login.Password = Settings.Current.GetPassword();

             if (login.Username.Length > 0 && login.Password.Length > 0)
                 login.TryLoginCommand.Execute();
        }
        catch (Exception e)
        {
            ThemeManager.UpdateTheme();
        }
        

        NavigationStore.InternLogger.Log("Application", $"Startup");

        base.OnFrameworkInitializationCompleted();
    }
    
    

    public static class ThemeManager
    {
        public static Convobox.Client.App App { get; set; }
        public static IBaseTheme CurrentTheme { get; private set; } = Theme.Dark;

        public static Avalonia.Media.Color CurrentPrimaryColor { get; private set; } = GetAvaloniaColor(Definition.Blue);

        public static Avalonia.Media.Color GetAvaloniaColor(System.Drawing.Color color)
        {
            return Avalonia.Media.Color.FromRgb(color.R, color.G, color.B);
        }

        public static void SetDefaults()
        {
            SetPrimaryColor(GetAvaloniaColor(Definition.Blue));
            SetTheme(Theme.Dark);
        }
        
        
        public static void SetPrimaryColor(Avalonia.Media.Color color)
        {
            CurrentPrimaryColor = color;
            NavigationStore.InternLogger.Log("Theme", $"Set theme color to {color.ToString()}");
            UpdateTheme();

        }

        public static void SetTheme(IBaseTheme theme)
        {
            CurrentTheme = theme;
            UpdateTheme();
            NavigationStore.InternLogger.Log("Theme", $"Set theme color to {theme.ToString()}");
        }

        public static void UpdateTheme()
        {
            Avalonia.Media.Color secondaryColor = GetAvaloniaColor(Definition.Blue);
            var theme = Theme.Create(CurrentTheme, CurrentPrimaryColor, secondaryColor);
            var themeBootstrap = App.LocateMaterialTheme<MaterialThemeBase>();

            Settings.Current.Theme = CurrentTheme;

            Dispatcher.UIThread.Post(() => { themeBootstrap.CurrentTheme = theme; }, DispatcherPriority.Default);

        }
    }
    


}