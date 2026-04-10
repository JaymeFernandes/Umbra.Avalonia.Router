using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Configuration;

public abstract class NavigationDefinition
{
    public readonly Type View;
    public readonly Type ViewModel;
    private string _key;
    private UriContext _route;
    public string Name = "myapp";

    public string Scheme = "app";

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

    public ICollection<GuardDefinition>? Guards { get; set; }

    public NavigationDefinition AddGuard(GuardDefinition guard)
    {
        if (Guards == null)
            Guards = new List<GuardDefinition>();

        Guards.Add(guard);

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