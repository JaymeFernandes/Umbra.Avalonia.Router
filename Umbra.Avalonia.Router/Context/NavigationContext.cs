using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace Umbra.Avalonia.Router.Context;

public class NavigationContext
{
    private Uri _currentUri;

    public NavigationContext(string url, object? body, string defaultScheme, string appName)
    {
        _currentUri = NormalizeRoute(url, defaultScheme, appName);
        Key = GenerateKey(_currentUri.ToString(), defaultScheme, appName);

        Body = new BodyContext(body);
        Query = new QueryContext(_currentUri);
    }

    public BodyContext Body { get; private set; }

    public string CurrentUrl
    {
        get => _currentUri.ToString();
    }

    public QueryContext Query { get; private set; }

    internal string Key
    {
        get;
        private set;
    }

    private string GenerateKey(string route, string defaultScheme, string appName)
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("Route cannot be null or empty.", nameof(route));

        Uri? uri;

        if (Uri.TryCreate(route, UriKind.Absolute, out uri))
        {
            var path = uri.AbsolutePath.TrimEnd('/');
            var host = string.IsNullOrWhiteSpace(uri.Host) ? appName : uri.Host;
            var scheme = string.IsNullOrWhiteSpace(uri.Scheme) ? defaultScheme : uri.Scheme;

            return $"{scheme}://{host}{path}";
        }

        var normalizedPath = route.TrimStart('/').TrimEnd('/');
        return $"{defaultScheme}://{appName}/{normalizedPath}".ToLower();
    }

    private Uri NormalizeRoute(string route, string defaultScheme, string appName)
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ArgumentException("Route cannot be null or empty.", nameof(route));

        Uri? uri;

        if (Uri.TryCreate(route, UriKind.Absolute, out uri))
            return uri;

        return new Uri($"{defaultScheme}://{appName}/{route.TrimStart('/').TrimEnd('/')}");
    }
}
