using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConsulClient.DataTypes;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;

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

        private string BaseAgentUrl()
        {
            return $"http://{ConsulHost}:{ConsulHttpPort}/v1/agent";
        }

        private string BaseHealthUrl()
        {
            return $"http://{ConsulHost}:{ConsulHttpPort}/v1/health";
        }

        public async Task<bool> RegisterService(ServiceRegistrationInfo service)
        {
            var url = $"{BaseAgentUrl()}/service/register";

            var result = await url.PostJsonAsync(service);
            
            return result.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> DeregisterService(string serviceId)
        {
            var url = $"{BaseAgentUrl()}/service/deregister/{serviceId}";

            var result = await url.PutAsync(null);

            return result.StatusCode == HttpStatusCode.OK;
        }

        public async Task<IList<Service>> GetServices(string serviceName)
        {
            var url = $"{BaseHealthUrl()}/service/{serviceName}?passing";

            var results = await url.GetStringAsync();

            var arry = JArray.Parse(results);
            var services = arry.Select(entry => new Service
            {
                Name = entry["Service"]["Service"].ToString(),
                Address = entry["Service"]["Address"].ToString(),
                Port = Convert.ToInt16(entry["Service"]["Port"])
            }).ToList();
            
            return services;
        }

        public async Task<bool> RegisterCheck(CheckRegistrationInfo check)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeregisterCheck(string checkId)
        {
            var url = $"{BaseAgentUrl()}/check/deregister/{checkId}";

            var result = await url.PutAsync(null);

            return result.StatusCode == HttpStatusCode.OK;
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
