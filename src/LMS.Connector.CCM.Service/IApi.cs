using System.Collections.Generic;
using System.ServiceModel;
using Akcelerant.Core.Data.DTO.Host;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;

namespace LMS.Connector.CCM.Service
{
    /// <summary>
    /// Defines all behaviors specific to calling the CCM connector from the service layer.
    /// </summary>
    [ServiceContract]
    interface IApi
    {
        Application Application { get; set; }

        string UserToken { get; set; }

        /// <summary>
        /// Tests the connectivity to the CCM SOAP and REST services.
        /// </summary>
        /// <param name="connectorParams"></param>
        /// <returns></returns>
        [OperationContract]
        BaseResult TestConnections(IList<HostParameter> connectorParams);
    }
}
