using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Convobox.Client.Models;
using ReactiveUI;

namespace Convobox.Client.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private ChatViewModel _chatViewModel;
    private SettingsViewModel _settingsViewModel;
    private ObservableCollection<ViewModelBase> _userViews;
    private ViewModelBase _selectedView;
    private bool _toggleButtonChecked;
    
    public DashboardViewModel()
    {
        Current = this;
        _chatViewModel = new ChatViewModel();
        _settingsViewModel = new SettingsViewModel();
        SelectedView = _chatViewModel;
        _userViews = new ObservableCollection<ViewModelBase>();
        _userViews.Add(_chatViewModel);
        _userViews.Add(_settingsViewModel);
        ToggleButtonChecked = false;
        LogoutButtonCommand = ReactiveCommand.CreateFromObservable(LogoutCommand);
    }

    #region commands

    public ReactiveCommand<Unit,Unit> LogoutButtonCommand { get; }
    
    private IObservable<Unit> LogoutCommand()
    {
        return Observable.Start(() =>
        {
            ClientConversationManager.CloseConnection();
            NavigationStore.BackToLogin("Logged out");
        });
    }

    #endregion
    
    #region properties

    public static DashboardViewModel Current { get; set; }
    
    public ChatViewModel ChatViewModel
    {
        get => _chatViewModel;
        set => _chatViewModel = value ?? throw new ArgumentNullException(nameof(value));
    }

    public SettingsViewModel SettingsViewModel
    {
        get => _settingsViewModel;
        set => _settingsViewModel = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ObservableCollection<ViewModelBase> UserViews
    {
        get => _userViews;
        set => _userViews = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ViewModelBase SelectedView
    {
        get => _selectedView;
        set
        {
            _selectedView = this.RaiseAndSetIfChanged(ref _selectedView, value);
            OnPropertyChanged(nameof(SelectedCarouselIndex));
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(SelectedView.Title));
        }
    }

    public bool ToggleButtonChecked
    {
        get => _toggleButtonChecked;
        set => _toggleButtonChecked = this.RaiseAndSetIfChanged(ref _toggleButtonChecked, value);
    }
    
    public int SelectedCarouselIndex
    {
        get
        {
            return UserViews.IndexOf(SelectedView);
        }
    }

    #endregion
}