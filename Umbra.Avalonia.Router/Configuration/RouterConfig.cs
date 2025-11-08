using Avalonia.Controls;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace Umbra.Avalonia.Router.Configuration;

public class RouterConfig
{
    internal readonly Dictionary<string, Type> _routes = new Dictionary<string, Type>();
    internal readonly Dictionary<Type, Type> _pages = new Dictionary<Type, Type>();
    
    public int HistorySize { get; set; } = 10;
    public string Scheme { get; set; } = "app";
    public string AppName { get; set; } = "MyApp";
    
    public void Register<TPage, TViewModel>(string route) where TViewModel : class, IRoutePage where TPage : Control
    {
        if (_pages.ContainsKey(typeof(TViewModel)))
            throw new InvalidOperationException($"ViewModel '{typeof(TViewModel).Name}' already registered with a Page.");
        
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("Route must not be null or empty.", nameof(route));

        var context = new NavigationContext(route, null, Scheme, AppName);
        
        if (_routes.ContainsKey(context.CurrentUrl))
            throw new InvalidOperationException($"Route '{route}' already registered.");
        
        
        _routes[context.CurrentUrl] = typeof(TViewModel);
        _pages.Add(typeof(TViewModel), typeof(TPage));
    }
}