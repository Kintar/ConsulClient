using System.Collections.Generic;
using ConsulClient.DataTypes;

namespace ConsulClient
{
    public interface IConsulProvider
    {
        bool RegisterService(ServiceRegistrationInfo service);
        bool DeregisterService(string serviceId);
        List<Service> GetServices(string serviceName);
        bool RegisterCheck(CheckRegistrationInfo check);
        bool DeregisterCheck(string checkId);
        List<Check> GetChecks();
        bool UpdateTTLCheck(string checkId, string note, ServiceCheckStatus status);
    }
}