using Umbra.Avalonia.Router.Configuration;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace Umbra.Avalonia.Router.Services;

public class RouterMapper
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _routes;
    private readonly RouterConfig _config;
    private readonly string _uri404;

    public RouterMapper(IServiceProvider serviceProvider, RouterConfig routerConfig)
    {
        _serviceProvider = serviceProvider;
        _routes = routerConfig._routes;
        _config = routerConfig;
        _uri404 = new NavigationContext("404", null, _config.Scheme, _config.AppName).Key;
    }

    public IRoutePage Resolve(string route, object body)
    {
        var context = new NavigationContext(route, body, _config.Scheme, _config.AppName);

        if (_routes.TryGetValue(context.Key, out var type))
            return ResolveIRoutePage(type, context);
        else if (_routes.TryGetValue(_uri404, out type))
            return ResolveIRoutePage(type, context);
        
        throw new NotSupportedException($"Route '{route}' is not supported.");
    }
    
    public IRoutePage Resolve(NavigationContext navigationContext)
    {
        var route = navigationContext.Key;

        if (_routes.TryGetValue(route, out var type))
            return ResolveIRoutePage(type, navigationContext);
        else if (_routes.TryGetValue(_uri404, out type))
            return ResolveIRoutePage(type, navigationContext);

        throw new NotSupportedException($"Route '{route}' is not supported.");
    }

    private IRoutePage ResolveIRoutePage(Type type, NavigationContext context)
    {
        var vm = _serviceProvider.GetService(type) as IRoutePage;
        
        if (vm == null)
            throw new InvalidOperationException($"The type {type.Name} is not registered in the container.");
            
        vm.OnNavigatedTo(context);
        Task.Run(async () => await vm.OnNavigatedToAsync(context));

        return vm;
    }
}