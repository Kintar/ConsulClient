using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConsulClient.DataTypes;
using Flurl;
using Flurl.Http;

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

        private string BaseConsulUrl()
        {
            return $"http://{ConsulHost}:{ConsulHttpPort}/v1/agent";
        }

        public async Task<bool> RegisterService(ServiceRegistrationInfo service)
        {
            var url = $"{BaseConsulUrl()}/service/register";

            var result = await url.PostJsonAsync(service);
            
            return result.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> DeregisterService(string serviceId)
        {
            var url = $"{BaseConsulUrl()}/service/deregister/{serviceId}";

            var result = await url.PutAsync(null);

            return result.StatusCode == HttpStatusCode.OK;
        }

        public async Task<List<Service>> GetServices(string serviceName)
        {
            var url = $"{BaseConsulUrl()}/services";

            throw new NotImplementedException();
        }

        public async Task<bool> RegisterCheck(CheckRegistrationInfo check)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeregisterCheck(string checkId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Check>> GetChecks()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTTLCheck(string checkId, string note, ServiceCheckStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
