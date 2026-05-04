using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Work;

public enum GuardDecision
{
    Allow,
    Deny,
    Redirect,
    Blocked
}

public class GuardResult
{
    private GuardResult(GuardDecision decision, string? redirectTo = null, NavigationBlocked? blockedData = null)
    {
        Decision = decision;
        RedirectTo = redirectTo;
        BlockedData = blockedData;
    }

    internal NavigationContext Context { get; set; }

    public GuardDecision Decision { get; }
    public string? RedirectTo { get; }
    public NavigationBlocked? BlockedData { get; }

    public static GuardResult Allow()
        => new(GuardDecision.Allow);

    public static GuardResult Deny()
        => new(GuardDecision.Deny);

    public static GuardResult Redirect(string to)
        => new(GuardDecision.Redirect, to);

    public static GuardResult Blocked(string reason, object? data = null)
        => new(GuardDecision.Blocked, null, new NavigationBlocked(reason, data));
}

public class NavigationBlocked
{
    public NavigationBlocked(string reason, object? data)
    {
        Reason = reason;
        Data = data;
    }

    public string Reason { get; }
    public object? Data { get; }
    
    
}
