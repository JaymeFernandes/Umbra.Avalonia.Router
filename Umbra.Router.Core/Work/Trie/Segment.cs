namespace Umbra.Router.Core.Work.Trie;

public abstract class Segment
{
    protected Segment(string key)
    {
        Key = key;
    }

    public string Key { get; set; }
}

public class LiteralSegment : Segment
{
    public LiteralSegment(string key) : base(key)
    {
    }
}

public class ParameterSegment : Segment
{
    public ParameterSegment(string key) : base(key)
    {
    }
}

public class CatchAllSegment : Segment
{
    public CatchAllSegment() : base("**")
    {
    }
}