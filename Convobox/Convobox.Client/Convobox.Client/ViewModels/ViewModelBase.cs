using System.ComponentModel;
using System.Runtime.CompilerServices;
using Material.Icons;
using ReactiveUI;

namespace Convobox.Client.ViewModels;

public class ViewModelBase : ReactiveObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    // The calling member's name will be used as the parameter.
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    
    public MaterialIconKind Icon { get; set; }
    
    public string Title { get; set; }
}