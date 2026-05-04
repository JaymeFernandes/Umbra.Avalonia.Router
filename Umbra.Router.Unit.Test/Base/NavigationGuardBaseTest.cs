using Umbra.Router.Core.Base;
using Umbra.Router.Core.Work;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;
using Umbra.Router.Unit.Test.Work.Trie;

namespace Umbra.Router.Unit.Test.Base;

public class NavigationGuardBaseTest
{
    [Fact]
    public async Task Test1()
    {
        var testGuard = new TestGuard(true);

        var uri = new RouteSnapshot("");
        var ctx = GenerateContext(uri);

        var result = await testGuard.ExecuteGuardAsync(ctx);
        
        Assert.True(result.Decision == GuardDecision.Allow);
        Assert.False(testGuard.RunGuardFailed);
        Assert.True(testGuard.RunGuardPassed);
    }

    [Fact]
    public async Task Test2()
    {
        var testGuard = new TestGuard(false);

        var uri = new RouteSnapshot("");
        var ctx = GenerateContext(uri);
        
        var result = await testGuard.ExecuteGuardAsync(ctx);
        
        Assert.True(result.Decision == GuardDecision.Deny);
        Assert.True(testGuard.RunGuardFailed);
        Assert.False(testGuard.RunGuardPassed);
    }
    
    private NavigationContext GenerateContext(RouteSnapshot context)
    {
        var template = new RouteTemplate(new TestDefinition(""));

        return template.ResolveContext(context);
    }
}

class TestGuard : NavigationGuardBase
{
    public bool Auth { get; }

    public bool RunGuardFailed { get; private set; } = false;
    public bool RunGuardPassed { get; private set; } = false;
    
    public TestGuard(bool auth)
    {
        Auth = auth;
    }
    
    protected override Task<GuardResult> GuardAsync(NavigationContext context)
    {
        return Auth ? 
            Task.FromResult(GuardResult.Allow()) : 
            Task.FromResult(GuardResult.Deny());
    }

    protected override Task OnGuardDeny(NavigationContext context)
    {
        RunGuardFailed = true;
        
        return base.OnGuardDeny(context);
    }

    protected override Task OnGuardAllow(NavigationContext context)
    {
        RunGuardPassed = true;
        
        return base.OnGuardAllow(context);
    }
}