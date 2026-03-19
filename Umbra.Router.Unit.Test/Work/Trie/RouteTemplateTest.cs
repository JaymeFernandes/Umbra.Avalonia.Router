using Umbra.Router.Core.Configuration;
using Umbra.Router.Core.Work.Trie;

namespace Umbra.Router.Unit.Test.Work.Trie;

public class RouteTemplateTests
{
    private NavigationDefinition Def(string route)
        => new TestDefinition(route);

    [Fact]
    public void Should_parse_literal_segments()
    {
        var template = new RouteTemplate(Def("/users/list"));

        Assert.Equal(2, template.Segments.Count);
        Assert.IsType<LiteralSegment>(template.Segments[0]);
        Assert.IsType<LiteralSegment>(template.Segments[1]);
    }

    [Fact]
    public void Should_parse_parameter_segment()
    {
        var template = new RouteTemplate(Def("/users/:id"));

        Assert.Equal(2, template.Segments.Count);
        Assert.IsType<LiteralSegment>(template.Segments[0]);
        Assert.IsType<ParameterSegment>(template.Segments[1]);
    }

    [Fact]
    public void Should_parse_catchall_segment()
    {
        var template = new RouteTemplate(Def("/files/**"));

        Assert.Equal(2, template.Segments.Count);
        Assert.IsType<CatchAllSegment>(template.Segments[1]);
    }

    [Fact]
    public void Should_ignore_empty_segments()
    {
        var template = new RouteTemplate(Def("//users///list"));
        
        Assert.Equal(2, template.Segments.Count);
    }

    [Fact]
    public void Should_extract_parameters_from_route()
    {
        var template = new RouteTemplate(Def("/users/:id"));

        var snapshot = new RouteSnapshot("/users/42");

        var context = template.ResolveContext(snapshot);

        var containsId = context.Parameters.TryGetValue("id", out var id);

        Assert.True(containsId);
        Assert.Equal("42", id);
    }

    [Fact]
    public void Should_extract_multiple_parameters()
    {
        var template = new RouteTemplate(Def("/users/:userId/orders/:orderId"));

        var snapshot = new RouteSnapshot("/users/7/orders/99");

        var context = template.ResolveContext(snapshot);

        var containsUserId = context.Parameters.TryGetValue("userId", out var userId);
        var containsOrderId = context.Parameters.TryGetValue("orderId", out var orderId);
        
        
        Assert.True(containsUserId);
        Assert.Equal("7", userId);
        
        Assert.True(containsOrderId);
        Assert.Equal("99", orderId);
    }

    [Fact]
    public void Should_return_empty_parameters_when_no_params_exist()
    {
        var template = new RouteTemplate(Def("/users/list"));

        var snapshot = new RouteSnapshot("/users/list");

        var context = template.ResolveContext(snapshot);

        Assert.True(!context.Parameters.Any());
    }
}