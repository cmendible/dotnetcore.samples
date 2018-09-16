namespace EchoServer
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore;

    class Program
    {
        static int Main(string[] args)
        {
            WebHost.CreateDefaultBuilder()
                .UseKestrel()
                .Configure(app =>
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
                    })
                .Build()
                .Run();

            return 0;
        }
    }
}