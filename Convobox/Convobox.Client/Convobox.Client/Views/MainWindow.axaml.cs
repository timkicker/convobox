using Avalonia.Controls;
using Convobox.Client.Models;

namespace Convobox.Client.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        ClientConversationManager.CloseConnection();
    }
}