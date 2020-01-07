using Akcelerant.Core.Data.DTO.Result;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public interface ITestConnection
    {
        IRestRepository RestRepository { get; set; }

        ISoapRepository SoapRepository { get; set; }

        ILmsRepository LmsRepository { get; set; }

        /// <summary>
        /// Holds the response from a CCM SOAP service call.
        /// </summary>
        MessageResponse MessageResponse { get; set; }

        bool ConnectionEstablished { get; set; }

        /// <summary>
        /// Tests the connection to a CCM service.
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="facility"></param>
        /// <returns></returns>
        BaseResult TestConnection(string serviceUrl, string userName, string password, string facility);
    }
}
