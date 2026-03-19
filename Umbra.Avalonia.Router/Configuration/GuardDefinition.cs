using Umbra.Router.Core.Interfaces;

namespace Umbra.Router.Core.Configuration;

public abstract class GuardDefinition
{
    public readonly Type Guard;
    
    public GuardType GuardType = GuardType.CanActivate;

    protected GuardDefinition(Type guard)
        => Guard = guard;
}
    
public class GuardDefinition<TGuard> : GuardDefinition where TGuard : IGuard 
{
    public GuardDefinition() : base(typeof(TGuard)) { }
    
    public GuardDefinition(GuardType type) : base(typeof(TGuard))
        => GuardType = type;
}

public enum GuardType
{
    CanActivate,
    CanActivateChild,
    CanDeactivate
}