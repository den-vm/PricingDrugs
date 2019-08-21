using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ServiceJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 1073741824;  /*ограничение размера запроса отдельных значений формы*/  });
                }
        }
    }