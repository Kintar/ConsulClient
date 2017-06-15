using System.Collections.Generic;
using System.Threading.Tasks;
using ConsulClient.DataTypes;

namespace ConsulClient
{
    public interface IConsulProvider
    {
        Task<bool> RegisterService(ServiceRegistrationInfo service);
        Task<bool> DeregisterService(string serviceId);
        Task<List<Service>> GetServices(string serviceName);
        Task<bool> RegisterCheck(CheckRegistrationInfo check);
        Task<bool> DeregisterCheck(string checkId);
        Task<List<Check>> GetChecks();
        Task<bool> UpdateTTLCheck(string checkId, string note, ServiceCheckStatus status);
    }
}