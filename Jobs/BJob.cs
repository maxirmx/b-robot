using Models;
using Binance.Net.Clients;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Objects;
using Binance.Net.Objects;

namespace Jobs {
public class BJob {
    public int Id { get; set; }
    public string ApiKey { get; set; }
    public string Secret { get; set; }
    public string Strategy { get; set; }

    protected Thread? BThread { get; set; }
    protected bool ShallTerminate { get; set; }

    public BJob(BTask btask)
    {
        Id = btask.Id;
        ApiKey = btask.ApiKey;
        Strategy = btask.Strategy;
        Secret = btask.Secret;
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
    public async void BBJob()
    {
        bool _shallTerminate = false;

        BinanceRestClient.SetDefaultOptions(options =>
        {
            options.ApiCredentials = new ApiCredentials(ApiKey, Secret);
        });
        BinanceSocketClient.SetDefaultOptions(options =>
        {
            options.ApiCredentials = new ApiCredentials(ApiKey, Secret);
        });

        Console.WriteLine("Starting {0} -- strategy: {1}", Thread.CurrentThread.Name, Strategy);
        var client = new BinanceRestClient();


        while (!_shallTerminate) {
            Console.WriteLine("{0} -- strategy: {1}", Thread.CurrentThread.Name, Strategy);
            // Get the account info
            var accountInfo = await client.SpotApi.Account.GetAccountInfoAsync();

            // Check if the response was successful
            if (accountInfo.Success)
            {
                foreach (var balance in accountInfo.Data.Balances)
                {
                    if (balance.Total > 0) {
                        Console.WriteLine($"{balance.Asset}: {balance.Total}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error: {accountInfo.Error?.Message}");
            }

            Thread.Sleep(1000 * 60);   /* 60 seconds */
            lock(this) {
                _shallTerminate = ShallTerminate;
            }
        }
    }
}
}
