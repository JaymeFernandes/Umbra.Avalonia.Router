namespace Umbra.Router.Core.Work;

public class GuardResult
{
    private GuardResult(GuardAction guardAction)
    {
        Action = guardAction;
    }

    public GuardAction Action { get; }

    public static GuardResult Allow()
    {
        return new GuardResult(GuardAction.Allow);
    }

    public static GuardResult Cancel()
    {
        return new GuardResult(GuardAction.Cancel);
    }
}

public enum GuardAction
{
    Allow,
    Cancel
}