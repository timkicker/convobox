using System;
using Avalonia.Controls;
using Convobox.Client.Models;
using Convobox.Client.ViewModels;

namespace Convobox.Client;

public class NavigationStore
{
    private static MainViewModel _mainWindowViewModel;
    
    public static void SwitchMainTo(ViewModelBase viewModel)
    {
        _mainWindowViewModel.CurrentViewModel = viewModel;
        NavigationStore.InternLogger.Log("Navigation", $"Switched to {nameof(viewModel)}");
    }
    public static MainViewModel MainWindowViewModel
    {
        get => _mainWindowViewModel;
        set => _mainWindowViewModel = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static Window MainWindow { get; set; }
    public static InternLoggerViewModel InternLogger { get; set; } = new InternLoggerViewModel();
}