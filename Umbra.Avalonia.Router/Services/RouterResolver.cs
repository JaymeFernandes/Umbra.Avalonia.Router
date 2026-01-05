using rm.Trie;
using Umbra.Avalonia.Router.Configuration;
using Umbra.Avalonia.Router.Context;
using Umbra.Avalonia.Router.Interfaces;

namespace Umbra.Avalonia.Router.Services;

public class RouterResolver<T> : IRouterResolver<T>
   where T : class, IRoutePage
{
    private readonly string _uri404;

    private RouterConfig<T> _config;

    private IServiceProvider _serviceProvider;

    private TrieMap<Type> _trie = new();

    public RouterResolver(RouterConfig<T> config, IServiceProvider serviceProvider)
    {
        foreach (var page in config._routes)
            _trie.Add(page.Key, page.Value);

        _serviceProvider = serviceProvider;

        _config = config;

        _uri404 = new NavigationContext("404", null, _config.Scheme, _config.AppName).Key;
    }

    public IRoutePage Resolve(string route, object body)
    {
        var context = new NavigationContext(route, body, _config.Scheme, _config.AppName);
        var key = context.Key;

        return Resolve(context);
    }

    public IRoutePage Resolve(NavigationContext context)
    {
        var node = _trie.ValueBy(context.Key);

        if (node != null)
            return ResolveIRoutePage(type: node, context);

        var node404 = _trie.ValueBy(_uri404);

        if (node404 != null)
            return ResolveIRoutePage(type: node404, context);

        throw new NotSupportedException($"Route '{context.CurrentUrl}' is not supported.");
    }

    private IRoutePage ResolveIRoutePage(Type type, NavigationContext context)
    {
        if (!typeof(IRoutePage).IsAssignableFrom(type))
            throw new InvalidOperationException(
                $"The type {type.Name} does not implement IRoutePage.");

        var vm = _serviceProvider.GetService(type) as IRoutePage;

        if (vm == null)
            throw new InvalidOperationException($"The type {type.Name} is not registered in the container.");

        _ = Task.Run(
            async () => await vm.InitializeAsync(context));

        return vm;
    }
}
