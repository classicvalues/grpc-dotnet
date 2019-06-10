#region Copyright notice and license

// Copyright 2019 The gRPC Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Runtime.InteropServices;
using Count;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sample.Clients
{
    public class Program
    {
        private const string Address = "localhost:50051";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    Uri baseAddress;

                    // ALPN is not available on macOS so only use HTTP/2 without TLS
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                        baseAddress = new Uri($"http://{Address}");
                    }
                    else
                    {
                        baseAddress = new Uri($"https://{Address}");
                    }

                    services.AddHostedService<Worker>();
                    services.AddGrpcClient<Counter.CounterClient>(options =>
                    {
                        options.BaseAddress = baseAddress;
                    });
                });
    }
}