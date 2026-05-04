using Umbra.Router.Core.Configuration;

namespace Umbra.Router.Core.Extensions;

public static class RouterBuilderExtensions
{
    public static void UseAngularStyleRoutes(this RoutesBuilder route, RoutesAngularStyle config)
    {
        var stack = new Stack<(RouteAngularStyle node, string parentPath)>();

        foreach (var item in config)
            stack.Push((item, ""));

        while (stack.Count > 0)
        {
            var (node, parent) = stack.Pop();
            
            var fullPath = Combine(parent, node.Path ?? "");

            if (node.IsEndPoint)
            {
                var builder = route.Register(node.Component!, node.ViewModel!, fullPath);
                
                if(node.CanMatch != null)
                    foreach (var guard in node.CanMatch)
                        builder.CanMatchGuard(guard);
                
                if(node.CanDeactivate != null)
                    foreach (var guard in node.CanDeactivate)
                        builder.CanDeactivateGuard(guard);

                if (!string.IsNullOrWhiteSpace(node.Title))
                    builder.SetTitle(node.Title);
            }
            
            if(node.Children != null)
                for (int i = node.Children.Count - 1; i >= 0; i--)
                    stack.Push((node.Children.ElementAt(i), fullPath));
        }
    }
    
    private static string Combine(string parent, string child)
    {
        if (string.IsNullOrWhiteSpace(parent)) return child ?? "";
        if (string.IsNullOrWhiteSpace(child)) return parent;

        return $"{parent}/{child}";
    }
}