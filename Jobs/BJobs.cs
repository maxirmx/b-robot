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

namespace b_robot_api.Jobs;
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
    public static void StartBJob(int id)
    {
        lock(Instance) {
            if (Instance.ContainsKey(id)) {
                Instance[id].Start();
            }
        }
    }
    public static bool QueryRunning(int id)
    {
        lock(Instance) {
            return Instance.ContainsKey(id) && Instance[id].QueryRunning();
        }
    }
    public static bool QueryFailed(int id)
    {
        lock(Instance) {
            return Instance.ContainsKey(id) && Instance[id].QueryFailed();
        }
    }
    private BJobs()
    {
    }
}
