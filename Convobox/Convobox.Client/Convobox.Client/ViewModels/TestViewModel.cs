using Material.Icons;

namespace Convobox.Client.ViewModels;

public class TestViewModel : ViewModelBase
{
    public TestViewModel()
    {
        Title = "Test";
        Icon = MaterialIconKind.Abacus;
    }
}