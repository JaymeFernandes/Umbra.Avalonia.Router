using Umbra.Avalonia.Router.Configuration;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace Umbra.Avalonia.Router.Services;

public class RouterMapper
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _routes;
    private readonly RouterConfig _config;

    public RouterMapper(IServiceProvider serviceProvider, RouterConfig routerConfig)
    {
        _serviceProvider = serviceProvider;
        _routes = routerConfig._routes;
        _config = routerConfig;
    }

    public void Register<TViewModel>(string route) where TViewModel : class, IRoutePage
    {
        var context = new NavigationContext(route, null, _config.Scheme, _config.AppName);
        
        _routes[context.CurrentUrl] = typeof(TViewModel);
    }

    public IRoutePage Resolve(string route, object body)
    {
        var context = new NavigationContext(route, body, _config.Scheme, _config.AppName);

        if (_routes.TryGetValue(context.Key, out var type))
        {
            var vm = _serviceProvider.GetService(type) as IRoutePage;
            
            if (vm == null)
                throw new InvalidOperationException($"The type {type.Name} is not registered in the container.");
            
            vm.OnNavigatedTo(context);
            Task.Run(async () => await vm.OnNavigatedToAsync(context));
            
            Console.WriteLine(context.CurrentUrl);

            return vm;
        }

        throw new NotSupportedException($"Route '{route}' is not supported.");
    }
    
    public IRoutePage Resolve(NavigationContext navigationContext)
    {
        var route = navigationContext.Key;
        
        if (_routes.TryGetValue(route, out var type))
        {
            var vm = _serviceProvider.GetService(type) as IRoutePage;
            if (vm == null)
                throw new InvalidOperationException($"The type {type.Name} is not registered in the container.");
            
            vm.OnNavigatedTo(navigationContext);
            Task.Run(async () => await vm.OnNavigatedToAsync(navigationContext));
            
            Console.WriteLine(navigationContext.CurrentUrl);

            return vm;
        }

        throw new NotSupportedException($"Route '{route}' is not supported.");
    }
}