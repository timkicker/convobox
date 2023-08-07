using System;
using System.Collections.ObjectModel;
using Convobox.Server;

namespace Convobox.Client.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private static ObservableCollection<ConvoMessage> _history = new ObservableCollection<ConvoMessage>();

    
    
    public static ObservableCollection<ConvoMessage> History
    {
        get => _history;
        set => _history = value;
    }
}