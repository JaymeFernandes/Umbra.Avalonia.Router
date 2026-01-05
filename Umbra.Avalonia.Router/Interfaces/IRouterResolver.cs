using Umbra.Avalonia.Router.Context;

namespace Umbra.Avalonia.Router.Interfaces;

public interface IRouterResolver<T> where T : class, IRoutePage
{
    public IRoutePage Resolve(string route, object body);

    public IRoutePage Resolve(NavigationContext context);
}
