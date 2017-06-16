using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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
            var client = new Consul();

            var info = new ServiceRegistrationInfo
            {
                Address = "localhost",
                Name = "SomeService",
                ID = "SomeService",
                Port = 1014,
                Tags = new[] { "v1" }
            };

            var checks = new[] {
                new CheckRegistrationInfo {
                    Name = "SomeService_AliveCheck",
                    DeregisterCriticalServiceAfter = "1m",
                    TTL = "30s",
                    Status = ServiceCheckStatus.Passing,
                    ServiceID = "SomeService",
                    CheckID = "service:SomeService_AliveCheck" }
            };

            info.Checks = checks;
            client.RegisterServiceAsync(info).Wait();

            //client.DeregisterCheckAsync("service:SomeService_OtherCheck").Wait();
            //client.DeregisterCheckAsync("service:SomeService_AliveCheck").Wait();
            //client.DeregisterServiceAsync("SomeService").Wait();

            //var task = client.GetChecksAsync();
            //task.Wait();

            //foreach (var svc in task.Result)
            //{
            //    Console.WriteLine($"{svc.CheckID}");
            //}

            //Console.ReadKey();
        }
    }
}
