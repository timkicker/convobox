using System;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media;
using Avalonia.Media.TextFormatting.Unicode;
using Convobox.Client.Models;
using Convobox.Server;
using ReactiveUI;

namespace Convobox.Client.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private bool _connecting;
    private string _portString;
    private string _address;
    private bool _errorFlag;
    private string _errorFlagContent;
    private string _username;
    private string _password;
    private bool _buttonsVisible;

    public LoginViewModel()
    {
        _portString = "5000";
        _address = "localhost";
        TryLoginCommand = ReactiveCommand.CreateFromObservable(LoginButtonCommand);
        TryRegisterCommand = ReactiveCommand.CreateFromObservable(RegisterButtonCommand);

        Username = "admin";
        Password = Username;
    }
    
    #region commands

    public ReactiveCommand<Unit, Unit> TryLoginCommand { get; }
    public ReactiveCommand<Unit,Unit> TryRegisterCommand { get; }

    #endregion

    #region methods

    public void CheckButtonVisibility()
    {
        if (Username?.Length < 3 || Password?.Length < 3 || Address?.Length < 3 || PortString?.Length < 1)
        {
            ButtonsVisible = false;
        }
        else ButtonsVisible = true;
    }

    #endregion
    
    #region  async

    private IObservable<Unit> LoginButtonCommand()
    {
        return Observable.Start(() =>
        {
            ErrorFlag = false;
            ErrorFlagContent = " ";
            Connecting = true;
            try
            {
                ClientConversationManager.Connect(Address, PortString);
            }
            catch (Exception e)
            {
                ErrorFlag = true;
                ErrorFlagContent = "Could not connect to server";
                Connecting = false;
                return;
            }

            if (ClientConversationManager.IsConnected)
            {
                ClientConversationManager.Login(Username,Password);
            }
            else
            {
                ClientConversationManager.CloseConnection();
                    
                ErrorFlagContent = "Could not connect to server";
                ErrorFlag = true;
                Connecting = false;
            }
            
            if (ClientConversationManager.IsLoggedIn)
            {
                var getRooms = new CommandMessge()
                {
                    Amount = 100,
                    Type = CommandType.MessagesReq
                };
                
                ClientConversationManager.Send(getRooms);
                
                // switch view
                var chatViewModel = new ChatViewModel();
                NavigationStore.SwitchMainTo(chatViewModel);
            }
            else
            {
                ClientConversationManager.CloseConnection();
                    
                ErrorFlagContent = "Login failed \n(Please check your data)";
                ErrorFlag = true;
      
            }
            Connecting = false;

            
        });
    }
    
    private IObservable<Unit> RegisterButtonCommand()
    {
        return Observable.Start(() =>
        {
            ErrorFlag = false;
            ErrorFlagContent = " ";
            Connecting = true;
            try
            {
                ClientConversationManager.Connect(Address, PortString);
            }
            catch (Exception e)
            {
                ErrorFlag = true;
                ErrorFlagContent = "Could not connect to server";
                Connecting = false;
                return;
            }

            if (ClientConversationManager.IsConnected)
            {
                ClientConversationManager.Register(Username,Password);
            }
            else
            {
                ClientConversationManager.CloseConnection();
                    
                ErrorFlagContent = "Could not connect to server";
                ErrorFlag = true;
                Connecting = false;
            }
            
            if (ClientConversationManager.IsLoggedIn)
            {
                
                var getRooms = new CommandMessge()
                {
                    Amount = 100,
                    Type = CommandType.MessagesReq
                };
                
                ClientConversationManager.Send(getRooms);
                // switch view
                var chatViewModel = new ChatViewModel();
                NavigationStore.SwitchMainTo(chatViewModel);
            }
            else
            {
                ClientConversationManager.CloseConnection();
                    
                ErrorFlagContent = "Register failed \n(Name unavailable?)";
                ErrorFlag = true;
      
            }
            Connecting = false;

            
        });
    }

    #endregion
    public bool Connecting
    {
        get => _connecting;
        set
        {
            _connecting = this.RaiseAndSetIfChanged(ref _connecting, value);
            OnPropertyChanged(nameof(ShowProcessIndicator));
            OnPropertyChanged(nameof(ShowTextFields));
            OnPropertyChanged(nameof(ButtonsVisible));
            OnPropertyChanged(nameof(_buttonsVisible));
        }
    }

    public bool ButtonsVisible
    {
        get
        {
            return _buttonsVisible && ShowTextFields;
        }
        set
        {
            _buttonsVisible = this.RaiseAndSetIfChanged(ref _buttonsVisible, value);
            OnPropertyChanged(nameof(ButtonsVisible));
            OnPropertyChanged(nameof(_buttonsVisible));
        }
    }

    public string PortString
    {
        get => _portString;
        set
        {
            _portString = value;
            CheckButtonVisibility();
        }
    }

    public string Address
    {
        get => _address;
        set
        {
            _address = value;
            CheckButtonVisibility();
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            _username = this.RaiseAndSetIfChanged(ref _username, value);
            CheckButtonVisibility();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _username = this.RaiseAndSetIfChanged(ref _password, value);
            CheckButtonVisibility();
        }
    }

    public bool ShowProcessIndicator
    {
        get => Connecting;
    }
    
    public bool ShowTextFields
    {
        get => !Connecting;
    }
    
    public bool ErrorFlag
    {
        get => _errorFlag;
        set
        {
            _errorFlag = this.RaiseAndSetIfChanged(ref _errorFlag, value);
            OnPropertyChanged(nameof(ErrorFlag));
            OnPropertyChanged(nameof(_errorFlag));
        }
    }

    public string ErrorFlagContent
    {
        get => _errorFlagContent;
        set
        {
            _errorFlagContent = this.RaiseAndSetIfChanged(ref _errorFlagContent, value);
            OnPropertyChanged(nameof(_errorFlagContent));
            OnPropertyChanged(nameof(ErrorFlagContent));
        }
    }
    
    public Avalonia.Media.Brush ErrorFlagBrush
    {
        get
        {
            return new SolidColorBrush(App.DefinedValues.Red);
        }
    }
}