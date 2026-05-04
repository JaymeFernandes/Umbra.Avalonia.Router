using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Configuration;

public class RoutesBuilder
{
    private readonly ICollection<RouteBuilder> _definitions = new List<RouteBuilder>();
    public int HistorySize { get; set; } = 10;

    public RouteBuilder Register<TView, TViewModel>(string route)
        where TViewModel : class, IRoutePage
        where TView : class
    {
        var builder = new RouteBuilder(route, typeof(TView), typeof(TViewModel));

        _definitions.Add(builder);

        return builder;
    }

    public RouteBuilder Register(Type view, Type viewModel, string route)
    {
        var builder = new RouteBuilder(route, view, viewModel);

        _definitions.Add(builder);

        return builder;
    }

    public RouteMap Build()
    {
        var map = new RouteMap();

        var values = _definitions.Select(x => x.Definition);

        foreach (var def in values)
            map.Add(def);

        return map;
    }

    public ICollection<NavigationDefinition> GetAllDefinitions()
    {
        if (!_definitions.Any())
            return new List<NavigationDefinition>();

        return _definitions.Select(x => x.Definition).ToList();
    }
}

public class RouteBuilder
{
    public RouteBuilder(string route, Type view, Type viewModel)
    {
        Definition = new InternalNavigationDefinition(route, view, viewModel);
    }

    internal NavigationDefinition Definition { get; }

    public RouteBuilder SetRoute(string route)
    {
        Definition.Route = route;
        return this;
    } 

    public RouteBuilder SetTitle(string title)
    {
        Definition.Title = title;
        return this; 
    }

    public RouteBuilder AddGuard(GuardDefinition? canMatch = null, GuardDefinition? canDeactivate = null)
    {
        Definition.CanMatch ??= new List<GuardDefinition>();
        Definition.CanDeactivate ??= new List<GuardDefinition>();
        
        Definition.AddGuard(canMatch, canDeactivate);

        return this;
    }

    public RouteBuilder CanMatchGuard<T>() where T : IGuard
    {
        Definition.CanMatch ??= new List<GuardDefinition>();
        Definition.CanMatch.Add(new GuardDefinition<T>());


        return this;
    }
    
    public RouteBuilder CanMatchGuard(GuardDefinition guard)
    {
        Definition.CanMatch ??= new List<GuardDefinition>();
        Definition.CanMatch.Add(guard);


        return this;
    }
    
    public RouteBuilder CanDeactivateGuard<T>() where T : IGuard
    {
        Definition.CanDeactivate ??= new List<GuardDefinition>();
        Definition.CanDeactivate.Add(new GuardDefinition<T>());

        return this;
    }
    
    public RouteBuilder CanDeactivateGuard(GuardDefinition guard)
    {
        Definition.CanDeactivate ??= new List<GuardDefinition>();
        Definition.CanDeactivate.Add(guard);

        return this;
    }

    private class InternalNavigationDefinition : NavigationDefinition
    {
        public InternalNavigationDefinition(string route, Type view, Type viewModel)
            : base(view, viewModel)
        {
            Route = route;
        }
    }
}