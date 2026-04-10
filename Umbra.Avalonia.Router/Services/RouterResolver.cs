using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Services;

public class RouterResolver<T> : IRouterResolver<T>
    where T : class, IRoutePage
{
    private readonly RouteMap _map;

    private readonly IServiceProvider _serviceProvider;
    private readonly string _uri404;

    private RouterConfig<T> _config;

    public RouterResolver(RouterConfig<T> config, IServiceProvider serviceProvider)
    {
        _map = config.Build();

        _serviceProvider = serviceProvider;

        _config = config;
    }

    public RouterResult Resolve(RouteSnapshot snapshot)
    {
        var template = _map.Match(snapshot.Path);

        if (template == null)
            throw new NotSupportedException($"Route '{snapshot.Path}' is not supported.");

        var context = template.ResolveContext(snapshot);

        return ResolveIRoutePage(template.Definition, context);
    }

    public RouterResult Resolve(string route, object body)
    {
        return Resolve(new RouteSnapshot(route, body));
    }

    private RouterResult ResolveIRoutePage(NavigationDefinition definition, NavigationContext context)
    {
        if (!typeof(IRoutePage).IsAssignableFrom(definition.ViewModel))
            throw new InvalidOperationException(
                $"The type {definition.ViewModel.Name} does not implement IRoutePage.");

        var vm = _serviceProvider.GetService(definition.ViewModel) as IRoutePage;

        if (vm == null)
            throw new InvalidOperationException(
                $"The type {definition.ViewModel.Name} is not registered in the container.");

        _ = Task.Run(async () => await vm.InitializeAsync(context));

        return new RouterResult(definition.View, vm, context);
    }
}

public class RouterResult
{
    public NavigationContext Context;
    public Type View;
    public IRoutePage ViewModel;

    public RouterResult(Type view, IRoutePage viewModel, NavigationContext context)
    {
        View = view;
        ViewModel = viewModel;
        Context = context;
    }
}