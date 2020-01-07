using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public interface IAddOrganization
    {
        IRestRepository RestRepository { get; set; }

        ISoapRepository SoapRepository { get; set; }

        ILmsRepository LmsRepository { get; set; }

        /// <summary>
        /// Holds the response from a CCM SOAP service call.
        /// </summary>
        MessageResponse MessageResponse { get; set; }

        /// <summary>
        /// Creates an Organization in CCM. This Organization must already exist in DNA/OSI.
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        BaseResult AddOrganization(Applicant organization);

        /// <summary>
        /// Factory method that creates an AddOrganization DTO.
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        AddOrganization GetDto(Applicant organization);

        /// <summary>
        /// Gets the current address of Applicant.
        /// </summary>
        /// <param name="applicant"></param>
        /// <returns></returns>
        Address GetCurrentAddress(Applicant applicant);

        /// <summary>
        /// Gets the main phone (mobile or home) of Applicant.
        /// </summary>
        /// <param name="applicant"></param>
        /// <returns></returns>
        Phone GetMainPhone(Applicant applicant);
    }
}
