using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Convobox.Client.ViewModels;
using Convobox.Client.Views;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace Convobox.Client;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }
        
        ThemeManager.App = this;
        ThemeManager.UpdateTheme();

        NavigationStore.InternLogger.Log("Application", $"Startup");

        base.OnFrameworkInitializationCompleted();
    }

    public static class ThemeManager
    {
        public static Convobox.Client.App App { get; set; }
        public static IBaseTheme CurrentTheme { get; private set; } = Theme.Dark;
        public static Avalonia.Media.Color CurrentPrimaryColor { get; private set; } = DefinedValues.LightBlue;


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
            Avalonia.Media.Color secondaryColor = DefinedValues.LightBlue;
            var theme = Theme.Create(CurrentTheme, CurrentPrimaryColor, secondaryColor);
            var themeBootstrap = App.LocateMaterialTheme<MaterialThemeBase>();


            Dispatcher.UIThread.Post(() => { themeBootstrap.CurrentTheme = theme; }, DispatcherPriority.Default);

        }
    }
    
    public static class DefinedValues
    {



        // primary colors
        public static Avalonia.Media.Color LightGray { get; } = Avalonia.Media.Color.FromRgb(170, 170, 170);
        public static Avalonia.Media.Color DarkBlue { get; } = Avalonia.Media.Color.FromRgb(20, 50, 115);
        public static Avalonia.Media.Color Black { get; } = Avalonia.Media.Color.FromRgb(0, 0, 0);


        // primary colors
        public static Avalonia.Media.Color DarkGrey { get; } = Avalonia.Media.Color.FromRgb(60, 60, 60);
        public static Avalonia.Media.Color LightBlue { get; } = Avalonia.Media.Color.FromRgb(0, 120, 190);
        public static Avalonia.Media.Color Petrol { get; } = Avalonia.Media.Color.FromRgb(0, 100, 115);
        public static Avalonia.Media.Color Orange { get; } = Avalonia.Media.Color.FromRgb(245, 155, 0);
        public static Avalonia.Media.Color Red { get; } = Avalonia.Media.Color.FromRgb(255, 23, 68);
        public static Avalonia.Media.Color DarkRed { get; } = Avalonia.Media.Color.FromRgb(150, 5, 55);


        // additional ( created my be, not internal b2 standard)

        public static Avalonia.Media.Color SecondaryGreen { get; } = Avalonia.Media.Color.FromRgb(0, 160, 80);
        public static Avalonia.Media.Color SecondaryDarkGreen { get; } = Avalonia.Media.Color.FromRgb(0, 80, 40);
        public static Avalonia.Media.Color Navy { get; } = Avalonia.Media.Color.FromRgb(63, 73, 95);
        public static Avalonia.Media.Color PrimaryYellow { get; } = Avalonia.Media.Color.FromRgb(255, 255, 0);
        public static Avalonia.Media.Color PrimaryPink { get; } = Avalonia.Media.Color.FromRgb(255, 105, 180);
    }

}