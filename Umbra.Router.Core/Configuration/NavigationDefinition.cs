using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Configuration;

public abstract class NavigationDefinition
{
    public readonly Type View;
    public readonly Type ViewModel;
    private string _key;
    private RouteSnapshot _route;
    
    protected NavigationDefinition(Type view, Type viewModel)
    {
        Route = "";
        View = view;
        ViewModel = viewModel;
    }

    public string Route
    {
        get => _key;
        set
        {
            var uri = new RouteSnapshot(value);

            _route = uri;
            _key = uri.Path;
        }
    }

    public string Title { get; set; }
    
    public ICollection<GuardDefinition>? CanMatch { get; set; }
    public ICollection<GuardDefinition>? CanDeactivate { get; set; }

    public NavigationDefinition AddGuard(GuardDefinition? canMatch = null, GuardDefinition? canDeactivate = null)
    {
        if (CanMatch == null)
            CanMatch = new List<GuardDefinition>();

        if(CanDeactivate == null)
            CanDeactivate = new List<GuardDefinition>();
        
        if(canMatch != null)
            CanMatch.Add(canMatch);
        
        if (canDeactivate != null)
            CanDeactivate.Add(canDeactivate);

        return this;
    }
}

public class NavigationDefinition<TView, TViewModel> : NavigationDefinition
    where TViewModel : class, IRoutePage
    where TView : class
{
    public NavigationDefinition() : base(typeof(TView), typeof(TViewModel))
    {
    }

    public NavigationDefinition(string route) : base(typeof(TView), typeof(TViewModel))
    {
        Route = route;
    }
}