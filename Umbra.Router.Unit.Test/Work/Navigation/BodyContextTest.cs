using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Unit.Test.Work.Navigation;

public class BodyContextTest
{
    [Fact]
    public void TryGetValue_ShouldReturnBody_WhenTypeMatches()
    {
        var obj = new MyBodyTest();
        var body = new BodyContext(obj);
        
        var success = body.TryGetValue(out MyBodyTest? result);
        
        Assert.True(success);
        Assert.NotNull(result);

        Assert.Same(obj, result);
        Assert.Same(obj, body.Value);
    }
    
    [Fact]
    public void TryGetValue_ShouldReturnFalse_WhenTypeDoesNotMatch()
    {
        var body = new BodyContext(new MyBodyTest());

        var success = body.TryGetValue(out string? result);

        Assert.False(success);
        Assert.Null(result);
    }

    class MyBodyTest
    {
        public string Id = Guid
            .NewGuid()
            .ToString();
    }
}