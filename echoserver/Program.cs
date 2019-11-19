using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

namespace EchoServer
{
    class Program
    {
        static int Main(string[] args)
        {
            Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.Configure(app =>
                    {
                        app.Run(httpContext =>
                        {
                            var request = httpContext.Request;
                            var response = httpContext.Response;

                            // Echo the Headers 
                            foreach (var header in request.Headers)
                            {
                                response.Headers.Add(header);
                            }

                            // Echo the body
                            return request.Body.CopyToAsync(response.Body);

                        });
                    });
                })
                .Build()
                .Run();

            return 0;
        }
    }
}