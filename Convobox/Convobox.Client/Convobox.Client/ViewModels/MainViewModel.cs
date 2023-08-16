using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using Convobox.Client.Views;
using ReactiveUI;
using Splat;

namespace Convobox.Client.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    
    
    public MainViewModel(LoginViewModel login)
    {
        CurrentViewModel = login;
        NavigationStore.MainWindowViewModel = this;
        
        // register vm/v
        Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));
        Locator.CurrentMutable.Register(() => new ChatView(), typeof(IViewFor<ChatViewModel>));
        Locator.CurrentMutable.Register(() => new DashboardView(), typeof(IViewFor<DashboardViewModel>));
        Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));
        Locator.CurrentMutable.Register(() => new ImageDisplayView(), typeof(IViewFor<ImageDisplayView>));
        
        
    }
    
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = this.RaiseAndSetIfChanged(ref _currentViewModel, value);
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
    
    public class MyCoolObservableExceptionHandler : IObserver<Exception>
    {
        public void OnNext(Exception value)
        {
                
        }

        public void OnError(Exception error)
        {
            if (Debugger.IsAttached) Debugger.Break();

                

            RxApp.MainThreadScheduler.Schedule(() => { throw error; });
        }

        public void OnCompleted()
        {
            if (Debugger.IsAttached) Debugger.Break();
            RxApp.MainThreadScheduler.Schedule(() => { throw new NotImplementedException(); });
        }
    }
}