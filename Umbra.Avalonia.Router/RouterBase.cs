using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Umbra.Avalonia.Router.Configuration;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace Umbra.Avalonia.Router;

public partial class RouterBase<TViewModelBase> where TViewModelBase : class, IRoutePage
{
    protected readonly RouterConfig<TViewModelBase> config;

    private readonly IRouterResolver<TViewModelBase> _resolver;

    private readonly IServiceProvider _serviceProvider;

    private TViewModelBase _currentViewModel = default!;

    private Dictionary<Type, Type> _routes;

    public RouterBase(IServiceProvider serviceProvider, RouterConfig<TViewModelBase> options)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        config = options;
        _routes = options._pages;
        _resolver = serviceProvider.GetRequiredService<IRouterResolver<TViewModelBase>>();
    }

    public event Action<Control>? PageChanged;

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

    public virtual TViewModelBase Navigate(string url, object? body = null)
    {
        var vm = ResolveViewModel(url, body);
        CurrentViewModel = vm;
        return vm;
    }

    protected TViewModelBase ResolveViewModel(string url, object? body)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Route cannot be null or empty.", nameof(url));

        var vm = _resolver.Resolve(url, body) as TViewModelBase
                 ?? throw new InvalidOperationException($"Route '{url}' could not be resolved.");

        return vm;
    }

    protected TViewModelBase ResolveViewModel(NavigationContext context)
    {
        var url = context.CurrentUrl;

        if (string.IsNullOrWhiteSpace(context.CurrentUrl))
            throw new ArgumentException("Route cannot be null or empty.", nameof(url));

        var vm = _resolver.Resolve(context) as TViewModelBase
                 ?? throw new InvalidOperationException($"Route '{url}' could not be resolved.");

        return vm;
    }
}
