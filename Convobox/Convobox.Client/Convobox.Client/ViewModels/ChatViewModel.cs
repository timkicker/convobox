using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Convobox.Client.Models;
using Convobox.Server;
using Material.Icons;
using ReactiveUI;
using SharedDefinitions;

namespace Convobox.Client.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private ObservableCollection<ConvoMessage> _history = new ObservableCollection<ConvoMessage>();
    private string _enteredText;
    private int _messagesBefore;
    private Vector _scrollMax;
    private Vector _scrollMaxOld;
    private Vector _scrollCurrent;
    private int _messagesSum;

    public ChatViewModel()
    {
        SendButtonCommand = ReactiveCommand.CreateFromObservable(SendCommand);
        RequestMoreMessagesCommand = ReactiveCommand.CreateFromObservable(RequestMoreMessages);
        Current = this;
        Title = "Chat";
        Icon = MaterialIconKind.Chat;
    }
    #region commands
    
    public ReactiveCommand<Unit,Unit> SendButtonCommand { get; }
    public ReactiveCommand<Unit,Unit> RequestMoreMessagesCommand { get; }

    #endregion

    #region methods

    public void UpdateForNewMessages()
    {
        OnPropertyChanged(nameof(ShowGetButton));
        OnPropertyChanged(nameof(History));
        OnPropertyChanged(nameof(ConvoMessage.Space));
    }

    public void CheckScroll()
    {
        var currentMax = ChatScrollViewer.ScrollBarMaximum;
        
        // get current scroll
        Dispatcher.UIThread.Post(() => GetScrollbarLength(), 
            DispatcherPriority.Background);
        
        // meine theorie: er holt zwar den offset, geht aber schon weiter da er nicht awaited und bekommt so nicht den neune wert vor dem check
        
        Thread.Sleep(50);
        
        if ((_messagesBefore != History.Count) && _scrollCurrent.Y >= _scrollMaxOld.Y)
        {
            Dispatcher.UIThread.Post(() => Scroll(), 
                DispatcherPriority.Background);
            _messagesBefore = History.Count;
        }

        _scrollMaxOld = currentMax;
    } 

    #endregion
    
    #region async
    
    private static async Task Scroll()
    {
        ChatScrollViewer.ScrollToEnd();
        return;
    }
    
    private async Task GetScrollbarLength()
    {
        _scrollCurrent = ChatScrollViewer.Offset;
        return;
    }

    private IObservable<Unit> SendCommand()
    {
        return Observable.Start(() =>
        {
            if (string.IsNullOrEmpty(EnteredText))
                return;
            
            if (EnteredText.Length > 0 && !EnteredText.All(char.IsWhiteSpace))
            {
                
                EnteredText = EnteredText.TrimEnd('\r', '\n');
                var message = new ConvoMessage()
                {
                    Data = EnteredText.TrimStart(),
                };

                var commandMessage = new CommandMessge(CommandType.SendMessage)
                {
                    ConvoMessage = message
                };
                
                ClientConversationManager.Send(commandMessage);
                EnteredText = "";
            }
            
        });
    }
    
    private IObservable<Unit> RequestMoreMessages()
    {
        return Observable.Start(() =>
        {
            if (ShowGetButton)
            {
                var msg = new CommandMessge()
                {
                    Type = CommandType.MessagesReq,
                    Amount = Definition.MessageGetDefault + History.Count
                };
                
                ClientConversationManager.Send(msg);
            }
            
        });
    }

    #endregion

    public string EnteredText
    {
        get => _enteredText;
        set
        {
            _enteredText = this.RaiseAndSetIfChanged(ref _enteredText, value);
            OnPropertyChanged(nameof(EnteredText));
            OnPropertyChanged(nameof(_enteredText));
            
        }
    }

    public ObservableCollection<ConvoMessage> History
    {
        get => _history;
        set => _history = value;
    }

    public bool ShowGetButton
    {
        get
        {
            if (History.Count >= Definition.MessageGetDefault)
            {
                if (MessagesSum == 0 || MessagesSum > History.Count)
                {
                    return true;
                }

                return false;
            }
                
            else return false;
        }
    }

    public int MessagesSum
    {
        get => _messagesSum;
        set => _messagesSum = value;
    }
    
    public static ChatViewModel Current { get; set; }

    // for autoscroll purpose
    public static ScrollViewer ChatScrollViewer { get; set; }
    
    
}