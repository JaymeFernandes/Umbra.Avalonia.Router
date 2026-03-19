namespace Umbra.Router.Core.Work;

public class GuardResult
{
    public GuardAction Action { get; }
    
    private GuardResult(GuardAction guardAction)
    {
        Action = guardAction;
    }
    
    public static GuardResult Allow()
        => new GuardResult(GuardAction.Allow);

    public static GuardResult Cancel()
        => new GuardResult(GuardAction.Cancel);
}


public enum GuardAction
{
    Allow,
    Cancel
}