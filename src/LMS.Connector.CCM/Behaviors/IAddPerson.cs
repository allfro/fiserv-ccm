using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public interface IAddPerson
    {
        IRestRepository RestRepository { get; set; }

        ISoapRepository SoapRepository { get; set; }

        ILmsRepository LmsRepository { get; set; }

        /// <summary>
        /// Holds the response from a CCM SOAP service call.
        /// </summary>
        MessageResponse MessageResponse { get; set; }

        /// <summary>
        /// Creates a Person in CCM. This Person must already exist in DNA/OSI.
        /// </summary>
        /// <param name="lmsPerson"></param>
        /// <returns></returns>
        BaseResult AddPerson(LmsPerson lmsPerson);

        /// <summary>
        /// Factory method that creates an AddPerson DTO.
        /// </summary>
        /// <param name="lmsPerson"></param>
        /// <returns></returns>
        AddPerson GetDto(LmsPerson lmsPerson);

        /// <summary>
        /// Gets the current address of Applicant.
        /// </summary>
        /// <param name="applicant"></param>
        /// <returns></returns>
        Address GetCurrentAddress(Applicant applicant);

        /// <summary>
        /// Gets the current address of Authorized User.
        /// </summary>
        /// <param name="authorizedUser"></param>
        /// <returns></returns>
        Address GetCurrentAddress(AuthorizedUser authorizedUser);

        /// <summary>
        /// Gets the main phone (mobile or home) of Applicant.
        /// </summary>
        /// <param name="applicant"></param>
        /// <returns></returns>
        Phone GetMainPhone(Applicant applicant);

        /// <summary>
        /// Gets the main phone (mobile or home) of Authorized User.
        /// </summary>
        /// <param name="authorizedUser"></param>
        /// <returns></returns>
        Phone GetMainPhone(AuthorizedUser authorizedUser);
    }
}
