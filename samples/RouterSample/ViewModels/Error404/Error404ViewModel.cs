using CommunityToolkit.Mvvm.ComponentModel;
using RouterSample.ViewModels.Shared;
using System.Threading;
using System.Threading.Tasks;
using Umbra.Avalonia.Router.Context;

namespace RouterSample.ViewModels.Error404;

public partial class Error404ViewModel : PageViewModelBase
{
    [ObservableProperty] private string _route;

    public override Task OnNavigatedToAsync(CancellationToken ctx)
    {
        Route = $"Not Found: {Context.CurrentUrl}";

        return Task.CompletedTask;
    }
}
