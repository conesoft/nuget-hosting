using System;

namespace Conesoft.Hosting;

public static class Safe
{
    public static T? Try<T>(Func<T?> method)
    {
        try
        {
            return method();
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static void Try(Action action)
    {
        try
        {
            action();
        }
        catch (Exception)
        {
        }
    }
}
