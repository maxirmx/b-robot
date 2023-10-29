using System.Text.Json;
using Models;
using Binance.Net.Clients;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Binance.Net.Enums;

namespace Jobs {
public class BJob {
    public int Id { get; set; }
    public string ApiKey { get; set; }
    public string Secret { get; set; }
    public IStrategy Strategy { get; set; }

    protected Thread? BThread { get; set; }
    protected bool ShallTerminate { get; set; }

    public BJob(BTask btask)
    {
        Id = btask.Id;
        ApiKey = btask.ApiKey;
        _ = decimal.TryParse(btask.Threshold, out decimal threshold);
        Strategy = BStrategyFactory.CreateStategy( btask.Strategy, btask.Symbol1, btask.Symbol2, threshold );
        Secret = btask.Secret;
        ShallTerminate = false;
        BThread = new (new ThreadStart(BBJob)) { Name = "Binance job " + Id };
    }

    ~BJob()
    {
        Stop();
        BThread?.Join();
    }
    public void Start()
    {
        BThread?.Start();
    }
    public void Stop()
    {
        lock(this) {
            ShallTerminate = true;
        }
    }
    public void BBJob()
    {
        bool _shallTerminate = false;
        string threadName = Thread.CurrentThread.Name ?? "unknown";

        BinanceRestClient.SetDefaultOptions(options => { options.ApiCredentials = new ApiCredentials(ApiKey, Secret); });
//        BinanceSocketClient.SetDefaultOptions(options => { options.ApiCredentials = new ApiCredentials(ApiKey, Secret); });

        Console.WriteLine($"Starting {threadName} -- strategy: {Strategy.GetName()}");
        var client = new BinanceRestClient();


        while (!_shallTerminate) {
            Strategy.ProcessTick(threadName, client);
            Thread.Sleep(1000 * 5);   /* 5 seconds */
            lock(this) {
                _shallTerminate = ShallTerminate;
            }
        }
    }
}
}
