using Umbra.Avalonia.Router.Context;

namespace Umbra.Avalonia.Router.Interfaces;

public interface IRoutePage : IDisposable
{
    public void OnNavigatedTo(NavigationContext context);
    
    public Task OnNavigatedToAsync(NavigationContext context);
}
