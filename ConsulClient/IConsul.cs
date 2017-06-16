using System.Collections.Generic;
using System.Threading.Tasks;
using ConsulClient.DataTypes;

namespace ConsulClient
{
    public interface IConsul
    {
        Task<bool> RegisterServiceAsync(ServiceRegistrationInfo service);
        Task<bool> DeregisterServiceAsync(string serviceId);

        /// <summary>
        /// Finds all services of the given name with a passing check
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="tags"></param>
        /// <param name="requireNonSerfCheck"></param>
        /// <returns></returns>
        Task<IList<Service>> GetServicesAsync(string serviceName, params string[] tags);
        Task<bool> RegisterCheckAsync(CheckRegistrationInfo check);
        Task<bool> DeregisterCheckAsync(string checkId);
        Task<IList<Check>> GetChecksAsync();
        Task<bool> UpdateTTLCheckAsync(string checkId, string output, ServiceCheckStatus status);
    }
}