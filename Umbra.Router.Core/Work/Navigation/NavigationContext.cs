using Umbra.Router.Core.Events;

namespace Umbra.Router.Core.Work.Navigation;

public class NavigationContext
{
    public NavigationContext(RouteSnapshot snapshot, Dictionary<string, string> parameters)
    {
        CurrentUrl = snapshot.Path;

        Body = snapshot.Body;
        Query = snapshot.Query;

        Parameters = new ParametersContext(parameters);
    }

    public BodyContext Body { get; private set; }

    public string CurrentUrl { get; }

    public QueryContext Query { get; private set; }
    public ParametersContext Parameters { get; private set; }

}