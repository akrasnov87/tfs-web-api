using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TfsWebAPi
{
    /// <summary>
    /// aHR0cDovL3RmczIwMTcuY29tcHVsaW5rLmxvY2FsOjgwODAvdGZzL0RlZmF1bHRDb2xsZWN0aW9ufElTZXJ2fENvbXB1bGlua3xhLWtyYXNub3Z8JGVjdXJpdHkwfGJkMTZkYjljLWQ5NjItNGU4ZS1hNGUyLWU3ZWQyNjA4YzYyYnw3ODkwNzhlZi02Y2NiLTQzODctYmUzMC03NGI5NTg4MGZkODg=
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
