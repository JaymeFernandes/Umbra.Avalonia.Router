using Umbra.Router.Core.Interfaces;
using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Base;

public abstract class NavigationGuardBase : IGuard
{
    public async Task<GuardResult> ExecuteGuardAsync(NavigationContext context)
    {
        var result = await CanNavigateAsync(context);

        switch (result.Action)
        {
            case GuardAction.Allow:
                await OnGuardPassed(context);
                break;

            case GuardAction.Cancel:
                await OnGuardFailed(context);
                break;
        }

        return result;
    }

    protected abstract Task<GuardResult> CanNavigateAsync(NavigationContext context);

    protected virtual Task OnGuardPassed(NavigationContext context)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnGuardFailed(NavigationContext context)
    {
        return Task.CompletedTask;
    }


    protected GuardResult Allow()
    {
        return GuardResult.Allow();
    }

    protected GuardResult Cancel()
    {
        return GuardResult.Cancel();
    }
}