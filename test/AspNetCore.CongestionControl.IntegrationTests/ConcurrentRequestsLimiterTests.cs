using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Machine.Specifications;

namespace AspNetCore.CongestionControl.IntegrationTests
{
    class ConcurrentRequestsLimiterTests
    {
        [Subject("Concurrent Requests Limiter"), Tags("Negative Test")]
        public class When_request_is_made_without_client_id_and_anonymous_clients_are_not_allowed
        {
            Establish context = () =>
            {
                _testServer = TestServerFactory.Create(services =>
                {
                    services.AddCongestionControl(options =>
                    {
                        options.AllowAnonymousClients = false;
                        options.AddConcurrentRequestLimiter();
                    });

                    services.AddSingleton<IStartupFilter, StartupFilterWithCongestionControl>();
                });

                _client = _testServer.CreateClient();
            };

            Because of = () =>
            {
                _client.GetAsync("api/values").ContinueWith((response) => 
                {
                    _response = response.Await();
                }).Await();
            };

            It should_result_in_unauthorized_response = () =>
            {
                _response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
            };

            static HttpResponseMessage _response;
            static HttpClient _client;
            static TestServer _testServer;
        }

        [Subject("Concurrent Requests Limiter"), Tags("Negative Test")]
        public class When_making_3_concurrent_requests_to_api_limited_at_2_concurrent_requests_per_60_seconds
        {
            Establish context = () =>
            {
                _testServer = TestServerFactory.Create(services =>
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

                _client = _testServer.CreateClient();
                _client.DefaultRequestHeaders.Add("x-api-key", Guid.NewGuid().ToString());
            };

            Because of = () =>
            {
                var tasks = new List<Task<HttpResponseMessage>>();

                for (var i = 0; i < AsyncRequests; i++)
                {
                    tasks.Add(_client.GetAsync("api/values"));
                }

                Task.WhenAll(tasks).ContinueWith(response =>
                {
                    _response = response.Await();
                }).Await();
            };

            It should_not_allow_1_out_of_3_requests = () =>
            {
                _response.SingleOrDefault(resp => resp.StatusCode == HttpStatusCode.TooManyRequests)
                    .ShouldNotBeNull();
            };

            const int AsyncRequests = 3;

            static HttpResponseMessage[] _response;
            static HttpClient _client;
            static TestServer _testServer;
        }

        [Subject("Concurrent Requests Limiter"), Tags("Negative Test")]
        public class When_making_3_concurrent_requests_to_api_limited_at_2_concurrent_requests_per_60_seconds_using_redis
        {
            Establish context = () =>
            {
                _testServer = TestServerFactory.Create(services =>
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

                _client = _testServer.CreateClient();
            };

            Because of = () =>
            {
                var tasks = new List<Task<HttpResponseMessage>>();
                var apiKey = Guid.NewGuid().ToString();

                for (var i = 0; i < AsyncRequests; i++)
                {
                    tasks.Add(_client.GetAsync($"api/values?api_key={apiKey}"));
                }

                Task.WhenAll(tasks).ContinueWith(response =>
                {
                    _response = response.Await();
                }).Await();
            };

            It should_not_allow_1_out_of_3_requests = () =>
            {
                _response.SingleOrDefault(resp => resp.StatusCode == HttpStatusCode.TooManyRequests)
                    .ShouldNotBeNull();
            };

            const int AsyncRequests = 3;

            static HttpResponseMessage[] _response;
            static HttpClient _client;
            static TestServer _testServer;
        }

        [Subject("Concurrent Requests Limiter"), Tags("Positive Test")]
        public class When_making_2_concurrent_requests_to_api_limited_at_2_concurrent_requests_per_60_seconds
        {
            Establish context = () =>
            {
                _testServer = TestServerFactory.Create(services =>
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

                _client = _testServer.CreateClient();
                _client.DefaultRequestHeaders.Add("x-api-key", Guid.NewGuid().ToString());
            };

            Because of = () =>
            {
                var tasks = new List<Task<HttpResponseMessage>>();

                for (var i = 0; i < AsyncRequests; i++)
                {
                    tasks.Add(_client.GetAsync("api/values"));
                }

                Task.WhenAll(tasks).ContinueWith(response =>
                {
                    _response = response.Await();
                }).Await();
            };

            It should_allow_all_requests = () =>
            {
                _response.Any(resp => resp.StatusCode != HttpStatusCode.OK)
                    .ShouldBeFalse();
            };

            const int AsyncRequests = 2;

            static HttpResponseMessage[] _response;
            static HttpClient _client;
            static TestServer _testServer;
        }

        [Subject("Concurrent Requests Limiter"), Tags("Positive Test")]
        public class When_making_2_concurrent_requests_to_api_limited_at_2_concurrent_requests_per_60_seconds_using_redis
        {
            Establish context = () =>
            {
                _testServer = TestServerFactory.Create(services =>
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

                _client = _testServer.CreateClient();
                _client.DefaultRequestHeaders.Add("x-api-key", Guid.NewGuid().ToString());
            };

            Because of = () =>
            {
                var tasks = new List<Task<HttpResponseMessage>>();

                for (var i = 0; i < AsyncRequests; i++)
                {
                    tasks.Add(_client.GetAsync("api/values"));
                }

                Task.WhenAll(tasks).ContinueWith(response =>
                {
                    _response = response.Await();
                }).Await();
            };

            It should_allow_all_requests = () =>
            {
                _response.Any(resp => resp.StatusCode != HttpStatusCode.OK)
                    .ShouldBeFalse();
            };

            const int AsyncRequests = 2;

            static HttpResponseMessage[] _response;
            static HttpClient _client;
            static TestServer _testServer;
        }
    }
}