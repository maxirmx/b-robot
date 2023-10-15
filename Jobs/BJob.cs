using System;
using System.Threading;

// C# program to illustrate the
// creation of thread using
// non-static method
using System;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace Jobs {
public class BJob {
    public int Id { get; set; }
    public string ApiKey { get; set; }
    public string Strategy { get; set; }

    protected Thread? BThread { get; set; }
    protected bool ShallTerminate { get; set; }

    public BJob(int _id, string _apiKey, string _strategy)
    {
        Id = _id;
        ApiKey =_apiKey;
        Strategy = _strategy;
        ShallTerminate = false;
        BThread = new (new ThreadStart(BBJob)) { Name = "Binance job " + Id };
    }

    ~BJob()
    {
        Stop();
        BThread?.Join();
    }
    public void Start() {
        BThread?.Start();
    }
    public void Stop() {
        lock(this) {
            ShallTerminate = true;
        }
    }
    public void BBJob()
    {
        bool _shallTerminate = false;
        for (int z = 0; z < 30000 && !_shallTerminate; z++) {
            Console.WriteLine("{0} -- strategy: {1}", Thread.CurrentThread.Name, Strategy);
            Thread.Sleep(1000);
            lock(this) {
                _shallTerminate = ShallTerminate;
            }
        }
    }
}
}
