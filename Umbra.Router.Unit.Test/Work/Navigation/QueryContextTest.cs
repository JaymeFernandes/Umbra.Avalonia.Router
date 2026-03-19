using Umbra.Router.Core.Work.Navigation;

namespace Umbra.Router.Unit.Test.Work.Navigation;

public class QueryContextTest
{
    [Fact]
    public void ParseQuery_ShouldConvertValuesToCorrectTypes()
    {
        var guid = Guid.NewGuid();
        
        var query = new QueryContext($"id=20&role=user&session={Escape(guid.ToString())}&public=false");

        Assert.Equal(4, query.Count);
        
        var containsId = query.TryGetValue("id", out int? id);
        var containsRole = query.TryGetValue("role", out string? role);
        var containsSession = query.TryGetValue("session", out Guid? session);
        var containsPublic = query.TryGetValue("public", out bool publicProfile);
        
        var containsTest = query.TryGetValue("test", out string? test);
        
        Assert.False(containsTest);
        Assert.Equal("", test);
        
        Assert.True(containsId);
        Assert.Equal(20, id);
        
        Assert.True(containsRole);
        Assert.Equal("user", role);
        
        Assert.True(containsSession);
        Assert.Equal(guid, session);
        
        Assert.True(containsPublic);
        Assert.False(publicProfile);
    }

    [Fact]
    public void Constructor_ShouldParseQueryFromFullUrl()
    {
        Dictionary<string, string> values = new Dictionary<string, string>()
        {
            { "id", "20" },
            { "role", "user" },
            { "email", "test@test.com" },
            { "nickname", "test" },
            { "username", "User Test" }
        };
        
        var query = new QueryContext($"app://myapp.com/user/update/info?{string.Join('&', values
            .Select(x => $"{x.Key}={Escape(x.Value)}"))}");
        
        Assert.Equal(values.Count, query.Count);

        foreach (var val in values)
        {
            var contains = query.TryGetValue(val.Key, out string? value);
            
            Assert.True(contains);
            Assert.Equal(val.Value, value);
        }
    }

    [Fact]
    public void Constructor_ShouldHandleLeadingQuestionMark()
    {
        var query = new QueryContext($"?username={Escape("Username Test")}");
        
        var contains = query.TryGetValue("username", out string? value);
        
        Assert.True(contains);
        Assert.Equal("Username Test", value);
    }

    [Fact]
    public void GetValues_ShouldReturnAllValuesForDuplicateKeys()
    {
        List<string> roles = new List<string>()
        {
            "administrator",
            "user",
            "moderator",
            "beta tester"
        };
        
        var query = new QueryContext(string.Join('&', roles.Select(x => $"role={Escape(x)}")));

        var queryValues = query.GetValues<string>("role");
        
        Assert.Equal(queryValues, roles);
    }

    [Fact]
    public void GetValue_ShouldReturnQueryValueWithAllStoredValues()
    {
        List<string> roles = new List<string>()
        {
            "administrator",
            "user",
            "moderator",
            "beta tester"
        };
        
        var query = new QueryContext(string.Join('&', roles.Select(x => $"role={Escape(x)}")));


        var queryValues = query.GetValue<QueryValue>("role");
        
        Assert.Equal(roles, queryValues.Values);
    }
    
    [Fact]
    public void ParameterWithoutValue()
    {
        var query = new QueryContext("id");

        var contains = query.TryGetValue("id", out string value);

        Assert.True(contains);
        Assert.Equal("", value);
    }

    private string Escape(string value)
        => Uri.EscapeDataString(value);
}