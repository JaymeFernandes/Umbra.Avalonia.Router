using Umbra.Router.Core.Services;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Core.Interfaces;

public interface IRouterResolver<T> where T : class, IRoutePage
{
    public RouterResult Resolve(string route, object body);

    public RouterResult Resolve(RouteSnapshot context);
}
