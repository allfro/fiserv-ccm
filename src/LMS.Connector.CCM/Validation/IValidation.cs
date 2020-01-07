using Akcelerant.Core.Data.DTO.Result;

namespace LMS.Connector.CCM.Validation
{
    public interface IValidation
    {
        BaseResult BaseResult { get; set; }

        BaseResult ValidateInquiry();

        BaseResult ValidateAddPerson();

        BaseResult ValidateAddOrganization();

        BaseResult ValidateAddAccount();

        BaseResult ValidateAddAccountPartyRelationship();

        BaseResult ValidateAddCard();

        BaseResult ValidateUpdateAccount();

        BaseResult ValidateUpdatePerson();

        /// <summary>
        /// Outputs a generic error message for Rules Only fields that require validation.
        /// </summary>
        /// <param name="fullHostValuePath">Full path to host value</param>
        /// <param name="category">Rule category that determines when a host value gets it value</param>
        /// <returns></returns>
        string GetRulesOnlyErrorMessage(string fullHostValuePath, string category);
    }
}
