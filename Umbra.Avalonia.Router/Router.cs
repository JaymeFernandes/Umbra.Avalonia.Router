using Umbra.Avalonia.Router.Configuration;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace Umbra.Avalonia.Router;

public class Router<TViewModelBase> : RouterBase<TViewModelBase> where TViewModelBase : class, IRoutePage
{
    private string CurrentRouter = "";

    public Router(IServiceProvider serviceProvider, RouterConfig<TViewModelBase> options) : base(serviceProvider, options)
    {
    }

    public event Action<string>? TitleChanged;

    public TViewModelBase Navigate(string url, string title, object body = null)
    {
        var context = new NavigationContext(url, body, config.Scheme, config.AppName);

        var destination = ResolveViewModel(url, body);

        if (CurrentRouter == context.CurrentUrl)
            return destination;

        CurrentViewModel = destination;
        CurrentRouter = context.CurrentUrl;

        TitleChanged?.Invoke(title);

        return destination;
    }
}
