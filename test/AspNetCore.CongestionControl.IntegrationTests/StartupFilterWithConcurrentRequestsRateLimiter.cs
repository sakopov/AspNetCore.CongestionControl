using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCore.CongestionControl.IntegrationTests
{
    class StartupFilterWithConcurrentRequestsRateLimiter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseConcurrentRequestsLimiter();

                next(app);
            };
        }
    }
}