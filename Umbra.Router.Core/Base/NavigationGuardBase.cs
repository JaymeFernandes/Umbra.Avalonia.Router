using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Base;

public abstract class NavigationGuardBase : IGuard
{
    public async Task<GuardResult> ExecuteGuardAsync(NavigationContext context)
    {
        var result = await GuardAsync(context);

        if (result.Decision == GuardDecision.Allow)
            await OnGuardAllow(context);
        else
            await OnGuardDeny(context);

        return result;
    }

    protected abstract Task<GuardResult> GuardAsync(NavigationContext context);
    protected virtual Task OnGuardAllow(NavigationContext context) => Task.CompletedTask;
    protected virtual Task OnGuardDeny(NavigationContext context) => Task.CompletedTask;
}