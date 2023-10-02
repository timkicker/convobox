using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Material.Icons;
using ReactiveUI;
using SharedDefinitions;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace Convobox.Client.ViewModels;

public class AboutViewModel : ViewModelBase
{
    private Bitmap _profileImage;
    
    public AboutViewModel()
    {
        Title = "About";
        SetProfilePictureAsync();
        Icon = MaterialIconKind.About;
        
        GithubButtonCommand = ReactiveCommand.CreateFromObservable(OpenGithubCommand);
        BlogButtonCommand = ReactiveCommand.CreateFromObservable(OpenBlogCommand);
        MailButtonCommand = ReactiveCommand.CreateFromObservable(OpenMailCommand);
    }

    #region commands

    public ReactiveCommand<Unit,Unit> GithubButtonCommand { get; }
    public ReactiveCommand<Unit,Unit> BlogButtonCommand { get; }
    public ReactiveCommand<Unit,Unit> MailButtonCommand { get; }
    
    private IObservable<Unit> OpenGithubCommand()
    {
        return Observable.Start(() =>
        {
            var uri = "https://github.com/timkicker/";
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = uri
            };
            System.Diagnostics.Process.Start(psi);
            
        });
    }
    
    private IObservable<Unit> OpenBlogCommand()
    {
        return Observable.Start(() =>
        {
            
            var uri = "https://tim.kicker.dev/";
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = uri
            };
            System.Diagnostics.Process.Start(psi);
        });
    }
    
    private IObservable<Unit> OpenMailCommand()
    {
        return Observable.Start(() =>
        {
            
            var uri = "mailto:tim.kicker@protonmail.com";
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = uri
            };
            System.Diagnostics.Process.Start(psi);
        });
    }
    

    #endregion

    private async Task SetProfilePictureAsync()
    {
        var tempFilePath = Path.Combine(PlatformInformation.GetApplicationTempFolder(), "about_profile_picture_github.jpeg");

        var downloadUri =
            new System.Uri($"https://avatars.githubusercontent.com/u/33966128?v=4");

        using (WebClient wc = new WebClient())
        {
            wc.DownloadFile(
                downloadUri,
                tempFilePath
            );
        }
        
        ProfileImage = new Bitmap(tempFilePath);
    }

    public Bitmap ProfileImage
    {
        get => _profileImage;
        set => _profileImage = this.RaiseAndSetIfChanged(ref _profileImage, value);
    }
}