using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public interface IAddAccountPartyRelationship
    {
        IRestRepository RestRepository { get; set; }

        ISoapRepository SoapRepository { get; set; }

        ILmsRepository LmsRepository { get; set; }

        /// <summary>
        /// Holds the response from a CCM SOAP service call.
        /// </summary>
        MessageResponse MessageResponse { get; set; }

        /// <summary>
        /// Creates a new Non-Primary CCM credit card account.
        /// </summary>
        /// <param name="applicant"></param>
        /// <returns></returns>
        BaseResult AddAccountPartyRelationship(Applicant applicant);

        /// <summary>
        /// Factory method that creates an AddAccountPartyRelationship DTO.
        /// </summary>
        /// <param name="applicant"></param>
        /// <returns></returns>
        AddAccountPartyRelationship GetDto(Applicant applicant);
    }
}
