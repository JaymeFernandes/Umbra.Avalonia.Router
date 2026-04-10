using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Configuration;

public class NavigationBuilder
{
    private readonly ICollection<NavigationsDefinitionBuilder> _definitions = new List<NavigationsDefinitionBuilder>();
    public int HistorySize { get; set; } = 10;

    public NavigationsDefinitionBuilder Register<TView, TViewModel>(string route)
        where TViewModel : class, IRoutePage
        where TView : class
    {
        var builder = new NavigationsDefinitionBuilder(route, typeof(TView), typeof(TViewModel));

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

public class NavigationsDefinitionBuilder
{
    public NavigationsDefinitionBuilder(string route, Type view, Type viewModel)
    {
        Definition = new InternalNavigationDefinition(route, view, viewModel);
    }

    internal NavigationDefinition Definition { get; }

    public void SetRoute(string route)
    {
        Definition.Route = route;
    }

    public void AddGuard(GuardDefinition guard)
    {
        Definition.Guards ??= new List<GuardDefinition>();
        Definition.AddGuard(guard);
    }

    public void AddGuard<T>() where T : IGuard
    {
        Definition.Guards ??= new List<GuardDefinition>();
        Definition.Guards.Add(new GuardDefinition<T>());
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