using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Umbra.Router.Core.Configuration;

public class RoutesAngularStyle  : IEnumerable<RouteAngularStyle>, ICollection<RouteAngularStyle>
{
    private ICollection<RouteAngularStyle> _routes = new List<RouteAngularStyle>();
    public IEnumerator<RouteAngularStyle> GetEnumerator()
        => _routes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _routes.GetEnumerator();

    public void Add(RouteAngularStyle item)
    {
        if(item == null)
            throw new ArgumentNullException(nameof(item));
        
        if(item.Path.Contains('?'))
            throw new InvalidOperationException("Cannot add a route without a query parameter.");
        
        if(_routes.Any(x => x.Path.TrimStart('/').TrimEnd('/').Equals(item.Path, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Route '{item.Path}' already exists");
        
        _routes.Add(item);
    }

    public void Clear()
        => _routes.Clear();

    public bool Contains(RouteAngularStyle item)
        => _routes.Contains(item);

    public void CopyTo(RouteAngularStyle[] array, int arrayIndex)
        => _routes.CopyTo(array, arrayIndex);

    public bool Remove(RouteAngularStyle item)
        => _routes.Remove(item);

    public int Count => _routes.Count;
    public bool IsReadOnly => _routes.IsReadOnly;
}

public class RouteAngularStyle
{
    [Required] public string? Path { get; set; }
    
    public Type? Component { get; set; }
    public Type? ViewModel { get; set; }
    
    internal bool IsEndPoint => Component is not null && ViewModel is not null;
    
    public GuardDefinition[]? CanMatch { get; set; }
    public GuardDefinition[]? CanDeactivate { get; set; }
    
    public string? RedirectTo { get; set; }
    public string? Title { get; set; }
    
    public List<RouteAngularStyle>? Children { get; set; }
}