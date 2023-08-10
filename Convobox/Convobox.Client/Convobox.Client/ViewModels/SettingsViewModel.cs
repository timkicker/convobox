using Material.Icons;

namespace Convobox.Client.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel()
    {
        Title = "Settings";
        Icon = MaterialIconKind.Settings;
    }
}