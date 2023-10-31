using Newtonsoft.Json;
using System.Reflection;
using Binance.Net.Clients;
using Binance.Net.Enums;

namespace b_robot_api.Jobs;
public interface IStrategy
{
    void ProcessTick(string threadName, BinanceRestClient client);
    string GetName();
}

public class ZeroStrategy : IStrategy
{
    public ZeroStrategy()
    {
    }

    public string GetName()
    {
        return "zero";
    }

    public void ProcessTick(string threadName, BinanceRestClient client)
    {
        Console.WriteLine($"{threadName} -- ZeroStrategy");
    }
}
public abstract class SimpleStrategy : IStrategy
{
    public string Algo { get; }
    public string Symbol1 {get; }
    public string Symbol2 {get; }
    public decimal Threshold {get; }
    public string Pair {get; }
    public SimpleStrategy(string _algo, string _symbol1, string _symbol2, decimal _threshold)
    {
        Algo = _algo;
        Symbol1 = _symbol1;
        Symbol2 = _symbol2;
        Pair = Symbol1 + Symbol2;
        Threshold = _threshold;
    }
    public abstract void ProcessTick(string threadName, BinanceRestClient client);
    protected async void ProcessTickInner(string threadName, BinanceRestClient client, bool less)
    {
        string amountString = "unknown/not found";
        decimal amountValue = 0M;
        var accountInfo = await client.SpotApi.Account.GetAccountInfoAsync();
        if (accountInfo.Success) {
            foreach (var balance in accountInfo.Data.Balances) {
                if (balance.Asset == Symbol1) {
                    amountValue = balance.Total;
                    amountString = balance.Total.ToString();
                    break;
                }
            }
        }
        else {
            Console.WriteLine($"Could not get account info: {accountInfo.Error?.Message}");
            amountString = "unknown/error";
        }

        var priceInfo = await client.SpotApi.ExchangeData.GetPriceAsync(Pair);
        var priceString = "unknown/error";
        decimal priceValue = 0M;
        if (accountInfo.Success) {
            priceValue = priceInfo.Data.Price;
            priceString = priceInfo.Data.Price.ToString();
        }
        else {
            Console.WriteLine($"Could not get price info: {priceInfo.Error?.Message}");
        }

        Console.WriteLine($"{threadName} -- {Algo} {Threshold} {Pair}: {priceString} ({amountString} {Symbol1})");

        if (amountValue > 0M && priceValue > 0M) {
            bool match = ((priceValue < Threshold) && less) ||
                         ((priceValue > Threshold) && !less);

            if (match) {
                Console.WriteLine($"Selling {amountString} {Symbol1}");
                var orderData = await client.SpotApi.Trading.PlaceOrderAsync(
                        Pair,
                        OrderSide.Sell,
                        SpotOrderType.Market,
                        quantity: amountValue);
                if (!orderData.Success) {
                    Console.WriteLine($"Could not place an order: {orderData.Error?.Message}");
                }
            }
        }
    }

    public string GetName()
    {
        return $"{Algo} @ {Threshold} {Pair}";
    }
}

public class StopLossStrategy : SimpleStrategy {
    public StopLossStrategy(string _symbol1, string _symbol2, decimal _threshold):
            base("stop-loss", _symbol1, _symbol2, _threshold)
    {
    }

    public override void ProcessTick(string threadName, BinanceRestClient client)
    {
        ProcessTickInner(threadName, client, true);
    }
}

public class TakeProfitStrategy : SimpleStrategy {
    public TakeProfitStrategy(string _symbol1, string _symbol2, decimal _threshold) :
            base("take-profit", _symbol1, _symbol2, _threshold)
    {
    }

    public override void ProcessTick(string threadName, BinanceRestClient client)
    {
        ProcessTickInner(threadName, client, false);
    }

}

public class BStrategyFactory
{
    public static IStrategy CreateStategy(string strategyName, string _symbol1, string _symbol2, decimal _threshold)
    {
        IStrategy strategy = new ZeroStrategy();
        if (strategyName == "stop-loss") {
            strategy = new StopLossStrategy(_symbol1, _symbol2, _threshold);
        }
        else if (strategyName == "take-profit") {
            strategy = new TakeProfitStrategy(_symbol1, _symbol2, _threshold);
        }
        else {
            Console.WriteLine($"Unknown strategy: {strategyName}");
        }
        return strategy;
    }
}
