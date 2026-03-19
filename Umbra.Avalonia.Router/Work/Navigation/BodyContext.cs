namespace Umbra.Router.Core.Work.Navigation;

public class BodyContext
{
    public BodyContext(object? value)
    {
        Value = value;
    }

    public object? Value { get; private set; }

    public bool TryGetValue<T>(out T? value) where T : class
    {
        if (Value is T typed)
        {
            value = typed;
            return true;
        }

        value = default;
        return false;
    }
}
