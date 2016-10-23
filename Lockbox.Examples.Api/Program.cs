using System.IO;
using Lockbox.Api;
using Microsoft.AspNetCore.Hosting;

namespace Lockbox.Examples.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<LockboxStartup>()
                .UseUrls("http://*:5000")
                .UseIISIntegration()
                .Build();

            host.Run();
        }
    }
}
