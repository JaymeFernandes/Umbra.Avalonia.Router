using Avalonia.Controls;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace Umbra.Avalonia.Router.Configuration;

public class RouterConfig<T> where T : class, IRoutePage
{
    internal readonly Dictionary<Type, Type> _pages = new Dictionary<Type, Type>();

    internal readonly Dictionary<string, Type> _routes = new Dictionary<string, Type>();

    public string AppName { get; set; } = "MyApp";

    public int HistorySize { get; set; } = 10;

    public string Scheme { get; set; } = "app";

    public void Register<TPage, TViewModel>(string route) where TViewModel : class, T, IRoutePage where TPage : Control
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("Route must not be null or empty.", nameof(route));

        var context = new NavigationContext(route, null, Scheme, AppName);

        if (_routes.ContainsKey(context.CurrentUrl))
            throw new InvalidOperationException($"Route '{route}' already registered.");

        _routes[context.CurrentUrl] = typeof(TViewModel);

        if (!_pages.ContainsKey(typeof(TViewModel)))
            _pages.Add(typeof(TViewModel), typeof(TPage));
    }

    public void SetPage404<TPage, TViewModel>() where TViewModel : class, T, IRoutePage where TPage : Control
    {
        var route = "404";

        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("Route must not be null or empty.", nameof(route));

        var context = new NavigationContext(route, null, Scheme, AppName);

        _routes[context.CurrentUrl] = typeof(TViewModel);

        if (!_pages.ContainsKey(typeof(TViewModel)))
            _pages.Add(typeof(TViewModel), typeof(TPage));
    }
}
