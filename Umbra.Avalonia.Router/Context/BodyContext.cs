using System;
using System.Collections.Generic;
using System.Text;

namespace Umbra.Avalonia.Router.Context;

public class BodyContext
{
    public BodyContext(object? value)
    {
        Value = value;
    }

    public object? Value { get; private set; }

    public bool TryGetValue<T>(out T? value) where T : class
    {
        var isEmpty = Value is null;

        value = isEmpty ? default : (T)Value;

        return !isEmpty;
    }
}
