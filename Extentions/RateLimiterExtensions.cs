using System.Threading.RateLimiting;

namespace Lpgin2.Extentions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddCustomRateLimiters(
            this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("AuthLimiter", httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ip,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
                });

                options.AddPolicy("ChangePassword", httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                         partitionKey: "ChangePasswordGlobal",
                         factory: _ => new FixedWindowRateLimiterOptions
                         {
                         PermitLimit = 50,
                         Window = TimeSpan.FromMinutes(1),

                         QueueLimit = 20,

                         QueueProcessingOrder =
                         QueueProcessingOrder.OldestFirst
                          });
                     });
                 });
            return services;
        }
    }
}
