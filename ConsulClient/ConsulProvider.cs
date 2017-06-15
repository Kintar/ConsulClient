using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsulClient.DataTypes;

namespace ConsulClient
{
    public class ConsulProvider : IConsulProvider
    {
        public string ConsulHost { get; } = "127.0.0.1";
        public int ConsulHttpPort { get; } = 8500;
        public string ServiceHostName { get; }

        public ConsulProvider()
        {
            ServiceHostName = Environment.MachineName;
        }

        public ConsulProvider(string host, int port, string serviceHostName)
        {
            ConsulHost = host;
            ConsulHttpPort = port;
            ServiceHostName = serviceHostName;
        }

        public bool RegisterService(ServiceRegistrationInfo service)
        {
            throw new NotImplementedException();
        }

        public bool DeregisterService(string serviceId)
        {
            throw new NotImplementedException();
        }

        public List<Service> GetServices(string serviceName)
        {
            throw new NotImplementedException();
        }

        public bool RegisterCheck(CheckRegistrationInfo check)
        {
            throw new NotImplementedException();
        }

        public bool DeregisterCheck(string checkId)
        {
            throw new NotImplementedException();
        }

        public List<Check> GetChecks()
        {
            throw new NotImplementedException();
        }

        public bool UpdateTTLCheck(string checkId, string note, ServiceCheckStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
