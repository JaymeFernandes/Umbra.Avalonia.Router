using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RouterSample.ViewModels.Shared;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace RouterSample.ViewModels.Error404;

public partial class Error404ViewModel : PageViewModelBase
{
    [ObservableProperty] private string _route;

    public override void OnNavigatedTo(NavigationContext context)
    {
        Route = $"Not Found: {context.CurrentUrl}";
    }
}