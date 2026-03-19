using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Interfaces;

public interface IRoutePage : IDisposable
{
    public NavigationContext Context { get; }
    
    public Task InitializeAsync(NavigationContext context);

    protected Task OnNavigatedToAsync(CancellationToken ctx);
}
