using Avalonia.Controls;
using Umbra.Avalonia.Router.Context;

namespace Umbra.Avalonia.Router.Interfaces;

public interface IRoutePage : IDisposable
{
    public Task InitializeAsync(NavigationContext context);

    protected Task OnNavigatedToAsync(CancellationToken ctx);
}
