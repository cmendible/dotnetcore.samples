using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace aspnet_core.https.docker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            // Calling UseKestrel to set https configuration
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options => 
            {
                options.Listen(IPAddress.Any, 443, listenOptions =>
                {
                    var configuration = (IConfiguration)options.ApplicationServices.GetService(typeof(IConfiguration));

                    listenOptions.UseHttps("cert.pfx", configuration["certPassword"]);
                });
            })
            .UseStartup<Startup>()
            .Build();
    }
}
