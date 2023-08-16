using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Convobox.Client.Models;
using Convobox.Server;
using Material.Icons;
using Microsoft.CodeAnalysis;
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
    private IReadOnlyList<IStorageFile> _selectedFiles;
    private bool _showErrorText;
    private string _errorText;

    public ChatViewModel()
    {
        SendButtonCommand = ReactiveCommand.CreateFromObservable(SendCommand);
        RequestMoreMessagesCommand = ReactiveCommand.CreateFromObservable(RequestMoreMessages);
        SelectFilesButtonCommand = ReactiveCommand.CreateFromObservable(SelectFilesCommand);
        ClearFilesButtonCommand = ReactiveCommand.CreateFromObservable(ClearFilesCommand);

        Current = this;
        Title = "Chat";
        Icon = MaterialIconKind.Chat;
    }
    #region commands
    
    public ReactiveCommand<Unit,Unit> SendButtonCommand { get; }
    public ReactiveCommand<Unit,Unit> RequestMoreMessagesCommand { get; }
    public ReactiveCommand<Unit,Unit> SelectFilesButtonCommand { get; }
    public ReactiveCommand<Unit,Unit> ClearFilesButtonCommand { get; }

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
    
    private IObservable<Unit> SelectFilesCommand()
    {
        return Observable.StartAsync(async () =>
        {
            try
            {
                var topLevel = NavigationStore.TopLevel;


                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Choose one or more files to send",
                    AllowMultiple = true
                });



                if (files.Count > 0)
                {
                    _selectedFiles = files;
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                OnPropertyChanged(nameof(ShowSelectedFilesBox));
                OnPropertyChanged(nameof(SelectedFileInfoBoxText));
            }

        });
    }

    private IObservable<Unit> SendCommand()
    {
        return Observable.Start(() =>
        {
            string textToSend = "";
            // trim text to send if exists
            if (!string.IsNullOrEmpty(EnteredText))
            {
                textToSend = EnteredText.TrimEnd('\r', '\n').TrimStart();
            }
            
            // create commandMessage
            var commandMessage = new CommandMessge();
            
            if (_selectedFiles is null)
            {
                // if no files attached and no entered text -> dont send
                if (string.IsNullOrEmpty(EnteredText))
                    return;
                //... also if length is 0 or only whitespace -> dont send
                if (EnteredText.Length <= 0 && EnteredText.All(char.IsWhiteSpace))
                    return;
                // ... if neither -> send
                var message = new ConvoMessage()
                {
                    Data = textToSend
                };
                commandMessage = new CommandMessge(CommandType.SendMessage)
                {
                    ConvoMessage = message
                };
                
                
                // serialize and check if length is over maximum
                if (commandMessage.Serialize().Length > Definition.MaxMessageBytes)
                {
                    ErrorText = $"Could not send message: bigger than {Definition.MaxMessageBytes} bytes";
                    ShowErrorText = true;
                }
                else
                {
                    // send message
                    ClientConversationManager.Send(commandMessage);
                    ShowErrorText = false;
                }
            }
            else if (_selectedFiles.Count > 0)  // check if files selected
            {
                foreach (var file in _selectedFiles)
                {
                    string text = "";
                    // if first message -> ammit text
                    if (file == _selectedFiles[0])
                        text = textToSend;
                    
                    // get filename
                    string filePath = file.Path.AbsolutePath;
                    string fileName = file.Name;

                    // compress file
                    var compressor = new FileCompressor();
                    var randomFileName = compressor.GenerateRandomFileName(fileName);
                    var compressedFilePath = Path.Combine(PlatformInformation.GetApplicationFileStorage(), randomFileName);
                    var imageLoader = new ImageLoader();
                    bool isImage = imageLoader.IsImage(filePath);

                    // check if image compression needed
                    if (isImage)
                    {
                        compressor.CompressImage(filePath,compressedFilePath);
                        fileName = System.IO.Path.ChangeExtension(fileName, "webp");
                    }
                    else compressor.CompressFile(filePath,compressedFilePath);
                    
                    // create convomessage
                    var message = new ConvoMessage()
                    {
                        Data = text,
                        FileName = fileName
                    };
                    
                    // add file
                    message.SetFile(File.ReadAllBytes(compressedFilePath));
                    
                    // set commandmessage
                    commandMessage = new CommandMessge(CommandType.SendMessage)
                    {
                        ConvoMessage = message,
                    };
                    
                    // serialize and check if length is over maximum
                    if (commandMessage.Serialize().Length > Definition.MaxMessageBytes)
                    {
                        ErrorText = $"Could not send message: bigger than {Definition.MaxMessageBytes} bytes";
                        ShowErrorText = true;
                    }
                    else
                    {
                        // send message
                        ClientConversationManager.Send(commandMessage);
                        ShowErrorText = false;
                    }
                    
                }
                
                // clear selected files
                _selectedFiles = null;
                    
            }

            // update bindings and clear entered text
            CheckScroll();
            EnteredText = "";
            OnPropertyChanged(nameof(ShowSelectedFilesBox));
            OnPropertyChanged(nameof(SelectedFileInfoBoxText));
            OnPropertyChanged(nameof(ShowErrorText));
            OnPropertyChanged(nameof(ErrorText));
            
        });
    }
    
    private IObservable<Unit> ClearFilesCommand()
    {
        return Observable.Start(() =>
        {
            if (_selectedFiles is not null)
            {
                _selectedFiles = null;
            }
            OnPropertyChanged(nameof(ShowSelectedFilesBox));
            OnPropertyChanged(nameof(SelectedFileInfoBoxText));
        });
    }
    
    public async Task SaveFileAsync(ConvoMessage msg)
    {
        try
        {
            var topLevel = NavigationStore.TopLevel;

            string defaultPath = PlatformInformation.GetDownloadsFolder();

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Save " + msg.FileName,
                SuggestedFileName = msg.FileName,
            });

            var tempFilePath = Path.Combine(PlatformInformation.GetApplicationTempFolder(), msg.UniqueFileName);

            var downloadUri =
                new System.Uri(
                    $"http://{Settings.Current.ServerInfo.Domain}:{Settings.Current.ServerInfo.PortFiles}/{msg.UniqueFileName}");

            
            
            using (WebClient wc = new WebClient())
            {
                wc.Credentials = new NetworkCredential(Settings.Current.Username, Settings.Current.GetPassword());
                wc.DownloadFile(
                    downloadUri,
                    tempFilePath
                );
            }
            
            var compressor = new FileCompressor();
            compressor.DecompressFile(tempFilePath,file.Path.AbsolutePath);

            Notifier.ShowCustom("File downloaded", msg.FileName);
        }
        catch (Exception e)
        {
            Notifier.ShowCustom("Download failed", msg.FileName);
        }
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

    public bool ShowErrorText
    {
        get => _showErrorText;
        set => _showErrorText = this.RaiseAndSetIfChanged(ref _showErrorText, value);
    }

    public string ErrorText
    {
        get => _errorText;
        set => _errorText = this.RaiseAndSetIfChanged(ref _errorText, value);
    }

    public bool ShowSelectedFilesBox
    {
        get
        {
            if (_selectedFiles is null)
            {
                return false;
            }
            else if (_selectedFiles.Count > 0)
            {
                return true;
            }
            else return false;
        }
    }

    public string SelectedFileInfoBoxText
    {
        get
        {
            if (_selectedFiles is not null)
            {
                return $"{_selectedFiles.Count} selected file(s)";
            }
            else return "";
        }
    }
    
    public Avalonia.Media.Brush ErrorBoxColor
    {
        get
        {
            return new SolidColorBrush(App.ThemeManager.GetAvaloniaColor(Definition.Red));
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