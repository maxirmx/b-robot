namespace Jobs {
public sealed class BJobs : Dictionary<int, BJob>
{
    private static readonly Lazy<BJobs> lazy =
        new (() => new BJobs());

    public static BJobs Instance { get { return lazy.Value; } }

    private BJobs()
    {
    }
}
}
