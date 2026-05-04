using Umbra.Router.Core.Interfaces;

namespace Umbra.Router.Core.Configuration;

public class RouterConfig<T> : RoutesBuilder where T : class, IRoutePage
{
    
}