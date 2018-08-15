using System;
using AspNetCore.CongestionControl.TestApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.CongestionControl.IntegrationTests
{
    static class TestServerFactory
    {
        public static TestServer Create(Action<IServiceCollection> services)
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>();

            if (services != null)
            {
                webHostBuilder.ConfigureTestServices(services);
            }

            return new TestServer(webHostBuilder);
        }
    }
}