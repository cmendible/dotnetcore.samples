namespace dapr.k8s.secrets
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Dapr.Client;
    using System;
    using System.Collections.Generic;

    public class Startup
    {
        const string localhost = "127.0.0.1";

        static string daprPort => Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "50001";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Create Dapr Client
            var client = new DaprClientBuilder()
                .UseEndpoint($"https://{localhost}:{daprPort}")
                .Build();

            // Add the DaprClient to DI.
            services.AddSingleton<DaprClient>(client);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DaprClient client)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("secret", Secret);
            });

            async Task Secret(HttpContext context)
            {
                var secretValues = await client.GetSecretAsync(
                    "kubernetes", // Name of the Dapr Secret Store
                    "super-secret", // Name of the k8s secret to get
                    new Dictionary<string, string>() { { "namespace", "default" } }); // Namespace where the k8s secret is deployed

                // Get the secret value
                var secretValue = secretValues["super-secret"];

                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.Body, secretValue);
            }
        }
    }
}