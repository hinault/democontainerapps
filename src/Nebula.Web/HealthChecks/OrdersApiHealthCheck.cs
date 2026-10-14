using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nebula.Web.HealthChecks
{
    internal class OrdersApiHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _factory;

        public OrdersApiHealthCheck(IHttpClientFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _factory.CreateClient("OrdersApi");
                using var response = await client.GetAsync("/health", cancellationToken);
                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy();

                return HealthCheckResult.Unhealthy($"Status code {(int)response.StatusCode}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }
        }
    }
}
