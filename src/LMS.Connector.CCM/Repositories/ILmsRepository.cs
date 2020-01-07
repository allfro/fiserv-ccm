using Akcelerant.Core;
using Akcelerant.Core.Lookups;
using LMS.Connector.CCM.Models;

namespace LMS.Connector.CCM.Repositories
{
    /// <summary>
    /// Provides a repository for interacting with LMS data stores.
    /// </summary>
    public interface ILmsRepository
    {
        string UserToken { get; set; }

        LookupComponent Lookup { get; set; }

        Parameters Parameters { get; set; }

        /// <summary>
        /// Gets the First and Last Name associated with the given UserId.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUserFullNameById(int userId);

        /// <summary>
        /// Retrieves a lookup code based on a lookup id.
        /// </summary>
        /// <param name="lookupId"></param>
        /// <param name="lookupTypes"></param>
        /// <returns></returns>
        string GetLookupCodeById(int lookupId, LookupTypes lookupTypes);

        /// <summary>
        /// Retrieves a lookup id based on lookup type and lookup code.
        /// </summary>
        /// <param name="lookupType">
        /// Specify a lookup type using the LookupType Enum.
        /// Add members to the LookupTypes Enum if the lookup type you require is not in the Enum.
        /// </param>
        /// <param name="lookupCode">
        /// Specify a lookup code using a string.  Use the constants from LookupCodes to avoid hardcoding literal strings.
        /// Add members to the LookupCodes subclasses if the lookup code you require is not in the constants.
        /// </param>
        /// <returns></returns>
        int GetLookupIdByTypeAndCode(LookupTypes lookupType, string lookupCode);

        /// <summary>
        /// Retrieves a lookup Desc based on a lookup id.
        /// </summary>
        /// <param name="lookupId"></param>
        /// <param name="lookupType"></param>
        /// <returns></returns>
        string GetLookupDescById(int lookupId, LookupTypes lookupType);

        /// <summary>
        /// Gets the service credentials from the LMS data store.
        /// </summary>
        /// <param name="solutionCode"></param>
        /// <param name="serviceUrlParameterCode"></param>
        /// <returns></returns>
        Credentials GetServiceCredentials(string solutionCode, string serviceUrlParameterCode);

        /// <summary>
        /// Retrieves the phone type code from the LMS data store.
        /// </summary>
        /// <param name="phoneTypeId"></param>
        /// <returns></returns>
        string GetPhoneType(int phoneTypeId);
    }
}
