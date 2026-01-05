using System.Threading;
using System.Threading.Tasks;
using Umbra.Avalonia.Router.Base;

namespace RouterSample.ViewModels.Shared;

public abstract class PageViewModelBase : PageRouterBase
{
    public override Task OnNavigatedToAsync(CancellationToken ctx)
      => Task.CompletedTask;
}
