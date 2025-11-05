using Avalonia.SimpleRouter.Configuration;
using Avalonia.SimpleRouter.Context;
using Avalonia.SimpleRouter.Interfaces;

namespace Avalonia.SimpleRouter.Services;

public class RouterMapper
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _routes;

    public RouterMapper(IServiceProvider serviceProvider, RouterConfig routerConfig)
    {
        _serviceProvider = serviceProvider;
        _routes = routerConfig.GetMapRoute();
    }

    public void Register<TViewModel>(string route) where TViewModel : class, IRoutePage
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("A URL não pode ser nula ou vazia.", nameof(route));

        var normalizedUrl = route.StartsWith("app://", StringComparison.OrdinalIgnoreCase)
            ? route
            : "app://" + route;

        var uri = new Uri(normalizedUrl);

        var key = $"{uri.Host}{uri.AbsolutePath}";
        
        _routes[key] = typeof(TViewModel);
    }

    public IRoutePage Resolve(string route, object body)
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("A URL não pode ser nula ou vazia.", nameof(route));

        var normalizedUrl = route.StartsWith("app://", StringComparison.OrdinalIgnoreCase)
            ? route
            : "app://" + route;

        var uri = new Uri(normalizedUrl);
        var key = $"{uri.Host}{uri.AbsolutePath}";

        if (_routes.TryGetValue(key, out var type))
        {
            var vm = _serviceProvider.GetService(type) as IRoutePage;
            if (vm == null)
                throw new InvalidOperationException($"O tipo {type.Name} não está registrado no container.");

            var context = new NavigationContext(body, normalizedUrl, route);
            
            vm.OnNavigatedTo(context);
            Task.Run(async () => await vm.OnNavigatedToAsync(context));

            return vm;
        }

        throw new NotSupportedException($"Route '{route}' is not supported.");
    }
    
    public IRoutePage Resolve(NavigationContext navigationContext)
    {
        var route = navigationContext.CurrentUrl;
        
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("A URL não pode ser nula ou vazia.", nameof(route));

        var normalizedUrl = route.StartsWith("app://", StringComparison.OrdinalIgnoreCase)
            ? route
            : "app://" + route;

        var uri = new Uri(normalizedUrl);
        var key = $"{uri.Host}{uri.AbsolutePath}";

        if (_routes.TryGetValue(key, out var type))
        {
            var vm = _serviceProvider.GetService(type) as IRoutePage;
            if (vm == null)
                throw new InvalidOperationException($"O tipo {type.Name} não está registrado no container.");
            
            vm.OnNavigatedTo(navigationContext);
            Task.Run(async () => await vm.OnNavigatedToAsync(navigationContext));

            return vm;
        }

        throw new NotSupportedException($"Route '{route}' is not supported.");
    }
}