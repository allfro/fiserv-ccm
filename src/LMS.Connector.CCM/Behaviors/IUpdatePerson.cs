using Akcelerant.Core.Data.DTO.Result;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public interface IUpdatePerson
    {
        IRestRepository RestRepository { get; set; }

        ISoapRepository SoapRepository { get; set; }

        ILmsRepository LmsRepository { get; set; }

        /// <summary>
        /// Holds the response from a CCM SOAP service call.
        /// </summary>
        MessageResponse MessageResponse { get; set; }

        /// <summary>
        /// Updates a Person in CCM. This Person must already exist in DNA/OSI.
        /// </summary>
        /// <param name="lmsPerson"></param>
        /// <returns></returns>
        BaseResult UpdatePerson(LmsPerson lmsPerson);

        /// <summary>
        /// Factory method that creates an UpdatePerson DTO.
        /// </summary>
        /// <param name="lmsPerson"></param>
        /// <returns></returns>
        UpdatePerson GetDto(LmsPerson lmsPerson);
    }
}
