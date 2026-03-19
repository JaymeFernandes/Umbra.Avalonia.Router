using Microsoft.Extensions.DependencyInjection;
using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Services;

public class GuardServices<T> where T : class, IRoutePage
{
    private RouterConfig<T> _config;
    private IServiceProvider _serviceProvider;
    private RouteMap _map;

    public GuardServices(RouterConfig<T> config, IServiceProvider serviceProvider)
    {
        _config = config;
        _serviceProvider = serviceProvider;

        _map = config.Build();
    }

    public async Task<GuardResult> CanNavigateAsync(Uri uri)
        => await CanNavigateAsync(new RouteSnapshot(uri.AbsolutePath));
    
    public async Task<GuardResult> CanNavigateAsync(string uri)
        =>  await CanNavigateAsync(new RouteSnapshot(uri));
    
    public async Task<GuardResult> CanNavigateAsync(RouteSnapshot snapshot)
    {
        var template = _map.Match(snapshot.Path);
        
        if(template == null)
            return GuardResult.Cancel();

        var context = template.ResolveContext(snapshot);
        
        var definitions = template.Definition.Guards;

        if (definitions == null)
            return GuardResult.Allow();
        
        foreach (var definition in definitions)
        {
            var guard = _serviceProvider.GetRequiredService(definition.Guard) as IGuard;

            if (guard is IGuard service)
            {
                var result = await service.ExecuteGuardAsync(context);
                
                if(result.Action == GuardAction.Allow)
                    continue;
            
                return result;
            }

            return GuardResult.Cancel();
        }
        
        return GuardResult.Allow();
    }
}