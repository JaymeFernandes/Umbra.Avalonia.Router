namespace Umbra.Router.Core.Work.Navigation;

public class QueryContext
{
    private readonly Dictionary<string, QueryValue> _values = new();
    
    public int Count => _values.Count;

    public QueryContext(string value)
    {
        Dictionary<string, string> values = new();

        var indexInit = value.IndexOf('?');
        
        if (indexInit != -1)
            value = value[(indexInit + 1)..];
        
        foreach (var pair in value.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var index = pair.IndexOf('=');

            var key = index == -1 ? pair : pair[..index];
            var val = index == -1 ? "" : pair[(index + 1)..];

            if (key.Contains('%'))
                key = Uri.UnescapeDataString(key);
            
            val = val.Replace('+', ' ');
            
            if (val.Contains('%'))
                val = Uri.UnescapeDataString(val);
            
            if (values.TryGetValue(key, out var existing))
                values[key] = existing + "," + val;
            else
                values[key] = val;
        }

        _values = values.ToDictionary(
            x => x.Key, 
            y => new QueryValue(y.Value));
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        if (_values.TryGetValue(key, out var val))
            return val.TryGetValue(out value);

        value = default;
        return false;
    }

    public bool TryGetValue(string key, out string value)
    {
        value = "";

        if (_values.TryGetValue(key, out var val))
        {
            var success = val.TryGetValue(out string? result);

            value = result is null ? "" : result;

            return success;
        }
        
        return false;
    }

    public T? GetValue<T>(string key)
    {
        if (_values.TryGetValue(key, out var val))
            return val.GetValue<T>();

        return default;
    }

    public IReadOnlyList<T> GetValues<T>(string key)
    {
        if (_values.TryGetValue(key, out var val))
            return val.GetValues<T>();
        
        return Array.Empty<T>();
    }
    
    public bool ContainsKey(string key)
        => _values.ContainsKey(key);
}

public class QueryValue
{
    private List<string> _values { get; }
    
    public QueryValue(string value)
    {
        _values = value.Split(',').Select(x => x.Trim()).ToList();
    }
    
    public IReadOnlyList<string> Values => _values;

    public bool TryGetValue<T>(out T value)
        => Convert<T>(out value);

    public T GetValue<T>()
    {
        Convert<T>(out var value);
        
        return value;
    }

    public IReadOnlyList<T> GetValues<T>()
    {
        if(typeof(T) == typeof(QueryValue))
            return new List<T>() { (T)(object)this };
        
        var values = new List<T>();
        
        foreach (var val in _values)
        {
            if(Convert(val, out T value))
                values.Add(value);
        }
        
        return values;
    }

    private bool Convert<T>(out T value)
        => Convert(_values.FirstOrDefault(), out value);

    private bool Convert<T>(string? value, out T result)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            result = default;
            return true;
        }
        
        if (typeof(T) == typeof(QueryValue))
        {
            result = (T)(object)this;
            return true;
        }
        
        if (typeof(T) == typeof(string))
        {
            result = (T)(object)value;
            return true;
        }

        if ((typeof(T) == typeof(int?) || typeof(T) == typeof(int)) && int.TryParse(value, out var i))
        {
            result = (T)(object)i;
            return true;
        }

        if ((typeof(T) == typeof(Guid?) || typeof(T) == typeof(Guid)) && Guid.TryParse(value, out var g))
        {
            result = (T)(object)g;
            return true;
        }

        if ((typeof(T) == typeof(bool?) || typeof(T) == typeof(bool)) && bool.TryParse(value, out var b))
        {
            result = (T)(object)b;
            return true;
        }

        result = default!;
        return false;
    }
}

