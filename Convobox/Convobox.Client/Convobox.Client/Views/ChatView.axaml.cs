using Avalonia;
using Avalonia.Controls;
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

    
}