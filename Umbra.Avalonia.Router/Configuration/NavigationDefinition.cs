using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Configuration;

public abstract class NavigationDefinition
{
    private UriContext _route;
    private string _key;

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

    public string Scheme = "app";
    public string Name = "myapp";
    
    public readonly Type View;
    public readonly Type ViewModel;
    
    public ICollection<GuardDefinition>? Guards { get; set; }

    public NavigationDefinition AddGuard(GuardDefinition guard)
    {
        if (Guards == null)
            Guards = new List<GuardDefinition>();
        
        Guards.Add(guard);

        return this;
    }

    protected NavigationDefinition(Type view, Type viewModel)
    {
        Route = "";
        View = view;
        ViewModel = viewModel;
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