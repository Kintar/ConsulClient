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
            client.DeregisterService("SomeService").Wait();
        }
    }
}
