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
            var info = new ServiceRegistrationInfo
            {
                Address = "localhost",
                Name = "SomeService",
                Port = 10411
            };
            var result = client.RegisterService(info);
            result.Wait();
            Console.WriteLine(result.Result);
            Console.ReadKey();
        }
    }
}
