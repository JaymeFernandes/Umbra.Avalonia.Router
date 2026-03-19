using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Work.Trie;

public class RouteMap
{
    private readonly IList<State> _states = new List<State>()
    {
        new State() { Id = 0 }
    };
    
    public RouteTemplate? Match(string key)
    {
        var parts = key.Split('/', StringSplitOptions.RemoveEmptyEntries);

        int state = 0;
        int catchAll = -1;

        foreach (var part in parts)
        {
            var current = _states[state];
            
            if(current.CatchAll != -1)
                catchAll = current.CatchAll;

            if (current.Literals.TryGetValue(part, out var next))
            {
                state = next;
                continue;
            }

            if (current.Param != -1)
            {
                state = current.Param;
                continue;
            }

            break;
        }

        return _states[state].IsEndpoint ? 
            _states[state].Template : catchAll != -1 ? 
                _states[catchAll].Template : null;
    }

    public RouteTemplate? Match(Uri uri)
        => Match(uri.AbsolutePath);

    public RouteTemplate? Match(UriContext uriContext)
        => Match(uriContext.Path);

    public RouteTemplate? Match(string[] route)
        => Match(string.Join('/', route));

    public void Add(RouteTemplate template)
    {
        int state = 0;

        foreach (var part in template.Segments)
        {
            var current = _states[state];

            if (part is ParameterSegment)
            {
                if (current.Param == -1)
                {
                    current.Param = _states.Count;
                    _states.Add(new State { Id = current.Param });
                }
                
                state = current.Param;
            }
            else if(part is LiteralSegment)
            {
                if (!current.Literals.TryGetValue(part.Key, out var next))
                {
                    next = _states.Count;
                    current.Literals.Add(part.Key, next);
                    _states.Add(new State { Id = next });
                }

                state = next;
            }
            else if (part is CatchAllSegment)
            {
                if (current.CatchAll == -1)
                {
                    current.CatchAll = _states.Count;
                    _states.Add(new State { Id = current.CatchAll });
                    
                    break;
                }
            }
        }
        
        var value = _states[state];
        
        value.IsEndpoint = true;
        value.Template = template;
    }

    public void Add(NavigationDefinition definition)
        => Add(new RouteTemplate(definition));

    class State
    {
        public int Id;
        public Dictionary<string, int> Literals = new();
    
        public int Param = -1;
        public int CatchAll = -1;
    
        public bool IsEndpoint;
    
        public RouteTemplate Template;
    }
}