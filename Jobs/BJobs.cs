namespace Jobs {
public sealed class BJobs : Dictionary<int, BJob>
{
    private static readonly Lazy<BJobs> lazy = new (() => new BJobs());

    public static BJobs Instance
    {
        get { return lazy.Value; }
    }

    public static void AddBJob(BJob bJob)
    {
        lock(Instance) {
            if (!Instance.ContainsKey(bJob.Id)) {
                Instance.Add(bJob.Id, bJob);
            }
        }
    }
    public static void RemoveBJob(int id)
    {
        StopBJob(id);
        lock(Instance) {
            Instance.Remove(id);
        }
    }
    public static void StopBJob(int id)
    {
        lock(Instance) {
            if (Instance.ContainsKey(id)) {
                Instance[id].Stop();
            }
        }
    }
    private BJobs()
    {
    }
}
}
