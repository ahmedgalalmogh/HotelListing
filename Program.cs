using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = (Serilog.ILogger)new LoggerConfiguration().WriteTo.File(path: "C:\\Users\\20111\\source\\repos\\HotelListing\\logs\\log-.txt",
               outputTemplate: "{timstamp:yyy-MM-dd HH:mm:ss.fff zzz}[{Level:u3}] {Message:lj}{NewLine}{Exception}",
               rollingInterval: RollingInterval.Day,
              restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information).CreateLogger();

            try
            {
                Log.Information("APPLICATION started");                
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "faild to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
