using Microsoft.Extensions.DependencyInjection;
using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Services;

public class GuardServices<T> where T : class, IRoutePage
{
    private readonly RouteMap _map;
    private readonly IServiceProvider _serviceProvider;
    private RouterConfig<T> _config;

    public GuardServices(RouterConfig<T> config, IServiceProvider serviceProvider)
    {
        _config = config;
        _serviceProvider = serviceProvider;

        _map = config.Build();
    }

    public async Task<GuardResult> CanNavigateAsync(Uri uri)
    {
        return await CanMatchAsync(new RouteSnapshot(uri.AbsolutePath));
    }

    public async Task<GuardResult> CanNavigateAsync(string uri)
    {
        return await CanMatchAsync(new RouteSnapshot(uri));
    }

    public async Task<GuardResult?> CanMatchAsync(RouteSnapshot snapshot)
    {
        var template = _map.Match(snapshot.Path);

        if (template == null)
            return null;

        var context = template.ResolveContext(snapshot);

        var definitions = template.Definition.CanMatch;

        return await CanAsync(definitions, context);
    }

    public async Task<GuardResult?> CanDeactivateAsync(RouteSnapshot snapshot)
    {
        var template = _map.Match(snapshot.Path);
        
        if(template == null)
            return null;

        var context = template.ResolveContext(snapshot);
        var definitions = template.Definition.CanDeactivate;
        
        return await CanAsync(definitions, context);
    }

    private async Task<GuardResult> CanAsync(ICollection<GuardDefinition>? definitions, NavigationContext context)
    {
        if (definitions == null)
            return GuardResult.Allow();
        
        foreach (var definition in definitions)
        {
            var guard = _serviceProvider.GetRequiredService(definition.Guard) as IGuard;
            
            if (guard is IGuard service)
            {
                var result = await service.ExecuteGuardAsync(context);

                if (result.Decision == GuardDecision.Allow)
                    continue;

                return result;
            }

            return GuardResult.Deny();
        }
        
        return GuardResult.Allow();
    }
}