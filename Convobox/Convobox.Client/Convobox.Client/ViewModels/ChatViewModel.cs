using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Convobox.Client.Models;
using Convobox.Server;
using ReactiveUI;

namespace Convobox.Client.ViewModels;

public class ChatViewModel : ViewModelBase
{
    private static ObservableCollection<ConvoMessage> _history = new ObservableCollection<ConvoMessage>();
    private string _enteredText;
    private static int _messagesBefore;
    private static Vector _scrollMax;
    private static Vector _scrollMaxOld;
    private static Vector _scrollCurrent;

    public ChatViewModel()
    {
        SendButtonCommand = ReactiveCommand.CreateFromObservable(SendCommand);
    }
    #region commands
    
    public ReactiveCommand<Unit,Unit> SendButtonCommand { get; }

    #endregion

    #region methods

    public static void CheckScroll()
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
    
    private static async Task GetScrollbarLength()
    {
        _scrollCurrent = ChatScrollViewer.Offset;
        return;
    }

    private IObservable<Unit> SendCommand()
    {
        return Observable.Start(() =>
        {
            if (EnteredText.Length > 0)
            {
                var message = new ConvoMessage()
                {
                    Data = EnteredText,
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

    public static ObservableCollection<ConvoMessage> History
    {
        get => _history;
        set => _history = value;
    }
    
    // for autoscroll purpose
    public static ScrollViewer ChatScrollViewer { get; set; }
    
    
}