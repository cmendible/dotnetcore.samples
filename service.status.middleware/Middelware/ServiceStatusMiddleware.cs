namespace WebApplication
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    /// <summary>
    /// Service Status Middleware used to check the Health of your service.
    /// </summary>
    public class ServiceStatusMiddleware
    {
        /// <summary>
        /// Next request RequestDelegate
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// Health check function.
        /// </summary>
        private readonly Func<Task<bool>> serviceStatusCheck;

        /// <summary>
        /// ServiceStatus enpoint path 
        /// </summary>
        private static readonly PathString statePath = new PathString("/_check");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">Request delegate.</param>
        /// <param name="serviceStatusCheck">The health check function</param>
        public ServiceStatusMiddleware(RequestDelegate next, Func<Task<bool>> serviceStatusCheck)
        {
            this.next = next;
            this.serviceStatusCheck = serviceStatusCheck;
        }

        /// <summary>
        /// Where the middleware magic happens
        /// </summary>
        /// <param name="httpContext">The httpContext</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            // If the path is different from the statePath let the request through the normal pipeline.
            if (!httpContext.Request.Path.Equals(statePath))
            {
                await this.next.Invoke(httpContext);
            }
            else
            {
                // If the path is statePath call the health check function.
                await CheckAsync(httpContext);
            }
        }

        /// <summary>
        /// Call the health check function and set the response.
        /// </summary>
        /// <param name="httpContext">The httpContext</param>
        /// <returns>Task</returns>
        private async Task CheckAsync(HttpContext httpContext)
        {
            if (await this.serviceStatusCheck().ConfigureAwait(false))
            {
                // Service is available.
                await WriteResponseAsync(httpContext, HttpStatusCode.OK, new ServiceStatus(true));
            }
            else
            {
                // Service is unavailable.
                await WriteResponseAsync(httpContext, HttpStatusCode.ServiceUnavailable, new ServiceStatus(false));
            }
        }

        /// <summary>
        /// Writes a response of the Service Status Check.
        /// </summary>
        /// <param name="httpContext">The HttpContext.</param>
        /// <param name="httpStatusCode">The http status to return.!--.</param>
        /// <param name="serviceStatus">The status</param>
        /// <returns>Task</returns>
        private Task WriteResponseAsync(HttpContext httpContext, HttpStatusCode httpStatusCode, ServiceStatus serviceStatus)
        {
            // Set content type.
            httpContext.Response.Headers["Content-Type"] = new[] { "application/json" };

            // Minimum set of headers to disable caching of the response.
            httpContext.Response.Headers["Cache-Control"] = new[] { "no-cache, no-store, must-revalidate" };
            httpContext.Response.Headers["Pragma"] = new[] { "no-cache" };
            httpContext.Response.Headers["Expires"] = new[] { "0" };

            // Set status code.
            httpContext.Response.StatusCode = (int)httpStatusCode;

            // Write the content.
            var content = JsonConvert.SerializeObject(serviceStatus);
            return httpContext.Response.WriteAsync(content);
        }
    }

    /// <summary>
    /// ServiceStatus to hold the response. 
    /// </summary>
    public class ServiceStatus
    {
        public ServiceStatus(bool available)
        {
            Available = available;
        }

        /// <summary>
        /// Tells if the service is available
        /// </summary>
        /// <returns>True if the service is available</returns>
        public bool Available { get; }
    }

    /// <summary>
    /// Service Status Middleware Extensions
    /// </summary>
    public static class ServiceStatusMiddlewareExtensions
    {
        public static IApplicationBuilder UseServiceStatus(
          this IApplicationBuilder app,
          Func<Task<bool>> serviceStatusCheck)
        {
            app.UseMiddleware<ServiceStatusMiddleware>(serviceStatusCheck);

            return app;
        }
    }

}