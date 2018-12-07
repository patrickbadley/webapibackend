using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace FluxTestApi2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.ListenAnyIP(5000);
                    options.ListenAnyIP(5001, listenOptions =>
                    {
                        listenOptions.UseHttps("cert.pfx", "1234");
                    });
                })
            .UseUrls("https://+;http://+");
    }
}
