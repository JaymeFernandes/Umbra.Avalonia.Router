using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Work.Trie;

public class RouteSnapshot : UriContext
{
    public RouteSnapshot(string uri) : base(uri)
    {
    }

    public RouteSnapshot(string uri, object? body) : base(uri, body)
    {
    }

    public RouteSnapshot(string[] segments) : base(segments)
    {
    }

    public RouteSnapshot(string[] segments, object? body) : base(segments, body)
    {
    }
}