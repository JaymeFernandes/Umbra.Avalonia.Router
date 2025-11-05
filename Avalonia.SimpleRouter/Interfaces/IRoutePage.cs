using Avalonia.SimpleRouter.Context;

namespace Avalonia.SimpleRouter.Interfaces;

public interface IRoutePage : IDisposable
{
    public void OnNavigatedTo(NavigationContext context);
    
    public Task OnNavigatedToAsync(NavigationContext context);
}
