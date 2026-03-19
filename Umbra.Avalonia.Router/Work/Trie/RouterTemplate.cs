using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Work.Trie;

public class RouteTemplate
{
    public string Key { get; private set; }
    public List<Segment> Segments { get; } = new List<Segment>();

    public readonly NavigationDefinition Definition;

    public RouteTemplate(NavigationDefinition definition)
    {
        Key = definition.Route;
        Definition = definition;
        
        ReadOnlySpan<char> span = Key.AsSpan();

        while (span.Length > 0)
        {
            int index = span.IndexOf('/');

            if (index == -1)
                index = span.Length;
            
            var part = span.Slice(0, index);

            if (part.Length > 0)
            {
                var segment = ParseSegment(part);
                Segments.Add(segment);
            }
            
            span = index < span.Length ? span.Slice(index + 1) : ReadOnlySpan<char>.Empty;
        }
    }

    public NavigationContext ResolveContext(UriContext snapshot)
    {
        var parameters = new Dictionary<string, string>();
        
        var parts = snapshot.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
           
        for (int i = 0; i < Segments.Count; i++)
        {
            if (Segments[i] is ParameterSegment parameter)
                parameters[parameter.Key] = parts[i];
        }

        return new NavigationContext(snapshot, parameters);
    }
    
    private Segment ParseSegment(ReadOnlySpan<char> value)
    {
        if (value.SequenceEqual("**"))
            return new CatchAllSegment();
        
        if(value[0] == ':')
            return new ParameterSegment(value.ToString());
        
        return new LiteralSegment(value.ToString());
    }
}