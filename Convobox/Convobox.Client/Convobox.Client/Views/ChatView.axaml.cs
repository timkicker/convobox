using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Convobox.Client.ViewModels;

namespace Convobox.Client.Views;

public partial class ChatView : UserControl
{
    public ChatView()
    {
        InitializeComponent();
        ChatViewModel.ChatScrollViewer = this.ChatScrollViewer;
    }


    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        ChatScrollViewer.ScrollToEnd();
    }

    private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
    {
        
    }

    private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ChatViewModel.Current.SendButtonCommand.Execute();
        }
    }
}