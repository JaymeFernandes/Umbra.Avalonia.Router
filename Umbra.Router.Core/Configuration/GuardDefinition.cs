using Umbra.Router.Core.Interfaces;

namespace Umbra.Router.Core.Configuration;

public abstract class GuardDefinition
{
    public readonly Type Guard;

    protected GuardDefinition(Type guard)
    {
        Guard = guard;
    }
}

public class GuardDefinition<TGuard> : GuardDefinition where TGuard : IGuard
{
    public GuardDefinition() : base(typeof(TGuard))
    {
    }
}

