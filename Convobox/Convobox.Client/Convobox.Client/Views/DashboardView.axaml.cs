using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Convobox.Client.Models;
using Convobox.Client.ViewModels;

namespace Convobox.Client.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        //LeftDrawer.SwitchLeftDrawerOpened();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        string content = (sender as Button).Tag.ToString();

        ViewModelBase selectedView = new ViewModelBase();

        foreach (var viewModel in DashboardViewModel.Current.UserViews)
        {
            if (viewModel.Title == content)
            {
                selectedView = viewModel;
                break;
            }
                
        }

        DashboardViewModel.Current.SelectedView = selectedView;

    }

    private void LeftDrawer_OnLoaded(object? sender, RoutedEventArgs e)
    {
        LeftDrawer.SwitchLeftDrawerOpened();
    }


}