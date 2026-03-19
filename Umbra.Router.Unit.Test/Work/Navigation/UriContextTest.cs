using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Unit.Test.Work.Navigation;

public class UriContextTest
{
    [Fact]
    public void Constructor_ShouldParsePathAndSegments()
    {
        var uri = new UriContext("/user/profile");

        Assert.Equal("/user/profile", uri.Path);
        Assert.Equal(2, uri.Segments.Length);
        Assert.Equal("user", uri.Segments[0]);
        Assert.Equal("profile", uri.Segments[1]);
    }

    [Fact]
    public void Constructor_ShouldIgnoreSchemeAndHost()
    {
        var uri = new UriContext("app://myapp.com/user/profile");

        Assert.Equal("/user/profile", uri.Path);
        Assert.Equal(["user","profile"], uri.Segments);
    }

    [Fact]
    public void Constructor_ShouldParseQuery()
    {
        var uri = new UriContext("/user?id=20&role=admin");

        Assert.NotNull(uri.Query);
        Assert.Equal(2, uri.Query!.Count);

        uri.Query.TryGetValue("id", out int id);
        uri.Query.TryGetValue("role", out string role);

        Assert.Equal(20, id);
        Assert.Equal("admin", role);
    }

    [Fact]
    public void Constructor_ShouldIgnoreFragment()
    {
        var uri = new UriContext("/user/profile#section1");

        Assert.Equal("/user/profile", uri.Path);
        Assert.Equal(["user","profile"], uri.Segments);
    }

    [Fact]
    public void Constructor_ShouldDecodeEncodedSegments()
    {
        var uri = new UriContext("/user/User%20Test");

        Assert.Equal(2, uri.Segments.Length);
        Assert.Equal("User Test", uri.Segments[1]);
    }

    [Fact]
    public void Constructor_ShouldSupportSegmentsConstructor()
    {
        var uri = new UriContext(new[] { "user", "profile" });

        Assert.Equal("user/profile", uri.Raw);
        Assert.Equal(["user","profile"], uri.Segments);
    }

    [Fact]
    public void Constructor_ShouldAttachBody()
    {
        var body = new { name = "John" };

        var uri = new UriContext("/user/create", body);

        Assert.NotNull(uri.Body);
    }

    [Fact]
    public void Constructor_ShouldAttachBodyWhenUsingSegments()
    {
        var body = new { name = "John" };

        var uri = new UriContext(new[] { "user", "create" }, body);

        Assert.NotNull(uri.Body);
    }

    [Fact]
    public void Constructor_ShouldHandleRootPath()
    {
        var uri = new UriContext("/");

        Assert.Equal("/", uri.Path);
        Assert.Empty(uri.Segments);
    }
}