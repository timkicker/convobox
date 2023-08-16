using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Convobox.Client.Models;
using Convobox.Client.ViewModels;
using Convobox.Server;

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
        var getInfo = new CommandMessge()
        {
            Type = CommandType.GetServerInfoReq
        };
        ClientConversationManager.Send(getInfo);
        
        NavigationStore.TopLevel = TopLevel.GetTopLevel(this);
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

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        ConvoMessage myValue = (ConvoMessage)((Button)sender).Tag;

        ChatViewModel.Current.SaveFileAsync(myValue);
    }

    private void ViewImageButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ConvoMessage myValue = (ConvoMessage)((Button)sender).Tag;

        var display = new ImageDisplayViewModel(myValue);
        NavigationStore.SwitchMainTo(display);
    }
}