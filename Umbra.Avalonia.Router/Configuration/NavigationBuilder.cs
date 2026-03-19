using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Configuration;

public class NavigationBuilder
{
    public int HistorySize { get; set; } = 10;
    
    private ICollection<NavigationsDefinitionBuilder> _definitions = new List<NavigationsDefinitionBuilder>();

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
        if(!_definitions.Any())
            return new List<NavigationDefinition>();
        
        return _definitions.Select(x => x.Definition).ToList();
    }
}

public class NavigationsDefinitionBuilder
{
    
    private NavigationDefinition _definition;
    
    internal NavigationDefinition Definition => _definition;
    
    public NavigationsDefinitionBuilder(string route, Type view, Type viewModel)
    { 
        _definition = new InternalNavigationDefinition(route, view, viewModel);
    }

    public void SetRoute(string route)
        => _definition.Route = route;

    public void AddGuard(GuardDefinition guard)
    {
        _definition.Guards ??= new List<GuardDefinition>();
        _definition.AddGuard(guard);
    }
    
    public void AddGuard<T>() where T : IGuard
    {
        _definition.Guards ??= new List<GuardDefinition>();
        _definition.Guards.Add(new GuardDefinition<T>());
    }
    
    class InternalNavigationDefinition : NavigationDefinition
    {
        public InternalNavigationDefinition(string route, Type view, Type viewModel) 
            : base(view, viewModel)
        {
            Route = route;
        }
    }
}