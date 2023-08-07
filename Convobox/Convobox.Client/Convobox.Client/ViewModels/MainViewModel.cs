﻿using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using Convobox.Client.Views;
using ReactiveUI;
using Splat;

namespace Convobox.Client.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    
    
    public MainViewModel()
    {
        CurrentViewModel = new LoginViewModel();
        NavigationStore.MainWindowViewModel = this;
        
        // register vm/v
        Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));
        Locator.CurrentMutable.Register(() => new ChatView(), typeof(IViewFor<ChatViewModel>));
        
        
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