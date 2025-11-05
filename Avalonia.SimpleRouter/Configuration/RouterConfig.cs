using Avalonia.Controls;
using Avalonia.SimpleRouter.Interfaces;

namespace Avalonia.SimpleRouter.Configuration;

public class RouterConfig
{
    private readonly Dictionary<string, Type> _routes = new Dictionary<string, Type>();
    internal readonly Dictionary<Type, Type> _pages = new Dictionary<Type, Type>();
    internal int HistorySize { get; set; } = 10;
    
    public void Register<TPage, TViewModel>(string route) where TViewModel : class, IRoutePage where TPage : Control
    {
        _pages.Add(typeof(TViewModel), typeof(TPage));
        
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("A URL não pode ser nula ou vazia.", nameof(route));

        var normalizedUrl = route.StartsWith("app://", StringComparison.OrdinalIgnoreCase)
            ? route
            : "app://" + route;

        var uri = new Uri(normalizedUrl);

        var key = $"{uri.Host}{uri.AbsolutePath}";
        _routes[key] = typeof(TViewModel);
    }

    internal Dictionary<string, Type> GetMapRoute()
        => _routes;
}