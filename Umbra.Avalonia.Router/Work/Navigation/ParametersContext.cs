using System.Collections.Concurrent;

namespace Umbra.Router.Core.Work.Navigation;

public class ParametersContext
{
    private readonly ConcurrentDictionary<string, string> _parameters;

    public ParametersContext(Dictionary<string, string> parameters)
    {
        _parameters = new ConcurrentDictionary<string, string>(parameters);
    }

    public bool Contains(string key)
    {
        return _parameters.ContainsKey(key);
    }

    public bool Any()
    {
        return _parameters.Any();
    }


    public bool TryGetValue(string key, out string? value)
    {
        value = "";

        if (string.IsNullOrWhiteSpace(key))
            return false;

        var response = _parameters.TryGetValue($":{key}", out var result);

        value = result;

        return response;
    }
}