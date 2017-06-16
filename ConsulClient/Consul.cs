using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConsulClient.DataTypes;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ConsulClient
{
    public class Consul : IConsul
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Consul));
        protected static readonly HttpClient Client = new HttpClient();
        protected static readonly JsonSerializer Serializer = JsonSerializer.CreateDefault();
        protected static readonly MediaTypeHeaderValue MediaJson = new MediaTypeHeaderValue("application/json");

        public string ConsulHost { get; } = "127.0.0.1";
        public int ConsulHttpPort { get; } = 8500;
        public string ServiceHostName { get; }

        public Consul()
        {
            ServiceHostName = Environment.MachineName;
            InitClient();
            InitSerializer();
        }

        public Consul(string host, int port, string serviceHostName)
        {
            ConsulHost = host;
            ConsulHttpPort = port;
            ServiceHostName = serviceHostName;
            InitClient();
            InitSerializer();
        }

        private static void InitSerializer()
        {
            Serializer.NullValueHandling = NullValueHandling.Ignore;
            Serializer.Converters.Add(new StringEnumConverter(true));
        }

        private void InitClient()
        {
            Client.BaseAddress = new Uri($"http://{ConsulHost}:{ConsulHttpPort}");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        protected string SerializeJson(object obj)
        {
            if (obj == null) return "";

            using (var sw = new StringWriter())
            {
                Serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        private async Task<bool> HandleResponse(Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            var responseText = await response.Content.ReadAsStringAsync();
            Log.Warn($"{response.RequestMessage.RequestUri} => {response.RequestMessage.Method} failed : ({(int)response.StatusCode}) {responseText}");
            return false;
        }

        private async Task<bool> PutJsonAsync(string path, object obj)
        {
            var content = new StringContent(SerializeJson(obj));
            content.Headers.ContentType = MediaJson;
            return await HandleResponse(Client.PutAsync(path, content));
        }
        
        public async Task<bool> RegisterServiceAsync(ServiceRegistrationInfo service)
        {
            return await PutJsonAsync("/v1/agent/service/register", service);
        }

        public async Task<bool> DeregisterServiceAsync(string serviceId)
        {
            var url = $"/v1/agent/service/deregister/{serviceId}";
            return await PutJsonAsync(url, null);
        }

        public async Task<IList<Service>> GetServicesAsync(string serviceName, params string[] tags)
        {
            var url = $"/v1/health/service/{serviceName}?passing";

            var result = await Client.GetStringAsync(url);
            
            var arry = JArray.Parse(result);
            
            var services = arry
                .Where(entry =>
                {
                    if (tags.Length == 0) return true;

                    var entryTags = entry["Service"]["Tags"].Select(i => i.Value<string>()).ToArray();
                    return tags.All(entryTags.Contains);
                })
                .Select(entry => new Service
            {
                Name = entry["Service"]["Service"].ToString(),
                Address = entry["Service"]["Address"].ToString(),
                Port = Convert.ToInt16(entry["Service"]["Port"]),
                Tags = entry["Service"]["Tags"].Select(i => i.Value<string>()).ToArray(),
                ID = entry["Service"]["ID"].ToString()
                }).ToList();
            
            return services;
        }

        public async Task<bool> RegisterCheckAsync(CheckRegistrationInfo check)
        {
            return await PutJsonAsync("/v1/agent/check/register", check);
        }

        public async Task<bool> DeregisterCheckAsync(string checkId)
        {
            var url = $"/v1/agent/check/deregister/{checkId}";
            return await PutJsonAsync(url, null);
        }

        public async Task<IList<Check>> GetChecksAsync()
        {
            var result = await Client.GetStringAsync($"/v1/health/node/{ServiceHostName}");
            
            var arry = JArray.Parse(result);

            return arry.Select(entry => new Check {
                Node = entry["Node"].ToString(),
                CheckID = entry["CheckID"].ToString(),
                Name = entry["Name"].ToString(),
                Notes = entry["Notes"].ToString(),
                Output = entry["Output"].ToString(),
                ServiceID = entry["ServiceID"].ToString(),
                ServiceName = entry["ServiceName"].ToString(),
                Status = ServiceCheckStatusConverter.FromString(entry["Status"].ToString())
            }).ToList();
        }

        public async Task<bool> UpdateTTLCheckAsync(string checkId, string output, ServiceCheckStatus status)
        {
            var url = $"/v1/agent/check/update/{checkId}";

            return await PutJsonAsync(url, new {
                Status = status.ToString().ToLower(),
                Output = output
            });
        }
    }
}
