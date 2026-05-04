namespace Umbra.Router.Core.Work.Navigation;

public class RouteSnapshot
{
    public RouteSnapshot(string uri)
    {
        var schemeIndex = uri.IndexOf("://");

        if (schemeIndex != -1)
        {
            uri = uri[(schemeIndex + 3)..];

            var firstSlash = uri.IndexOf('/');
            uri = firstSlash == -1 ? "/" : uri[firstSlash..];
        }

        Raw = uri;

        var fragmentIndex = uri.IndexOf('#');

        if (fragmentIndex != -1)
            uri = uri[..fragmentIndex];

        var queryIndex = uri.IndexOf('?');

        string path;
        string? query = null;

        if (queryIndex != -1)
        {
            path = uri[..queryIndex];
            query = uri[(queryIndex + 1)..];
        }
        else
        {
            path = uri;
        }

        Path = path;

        Segments = path
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(DecodeSegment)
            .ToArray();

        if (!string.IsNullOrEmpty(query))
            Query = new QueryContext(query);
    }

    public RouteSnapshot(string uri, object? body) : this(uri)
    {
        Body = new BodyContext(body);
    }

    public RouteSnapshot(string[] segments) : this(string.Join('/', segments))
    {
    }

    public RouteSnapshot(string[] segments, object? body) : this(string.Join('/', segments))
    {
        Body = new BodyContext(body);
    }

    public string Raw { get; }

    public string Path { get; }

    public string[] Segments { get; }

    public QueryContext? Query { get; }
    public BodyContext? Body { get; }

    private static string DecodeSegment(string value)
    {
        return value.IndexOf('%') >= 0
            ? Uri.UnescapeDataString(value)
            : value;
    }
}