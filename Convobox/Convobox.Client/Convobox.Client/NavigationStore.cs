using System;
using System.Net.Mime;
using Avalonia;
using Avalonia.Controls;
using Convobox.Client.Models;
using Convobox.Client.ViewModels;
using Avalonia.Platform;

namespace Convobox.Client;

public class NavigationStore
{
    private static MainViewModel _mainWindowViewModel;
    
    public static void SwitchMainTo(ViewModelBase viewModel)
    {
        _mainWindowViewModel.CurrentViewModel = viewModel;
        NavigationStore.InternLogger.Log("Navigation", $"Switched to {nameof(viewModel)}");
    }
    
    public static void BackToLogin(string errorMessage)
    {
        var login = new LoginViewModel()
        {
            ErrorFlagContent = errorMessage,
            ErrorFlag = !string.IsNullOrEmpty(errorMessage)
        };
        SwitchMainTo(login);
    }
    
    public static MainViewModel MainWindowViewModel
    {
        get => _mainWindowViewModel;
        set => _mainWindowViewModel = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static IPlatformSettings Settings { get; set; }
    public static Window MainWindow { get; set; }
    public static InternLoggerViewModel InternLogger { get; set; } = new InternLoggerViewModel();
}