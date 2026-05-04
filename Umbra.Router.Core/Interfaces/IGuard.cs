using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Interfaces;

public interface IGuard
{
    public Task<GuardResult> ExecuteGuardAsync(NavigationContext context);
}