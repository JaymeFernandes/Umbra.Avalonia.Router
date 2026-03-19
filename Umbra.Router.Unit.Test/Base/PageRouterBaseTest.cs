using Umbra.Router.Core.Base;
using Umbra.Router.Core.Work.Navigation;
using Umbra.Router.Core.Work.Trie;
using Umbra.Router.Unit.Test.Work.Trie;

namespace Umbra.Router.Unit.Test.Base;

public class PageRouterBaseTest
{
    [Fact]
    public async Task InitializeAsync_ShouldCompleteNavigation()
    {
        var router = new TestRouter();
        var ctx = GenerateContext();

        await router.InitializeAsync(ctx);

        Assert.True(router.NavigatedCalled);
        Assert.True(router.CompletedCalled);
        Assert.Equal(RouterStatus.Completed, router.Status);
    }

    [Fact]
    public async Task InitializeAsync_ShouldRunOnlyOnce()
    {
        var router = new TestRouter();
        var ctx = GenerateContext();

        await router.InitializeAsync(ctx);
        await router.InitializeAsync(ctx);

        Assert.Equal(1, router.NavigateCount);
    }

    [Fact]
    public async Task CancelNavigation_ShouldCancelNavigation()
    {
        var router = new DelayRouter();
        var ctx = GenerateContext();

        var navigationTask = router.InitializeAsync(ctx);

        await Task.Delay(50);
        await router.CancelNavigation();

        Assert.Equal(RouterStatus.Disposed, router.Status);
    }

    [Fact]
    public async Task InitializeAsync_ShouldHandleOperationCanceled()
    {
        var router = new CancelRouter();
        var ctx = GenerateContext();;

        await router.InitializeAsync(ctx);

        Assert.Equal(RouterStatus.Disposed, router.Status);
    }

    [Fact]
    public async Task InitializeAsync_ShouldHandleNavigationError()
    {
        var router = new ErrorRouter();
        var ctx = GenerateContext();

        await router.InitializeAsync(ctx);

        Assert.True(router.ErrorHandled);
    }

    private NavigationContext GenerateContext()
    {
        var template = new RouteTemplate(new TestDefinition(""));

        return template.ResolveContext(new RouteSnapshot(""));
    }
}

class TestRouter : PageRouterBase
{
    public bool NavigatedCalled;
    public bool CompletedCalled;
    public int NavigateCount;
    private bool _delay;

    public TestRouter(bool delay=false)
    {
        _delay = delay;
    }

    public override async Task OnNavigatedToAsync(CancellationToken ctx)
    {
        NavigateCount++;
        NavigatedCalled = true;

        if (_delay)
            await Task.Delay(1000, ctx);
    }

    protected override Task OnCompletedAsync()
    {
        CompletedCalled = true;
        return Task.CompletedTask;
    }
}

class DelayRouter : PageRouterBase
{
    public override async Task OnNavigatedToAsync(CancellationToken ctx)
    {
        await Task.Delay(2000, ctx);
    }
}

class ErrorRouter : PageRouterBase
{
    public bool ErrorHandled;

    public override Task OnNavigatedToAsync(CancellationToken ctx)
    {
        throw new Exception("test error");
    }

    protected override Task<bool> OnNavigationErrorAsync(Exception ex)
    {
        ErrorHandled = true;
        return Task.FromResult(true);
    }
}

class CancelRouter : PageRouterBase
{
    public override async Task OnNavigatedToAsync(CancellationToken ctx)
    {
        await Task.Delay(2000 , ctx);

        await CancelNavigation();
    }
}