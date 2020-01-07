using Akcelerant.Core.Data.DTO.Result;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public interface IAddCard
    {
        IRestRepository RestRepository { get; set; }

        ISoapRepository SoapRepository { get; set; }

        ILmsRepository LmsRepository { get; set; }

        /// <summary>
        /// Holds the response from a CCM SOAP service call.
        /// </summary>
        MessageResponse MessageResponse { get; set; }

        /// <summary>
        /// Makes a call to create cards for the Primary
        /// </summary>
        /// <param name="lmsPerson"></param>
        /// <returns></returns>
        BaseResult AddCard(LmsPerson lmsPerson);

        /// <summary>
        /// Factory method that creates an AddCard DTO.
        /// </summary>
        /// <param name="lmsPerson"></param>
        /// <returns></returns>
        AddCard GetDto(LmsPerson lmsPerson);
    }
}
