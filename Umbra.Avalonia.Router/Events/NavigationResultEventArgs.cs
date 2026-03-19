using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Core.Events;

public class NavigationResultEventArgs<T> : EventArgs where T : class
{
    public T Page { get; }
    
    public NavigationContext Context { get; }
    
    public bool GuardPassed { get; }
    
    public NavigationResultEventArgs(T page, NavigationContext context, bool guardPassed)
    {
        Page = page;
        Context = context;
        GuardPassed = guardPassed;
    }
}