using System.Collections.Generic;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Proxies;

namespace LMS.Connector.CCM.Repositories
{
    /// <summary>
    /// Provides a repository for interacting with the CCM REST service proxy.
    /// </summary>
    public interface IRestRepository : IServiceRepository
    {
        IServiceProxy Proxy { get; set; }

        /**********
         * Delete/Modify these signatures BELOW as necessary when we're ready for their real implementation
         **/

        //string AddAccount(AddAccount account);

        //string AddAccountPartyRelationship(AddAccountPartyRelationship accountPartyRelationship);

        //string AddCard(AddCard card);

        //string AddPerson(AddPerson person);

        //string AddOrganization(AddOrganization organization);

        //string UpdateAccount(UpdateAccount account);

        //string UpdatePerson(UpdatePerson person);

        /*
         * Delete/Modify these signatures ABOVE as necessary when we're ready for their real implementation
         **********/

        /// <summary>
        /// Gets list of RelationshipInfo objects from CCM.
        /// </summary>
        /// <param name="inquiry"></param>
        /// <returns></returns>
        IList<RelationshipInfo> MakeInquiry(PartyRelationshipsInquiry inquiry);

        /// <summary>
        /// Tests connectivity to CCM REST service.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        bool TestConnection(Session session);

        /// <summary>
        /// Gets an AuthToken from the CCM REST Service.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        string GetAuthToken(Session session);
    }
}
