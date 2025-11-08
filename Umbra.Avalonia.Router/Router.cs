using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Umbra.Avalonia.Router.Configuration;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;
using Umbra.Avalonia.Router.Services;

namespace Umbra.Avalonia.Router;

public class Router<TViewModelBase> where TViewModelBase : class, IRoutePage
{
    private readonly IServiceProvider _serviceProvider;
    private TViewModelBase _currentViewModel = default!;
    protected readonly RouterConfig config;
    private Dictionary<Type, Type> _routes;

    public event Action<Control>? PageChanged;

    public Router(IServiceProvider serviceProvider, RouterConfig options)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        config = options;
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
            
            PageChanged?.Invoke(control);
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
        
        var context = new NavigationContext(url, body, config.Scheme, config.AppName);

        var routerMapper = _serviceProvider.GetRequiredService<RouterMapper>();
        var vm = routerMapper.Resolve(context.CurrentUrl, body) as TViewModelBase
                 ?? throw new InvalidOperationException($"Route '{context.CurrentUrl}' could not be resolved.");
        
        return vm;
    }
    
    protected TViewModelBase ResolveViewModel(NavigationContext context)
    {
        var url = context.CurrentUrl;
        
        if (string.IsNullOrWhiteSpace(context.CurrentUrl))
            throw new ArgumentException("Route cannot be null or empty.", nameof(url));

        var routerMapper = _serviceProvider.GetRequiredService<RouterMapper>();
        var vm = routerMapper.Resolve(context) as TViewModelBase
                 ?? throw new InvalidOperationException($"Route '{url}' could not be resolved.");

        return vm;
    }
}