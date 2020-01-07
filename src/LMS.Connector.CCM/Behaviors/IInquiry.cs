using System.Collections.Generic;
using Akcelerant.Core.Data.DTO.Result;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors
{
    public interface IInquiry
    {
        IRestRepository RestRepository { get; set; }

        ISoapRepository SoapRepository { get; set; }

        ILmsRepository LmsRepository { get; set; }

        /// <summary>
        /// Holds the RelationshipInfos for a given PersonNumber.
        /// </summary>
        IList<RelationshipInfo> RelationshipInfos { get; set; }

        /// <summary>
        /// The resulting error message after an "Inquiry" call to CCM REST service.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Determines if each Applicant (Primary, Joint(s), or Guarantor(s)) and Authorized User(s) exists in CCM.
        /// </summary>
        /// <remarks>Person/Organization number must exist on the Application</remarks>
        /// <param name="personNumber"></param>
        /// <returns></returns>
        BaseResult Inquiry(string personNumber);

        /// <summary>
        /// Factory method that creates a PartyRelationshipsInquiry DTO.
        /// </summary>
        /// <param name="personNumber"></param>
        /// <returns></returns>
        PartyRelationshipsInquiry GetDto(string personNumber);
    }
}
