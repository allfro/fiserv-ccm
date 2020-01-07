using System;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Core.Rest;

namespace LMS.Connector.CCM.Proxies
{
    /// <summary>
    /// Provides a façade/proxy to the CCM REST services.
    /// </summary>
    public interface IServiceProxy
    {
        Lazy<IAPI> Api { get; set; }

        Credentials Credentials { get; set; }

        string BaseUrl { get; set; }

        /// <summary>
        /// Gets the Authorization header value;
        /// </summary>
        /// <returns></returns>
        string GetAuthorization();

        /// <summary>
        /// Gets an AuthToken from the CCM REST Service.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        string GetAuthToken(Session session);

        //Header GetHeader<T>() where T : class;

        /// <summary>
        /// Calls CCM REST API to retrieve PartyRelationships.
        /// </summary>
        /// <param name="inquiry"></param>
        /// <returns></returns>
        PartyRelationshipsInquiryResponse MakeInquiry(PartyRelationshipsInquiry inquiry);
    }
}
