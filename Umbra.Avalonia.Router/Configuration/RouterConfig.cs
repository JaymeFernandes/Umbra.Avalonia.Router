using Umbra.Router.Core.Interfaces;

namespace Umbra.Router.Core.Configuration;

public class RouterConfig<T> : NavigationBuilder where T : class, IRoutePage
{
}