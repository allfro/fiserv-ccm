using Akcelerant.Lending.Data.DTO.Applications;

namespace LMS.Connector.CCM.Models
{
    /// <summary>
    /// Models an LMS Applicant or Authorized User
    /// </summary>
    public class LmsPerson
    {
        public Applicant Applicant { get; set; }
        public AuthorizedUser AuthorizedUser { get; set; }
    }
}
