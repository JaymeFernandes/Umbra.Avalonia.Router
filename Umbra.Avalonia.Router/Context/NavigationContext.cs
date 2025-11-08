using System.Collections.Concurrent;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Umbra.Avalonia.Router.Context;

public class NavigationContext
{
    private Uri _currentUri;
    
    public string CurrentUrl
    {
        get => _currentUri.ToString();
    }

    internal string Key
    {
        get;
        private set;
    }
    
    public BodyContext<object> Body { get; private set; }
    public QueryContext Query { get; private set; }


    public NavigationContext(string url, object? body, string defaultScheme, string appName)
    {
        _currentUri = NormalizeRoute(url, defaultScheme, appName);
        Key = GenerateKey(url, defaultScheme, appName);
        
        Body = new BodyContext<object>(body);
        Query = new QueryContext(_currentUri);
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
}

public class BodyContext<T>
{
    public T? Value { get; private set; }

    public BodyContext(T? value)
    {
        Value = value;
    }

    public bool TryGetValue(out T? value)
    {
        var isEmpty = Value is null;
        
        value = isEmpty ? default : Value;

        return !isEmpty;
    }
}
    
public class QueryContext
{
    private readonly ConcurrentDictionary<string, StringValues> _query;
    
    public QueryContext(Uri uri)
    {
        var parsed = QueryHelpers.ParseQuery(uri.Query);
        _query = new ConcurrentDictionary<string, StringValues>(parsed);
    }
    
    public string GetValue(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        return _query.TryGetValue(key, out var value)
            ? value.ToString()
            : string.Empty;
    }
    
    public bool TryGetValue(string key, out string value)
    {
        value = "";
        
        if (string.IsNullOrWhiteSpace(key))
            return false;
        
        var response = _query.TryGetValue(key, out var result);

        value = response ? 
            result.ToString() : string.Empty;
        
        return response;
    }
    
    public List<string> GetValues(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return new List<string>();

        return _query.TryGetValue(key, out var value)
            ? value.ToList()!
            : new List<string>();
    }
    
    public int GetValueNumber(string key) =>
        int.TryParse(GetValue(key), out var n) ? n : 0;

    public bool TryGetValueNumber(string key, out int num)
    {
        var response = TryGetValue(key, out var value);
        
        num = int.TryParse(value, out var n) ? n : 0;
        
        return response;
    }
    
    public List<int> GetValuesNumber(string key) =>
        GetValues(key).Select(v => int.TryParse(v, out var n) ? n : 0).ToList();
    
    public bool GetValueBool(string key) =>
        bool.TryParse(GetValue(key), out var b) && b;

    public List<bool> GetValuesBool(string key) =>
        GetValues(key).Select(v => bool.TryParse(v, out var b) && b).ToList();
    
    public bool Contains(string key)
        => _query.ContainsKey(key);
}

