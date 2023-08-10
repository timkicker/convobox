using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Convobox.Client.Models;
using Newtonsoft.Json.Linq;

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
        Settings.Save();
    }
}