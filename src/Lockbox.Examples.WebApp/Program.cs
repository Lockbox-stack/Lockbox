﻿using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Lockbox.Examples.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseUrls("http://*:5001")
                .Build();

            host.Run();
        }
    }
}
