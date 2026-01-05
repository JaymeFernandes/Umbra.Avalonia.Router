using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;

namespace Umbra.Avalonia.Router.Context;

public class QueryContext
{
    private readonly ConcurrentDictionary<string, StringValues> _query;

    public QueryContext(Uri uri)
    {
        var parsed = QueryHelpers.ParseQuery(uri.Query);
        _query = new ConcurrentDictionary<string, StringValues>(parsed);
    }

    public bool Contains(string key)
        => _query.ContainsKey(key);

    public string GetValue(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        return _query.TryGetValue(key, out var value)
            ? value.ToString()
            : string.Empty;
    }

    public bool GetValueBool(string key) =>
        bool.TryParse(GetValue(key), out var b) && b;

    public int GetValueNumber(string key) =>
        int.TryParse(GetValue(key), out var n) ? n : 0;

    public List<string> GetValues(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return new List<string>();

        return _query.TryGetValue(key, out var value)
            ? value.ToList()!
            : new List<string>();
    }

    public List<bool> GetValuesBool(string key) =>
        GetValues(key).Select(v => bool.TryParse(v, out var b) && b).ToList();

    public List<int> GetValuesNumber(string key) =>
        GetValues(key).Select(v => int.TryParse(v, out var n) ? n : 0).ToList();

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

    public bool TryGetValueNumber(string key, out int num)
    {
        var response = TryGetValue(key, out var value);

        num = int.TryParse(value, out var n) ? n : 0;

        return response;
    }
}
