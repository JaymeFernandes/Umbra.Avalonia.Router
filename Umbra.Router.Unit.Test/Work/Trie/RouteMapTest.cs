using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Unit.Test.Work.Trie;

public class RouteMapTest
{
    [Fact]
    public void Should_match_literal_route()
    {
        var map = new RouteMap();
        var template = new RouteTemplate(new TestDefinition("/home"));

        map.Add(template);

        var result = map.Match("/home");

        Assert.NotNull(result);
        Assert.Equal(template, result);
    }

    [Fact]
    public void Should_return_null_when_route_not_found()
    {
        var map = new RouteMap();

        map.Add(new RouteTemplate(new TestDefinition("/home")));

        var result = map.Match("/about");

        Assert.Null(result);
    }

    [Fact]
    public void Should_match_parameter_route()
    {
        var map = new RouteMap();
        var template = new RouteTemplate(new TestDefinition("/users/:id"));

        map.Add(template);

        var result = map.Match("/users/42");

        Assert.NotNull(result);
        Assert.Equal(template, result);
    }

    [Fact]
    public void Should_prioritize_literal_over_parameter()
    {
        var map = new RouteMap();

        var literal = new RouteTemplate(new TestDefinition("/users/me"));
        var param = new RouteTemplate(new TestDefinition("/users/:id"));

        map.Add(param);
        map.Add(literal);

        var result = map.Match("/users/me");

        Assert.Equal(literal, result);
    }

    [Fact]
    public void Should_match_catch_all()
    {
        var map = new RouteMap();
        var template = new RouteTemplate(new TestDefinition("/files/**"));

        map.Add(template);

        var result = map.Match("/files/a/b/c");

        Assert.NotNull(result);
        Assert.Equal(template, result);
    }

    [Fact]
    public void Should_use_catch_all_when_no_other_match()
    {
        var map = new RouteMap();

        var catchAll = new RouteTemplate(new TestDefinition("/docs/**"));

        map.Add(catchAll);

        var result = map.Match("/docs/a/b/c");

        Assert.Equal(catchAll, result);
    }

    [Fact]
    public void Should_match_nested_routes()
    {
        var map = new RouteMap();

        var template = new RouteTemplate(new TestDefinition("/users/:id/profile"));

        map.Add(template);

        var result = map.Match("/users/10/profile");

        Assert.Equal(template, result);
    }
    
    [Fact]
    public void Should_match_more_specific_route()
    {
        var map = new RouteMap();

        var generic = new RouteTemplate(new TestDefinition("/users/:id"));
        var specific = new RouteTemplate(new TestDefinition("/users/:id/profile"));

        map.Add(generic);
        map.Add(specific);

        var result = map.Match("/users/10/profile");

        Assert.Equal(specific, result);
    }
    
    [Fact]
    public void Should_prioritize_literal_over_parameter_with_same_depth()
    {
        var map = new RouteMap();

        var param = new RouteTemplate(new TestDefinition("/users/:id"));
        var literal = new RouteTemplate(new TestDefinition("/users/admin"));

        map.Add(param);
        map.Add(literal);

        var result = map.Match("/users/admin");

        Assert.Equal(literal, result);
    }
    
    [Fact]
    public void CatchAll_ShouldNotOverrideSpecificRoutes()
    {
        var map = new RouteMap();

        var specific = new RouteTemplate(new TestDefinition("/files/image"));
        var catchAll = new RouteTemplate(new TestDefinition("/files/**"));

        map.Add(catchAll);
        map.Add(specific);

        var result = map.Match("/files/image");

        Assert.Equal(specific, result);
    }
    
    [Fact]
    public void Should_not_match_when_route_is_shorter()
    {
        var map = new RouteMap();

        var template = new RouteTemplate(new TestDefinition("/users/:id/profile"));

        map.Add(template);

        var result = map.Match("/users/10");

        Assert.Null(result);
    }
    
    [Fact]
    public void Should_ignore_duplicate_slashes()
    {
        var map = new RouteMap();

        var template = new RouteTemplate(new TestDefinition("/users/list"));

        map.Add(template);

        var result = map.Match("//users///list");

        Assert.Equal(template, result);
    }
    
    [Fact]
    public void Should_match_encoded_segment()
    {
        var map = new RouteMap();

        var template = new RouteTemplate(new TestDefinition("/users/:name"));

        map.Add(template);

        var result = map.Match("/users/User%20Test");

        Assert.NotNull(result);
    }
    
    [Fact]
    public void Should_choose_most_specific_among_multiple_candidates()
    {
        var map = new RouteMap();

        var a = new RouteTemplate(new TestDefinition("/a/:id"));
        var b = new RouteTemplate(new TestDefinition("/a/b"));
        var c = new RouteTemplate(new TestDefinition("/a/:id/c"));

        map.Add(a);
        map.Add(b);
        map.Add(c);

        var result = map.Match("/a/b");

        Assert.Equal(b, result);
    }
    
    [Fact]
    public void Should_handle_ambiguous_parameters()
    {
        var map = new RouteMap();

        var a = new RouteTemplate(new TestDefinition("/users/:id"));
        var b = new RouteTemplate(new TestDefinition("/users/:name"));

        map.Add(a);
        map.Add(b);

        var result = map.Match("/users/10");

        Assert.NotNull(result);
    }
}

public class TestDefinition : NavigationDefinition
{
    public TestDefinition(string route) : base(typeof(object),  typeof(object))
    {
        Route = route;
    }
}