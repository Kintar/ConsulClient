using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsulClient;
using ConsulClient.DataTypes;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ConsulProvider();
            //var info = new ServiceRegistrationInfo
            //{
            //    Address = "localhost",
            //    Name = "SomeService",
            //    ID = "SomeService",
            //    Port = 1014
            //};
            //client.RegisterService(info).Wait();
            var services = client.GetServices("SomeService");
            services.Wait();

            foreach (var item in services.Result)
            {
                Console.WriteLine($"Name: {item.Name}@{item.Address}:{item.Port}");
            }
            Console.ReadKey();
        }
    }
}
