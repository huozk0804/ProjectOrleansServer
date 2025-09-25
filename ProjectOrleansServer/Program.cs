using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHost(args).Build().Run();
        }

        public static IHostBuilder CreateHost(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);
            builder.UseOrleans(silo =>
            {
                silo.UseLocalhostClustering();
                silo.UseDashboard(option =>
                {
                    option.Port = 10010;
                });
                silo.Configure<ClusterOptions>(option =>
                {
                    option.ClusterId = "clusterId";
                    option.ServiceId = "serviceId";
                });
            });
            return builder;
        }
    }
}