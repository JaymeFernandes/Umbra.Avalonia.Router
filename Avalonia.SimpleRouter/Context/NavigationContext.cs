using System.Collections.Concurrent;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Avalonia.SimpleRouter.Context;

public class NavigationContext
{
    public string CurrentUrl { get; private set;  }
    
    public BodyContext<object> Body { get; private set; }
    public QueryContext Query { get; private set; }


    public NavigationContext(object body, string urlNormalize, string url)
    {
        Body = new BodyContext<object>(body);
        CurrentUrl = url;
        Query = new QueryContext(urlNormalize);
    }
}

public class BodyContext<T>
{
    public T? Value { get; private set; }
        
    public bool IsEmpty => Value is null;

    public BodyContext(T? value)
    {
        Value = value;
    }
}
    
public class QueryContext
{
    private readonly ConcurrentDictionary<string, StringValues> _query;
    
    public QueryContext(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("A URL não pode ser nula ou vazia.", nameof(url));
        
        var uri = new Uri(url);
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
            ? value.ToList()
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