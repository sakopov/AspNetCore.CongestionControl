﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentRequestsLimiterTests.cs">
//   Copyright (c) 2018-2021 Sergey Akopov
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AspNetCore.CongestionControl.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using FluentAssertions;
    using Xunit;

    public class ConcurrentRequestsLimiterTests
    {
        [Fact(DisplayName = "Request is Made Without Client ID and Anonymous Clients are Not Allowed")]
        public async void RequestIsMadeWithoutClientIdAndAnonymousClientsAreNotAllowed()
        {
            // Given
            var testServer = TestServerFactory.Create(services =>
            {
                services.AddCongestionControl(options =>
                {
                    options.AllowAnonymousClients = false;
                    options.AddConcurrentRequestLimiter();
                });

                services.AddSingleton<IStartupFilter, StartupFilterWithCongestionControl>();
            });

            var client = testServer.CreateClient();

            // When an anonymous request is made
            var response = await client.GetAsync("api/values");

            // Then it should result in unauthorized response
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "Making 3 Concurrent Requests to API Limited at 2 Concurrent Requests Per 60 Seconds")]
        public async void Making3ConcurrentRequestsToApiLimitedAt2ConcurrentRequestsPer60Seconds()
        {
            // Given
            const int AsyncRequests = 3;

            var testServer = TestServerFactory.Create(services =>
            {
                services.AddCongestionControl(options =>
                {
                    options.AddConcurrentRequestLimiter(crl =>
                    {
                        crl.Capacity = 2;
                        crl.RequestTimeToLive = 60;
                    });
                });

                services.AddSingleton<IStartupFilter, StartupFilterWithCongestionControl>();
            });

            var client = testServer.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", Guid.NewGuid().ToString());

            // When the requests are made
            var tasks = new List<Task<HttpResponseMessage>>();

            for (var i = 0; i < AsyncRequests; i++)
            {
                tasks.Add(client.GetAsync("api/values"));
            }

            var response = await Task.WhenAll(tasks);

            // Then it should not allow 1 out of 3 requests
            response.SingleOrDefault(resp => resp.StatusCode == HttpStatusCode.TooManyRequests).Should().NotBeNull();
        }

        [Fact(DisplayName = "Making 3 Concurrent Requests to API Limited at 2 Concurrent Requests Per 60 Seconds Using Redis")]
        public async void Making3ConcurrentRequestsToApiLimitedAt2ConcurrentRequestsPer60SecondsUsingRedis()
        {
            // Given
            const int AsyncRequests = 3;

            var testServer = TestServerFactory.Create(services =>
            {
                services.AddCongestionControl(options =>
                {
                    // Try another client identifier strategy for good measure
                    options.AddQueryBasedClientIdentifierProvider();
                    options.AddRedisStorage("127.0.0.1:6379");
                    options.AddConcurrentRequestLimiter(crl =>
                    {
                        crl.Capacity = 2;
                        crl.RequestTimeToLive = 60;
                    });
                });

                services.AddSingleton<IStartupFilter, StartupFilterWithCongestionControl>();
            });

            var client = testServer.CreateClient();

            // When the requests are made
            var tasks = new List<Task<HttpResponseMessage>>();
            var apiKey = Guid.NewGuid().ToString();

            for (var i = 0; i < AsyncRequests; i++)
            {
                tasks.Add(client.GetAsync($"api/values?api_key={apiKey}"));
            }

            var response = await Task.WhenAll(tasks);

            // It should not allow 1 out of 3 requests
            response.SingleOrDefault(resp => resp.StatusCode == HttpStatusCode.TooManyRequests).Should().NotBeNull();
        }

        [Fact(DisplayName = "Making 2 Concurrent Requests to API Limited at 2 Concurrent Requests Per 60 Seconds")]
        public async void Making2ConcurrentRequestsToApiLimitedAt2ConcurrentRequestsPer60Seconds()
        {
            // Given
            const int AsyncRequests = 2;

            var testServer = TestServerFactory.Create(services =>
            {
                services.AddCongestionControl(options =>
                {
                    options.AddConcurrentRequestLimiter(crl =>
                    {
                        crl.Capacity = 2;
                        crl.RequestTimeToLive = 60;
                    });
                });

                services.AddSingleton<IStartupFilter, StartupFilterWithCongestionControl>();
            });

            var client = testServer.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", Guid.NewGuid().ToString());

            // When the requests are made
            var tasks = new List<Task<HttpResponseMessage>>();

            for (var i = 0; i < AsyncRequests; i++)
            {
                tasks.Add(client.GetAsync("api/values"));
            }

            var response = await Task.WhenAll(tasks);

            // Then it should allow all requests
            response.Any(resp => resp.StatusCode != HttpStatusCode.OK).Should().BeFalse();
        }

        [Fact(DisplayName = "Making 2 Concurrent Requests to API Limited at 2 Concurrent Requests Per 60 Seconds Using Redis")]
        public async void Making2ConcurrentRequestsToApiLimitedAt2ConcurrentRequestsPer60SecondsUsingRedis()
        {
            // Given
            const int AsyncRequests = 2;

            var testServer = TestServerFactory.Create(services =>
            {
                services.AddCongestionControl(options =>
                {
                    options.AddRedisStorage("127.0.0.1:6379");
                    options.AddConcurrentRequestLimiter(crl =>
                    {
                        crl.Capacity = 2;
                        crl.RequestTimeToLive = 60;
                    });
                });

                services.AddSingleton<IStartupFilter, StartupFilterWithCongestionControl>();
            });

            var client = testServer.CreateClient();
            client.DefaultRequestHeaders.Add("x-api-key", Guid.NewGuid().ToString());

            // When the requests are made
            var tasks = new List<Task<HttpResponseMessage>>();

            for (var i = 0; i < AsyncRequests; i++)
            {
                tasks.Add(client.GetAsync("api/values"));
            }

            var response = await Task.WhenAll(tasks);

            // Then it should allow all requests
            response.Any(resp => resp.StatusCode != HttpStatusCode.OK).Should().BeFalse();
        }
    }
}
