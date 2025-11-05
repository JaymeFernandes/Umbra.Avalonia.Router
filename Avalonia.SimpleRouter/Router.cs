using Avalonia.Controls;
using Avalonia.SimpleRouter.Configuration;
using Avalonia.SimpleRouter.Context;
using Avalonia.SimpleRouter.Interfaces;
using Avalonia.SimpleRouter.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Avalonia.SimpleRouter;

public class Router<TViewModelBase> where TViewModelBase : class, IRoutePage
{
    private readonly IServiceProvider _serviceProvider;
    private TViewModelBase _currentViewModel = default!;
    private Dictionary<Type, Type> _routes;

    public event Action<Control>? CurrentViewModelChanged;

    public Router(IServiceProvider serviceProvider, RouterConfig options)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _routes = options._pages;
    }

    protected TViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (_currentViewModel == value) return;
            _currentViewModel = value;
            
            if (!_routes.TryGetValue(_currentViewModel.GetType(), out var controlType))
                throw new Exception($"No view registered for {_currentViewModel.GetType().Name}");

            var control = ActivatorUtilities.CreateInstance(_serviceProvider, controlType) as Control;
        
            control.DataContext = _currentViewModel;
            
            CurrentViewModelChanged?.Invoke(control);
        }
    }

    public virtual TViewModelBase GoTo(string url, object? body = null)
    {
        var vm = ResolveViewModel(url, body);
        CurrentViewModel = vm;
        return vm;
    }

    protected TViewModelBase ResolveViewModel(string url, object? body)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Route cannot be null or empty.", nameof(url));

        var normalizedUrl = url.StartsWith("app://", StringComparison.OrdinalIgnoreCase)
            ? url
            : "app://" + url;

        var routerMapper = _serviceProvider.GetRequiredService<RouterMapper>();
        var vm = routerMapper.Resolve(normalizedUrl, body) as TViewModelBase
                 ?? throw new InvalidOperationException($"Route '{normalizedUrl}' could not be resolved.");
        
        return vm;
    }
    
    protected TViewModelBase ResolveViewModel(NavigationContext context)
    {
        var url = context.CurrentUrl;
        
        if (string.IsNullOrWhiteSpace(context.CurrentUrl))
            throw new ArgumentException("Route cannot be null or empty.", nameof(url));

        var normalizedUrl = url.StartsWith("app://", StringComparison.OrdinalIgnoreCase)
            ? url
            : "app://" + url;

        var routerMapper = _serviceProvider.GetRequiredService<RouterMapper>();
        var vm = routerMapper.Resolve(context) as TViewModelBase
                 ?? throw new InvalidOperationException($"Route '{normalizedUrl}' could not be resolved.");

        return vm;
    }
}