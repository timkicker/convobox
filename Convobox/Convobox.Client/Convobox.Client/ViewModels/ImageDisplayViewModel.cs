using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Platform.Storage;
using Convobox.Client.Models;
using Convobox.Server;
using ReactiveUI;
using SharedDefinitions;

namespace Convobox.Client.ViewModels;

public class ImageDisplayViewModel : ViewModelBase
{
    private ConvoMessage _message;
    
    public ImageDisplayViewModel(ConvoMessage message)
    {
        this._message = message;
        NavigateBackButtonCommand = ReactiveCommand.CreateFromObservable(NavigateBackCommand);
        SaveButtonCommand = ReactiveCommand.CreateFromObservable(SaveCommand);
    }
    
    public ReactiveCommand<Unit,Unit> NavigateBackButtonCommand { get; }
    public ReactiveCommand<Unit,Unit> SaveButtonCommand { get; }
    
    private IObservable<Unit> NavigateBackCommand()
    {
        return Observable.StartAsync( async() =>
        {
            NavigationStore.SwitchMainTo(NavigationStore.Dashboard);
        });
        
    }
    
    private IObservable<Unit> SaveCommand()
    {
        return Observable.StartAsync( async() =>
        {
            try
            {
                var currentFilePath = Path.Combine(PlatformInformation.GetApplicationTempImageFolder(),
                    Message.UniqueFileName);

                var topLevel = NavigationStore.TopLevel;

                var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                {
                    Title = "Save " + Message.FileName,
                    SuggestedFileName = Message.FileName,
                });

                if (!string.IsNullOrEmpty(file.Path.AbsolutePath))
                {
                    File.Copy(currentFilePath, file.Path.AbsolutePath,true);
                    Notifier.ShowCustom("Successfully saved file", Message.FileName);
                }
                else throw new Exception("Selected file storage is invalid");
            }
            catch (Exception e)
            {
                Notifier.ShowCustom("Failed to save file", e.Message);
            }

        });
    
    }

    public ConvoMessage Message
    {
        get => _message;
        set => _message = value;
    }
}