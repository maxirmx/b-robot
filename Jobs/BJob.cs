// Copyright (C) 2023 Maxim [maxirmx] Samsonov (www.sw.consulting)
// All rights reserved.
// This file is a part of b-robot applcation
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 1. Redistributions of source code must retain the above copyright
// notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in the
// documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDERS OR CONTRIBUTORS
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using b_robot_api.Models;
using Binance.Net.Clients;
using CryptoExchange.Net.Authentication;

namespace b_robot_api.Jobs;
public class BJob
{
    public int Id { get; set; }
    public string ApiKey { get; set; }
    public string Secret { get; set; }
    public IStrategy Strategy { get; set; }

    protected Thread? BThread { get; set; }
    protected bool ShallTerminate { get; set; }
    protected bool IsRunning { get; set; }
    protected bool HasFailed { get; set; }

    public BJob(BTask btask, User user)
    {
        Id = btask.Id;
        ApiKey = user.ApiKey;
        _ = decimal.TryParse(btask.Threshold, out decimal threshold);
        Strategy = BStrategyFactory.CreateStategy( btask.Strategy, btask.Symbol1, btask.Symbol2, threshold );
        Secret = user.ApiSecret;
        ShallTerminate = false;
        IsRunning = false;
        HasFailed = false;
    }

    ~BJob()
    {
        Stop();
    }
    public void Start()
    {
        ShallTerminate = false;
        IsRunning = false;
        HasFailed = false;
        BThread = new (new ThreadStart(BBJob)) { Name = "Binance job " + Id };
        if (BThread == null) HasFailed = true;
        else  BThread.Start();
    }
    public void Stop()
    {
        lock(this) {
            ShallTerminate = true;
        }
        BThread?.Join();
        BThread = null;
    }
    public void BBJob()
    {
        bool _shallTerminate = false;
        string threadName = Thread.CurrentThread.Name ?? "unknown";

        try {

            BinanceRestClient.SetDefaultOptions(options => { options.ApiCredentials = new ApiCredentials(ApiKey, Secret); });
//        BinanceSocketClient.SetDefaultOptions(options => { options.ApiCredentials = new ApiCredentials(ApiKey, Secret); });

            Console.WriteLine($"Starting {threadName} -- strategy: {Strategy.GetName()}");
            var client = new BinanceRestClient();


            while (!_shallTerminate) {
                lock(this) {
                    _shallTerminate = ShallTerminate;
                    IsRunning = true;
                }
                Strategy.ProcessTick(threadName, client);
                Thread.Sleep(1000 * 5);   /* 5 seconds */
            }
        }
        catch(Exception) {
            lock(this) {
                HasFailed = true;
            }
            Console.WriteLine($"Exception at {threadName} -- strategy: {Strategy.GetName()} -- terminating");
        }
        finally {
            lock(this) {
                IsRunning = false;
            }
            Console.WriteLine($"Terminated {threadName} -- strategy: {Strategy.GetName()}  -- HasFailed: {HasFailed}");
        }
    }

    public bool QueryRunning()
    {
        lock(this) {
            return IsRunning;
        }
    }

    public bool QueryFailed()
    {
        lock(this) {
            return HasFailed;
        }
    }
}
