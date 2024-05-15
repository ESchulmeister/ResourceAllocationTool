using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ResourceAllocationTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)

           .ConfigureLogging(logging =>
           {
               logging.AddLog4Net(new Log4NetProviderOptions("log4net.config"));
           })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseSetting("https_port", "443");   //URL Rewrite - HTTP - > HTTPS
                webBuilder.SuppressStatusMessages(true);//disable the status messages
                webBuilder.UseStartup<Startup>();
            });
        }
    }
}