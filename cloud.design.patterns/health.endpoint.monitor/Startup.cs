namespace Health.Enpoint.Monitor
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            services.AddHealthChecks()
                .AddUrlGroup(new Uri("https://carlos.mendible.com"), "CodeItYourSelf")
                .AddUrlGroup(new Uri("http://localhost:51234/"), "UnhealthyService");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseRouting()
                .UseEndpoints(config =>
                {
                    config.MapHealthChecks("/health");
                });
        }
    }
}
