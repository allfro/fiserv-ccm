using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public interface IUpdateAccount
    {
        IRestRepository RestRepository { get; set; }

        ISoapRepository SoapRepository { get; set; }

        ILmsRepository LmsRepository { get; set; }

        /// <summary>
        /// Holds the response from a CCM SOAP service call.
        /// </summary>
        MessageResponse MessageResponse { get; set; }

        /// <summary>
        /// Updates the credit limit on the CCM credit card account.
        /// </summary>
        /// <param name="primaryApplicant"></param>
        /// <returns></returns>
        BaseResult UpdateAccount(Applicant primaryApplicant);

        /// <summary>
        /// Factory method that creates an UpdateAccount DTO.
        /// </summary>
        /// <param name="primaryApplicant"></param>
        /// <returns></returns>
        UpdateAccount GetDto(Applicant primaryApplicant);
    }
}
