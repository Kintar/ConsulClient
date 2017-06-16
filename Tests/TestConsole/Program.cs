using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsulClient;
using ConsulClient.DataTypes;
using log4net.Config;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            BasicConfigurator.Configure();
            var client = new ConsulProvider();

            var info = new ServiceRegistrationInfo
            {
                Address = "localhost",
                Name = "SomeService",
                ID = "SomeService",
                Port = 1014
            };

            var check = new CheckRegistrationInfo
            {
                Name = "SomeService_AliveCheck",
                DeregisterCriticalServiceAfter = "1m",
                HTTP = "http://localhost:1014/someservice/health_check",
                ServiceID = "SomeService",
                Interval = "15s",
                CheckID = "service:SomeService_HealthCheck"
            };

            info.Check = check;
            var task = client.RegisterServiceAsync(info);
            task.Wait();
            if (task.Result)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Failure!");
            }
            
            Console.ReadKey();
        }
    }
}
