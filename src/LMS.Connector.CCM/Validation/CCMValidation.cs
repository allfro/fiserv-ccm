using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Validation
{
    public class CCMValidation
    {
        private Application _app;
        private string _userToken;
        private IValidation _validationManager;

        public CCMValidation()
        {
            _app = new Application();
        }

        public CCMValidation(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _validationManager = new ValidationManager(app, userToken, new LmsRepository(userToken));
        }

        public CCMValidation(Application app, string userToken, IValidation validationManager)
        {
            _app = app;
            _userToken = userToken;
            _validationManager = validationManager;
        }

        /// <summary>
        /// Checks for required fields prior to sending the request to the CCM service.
        /// </summary>
        /// <returns></returns>
        public BaseResult DisbursementValidation()
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Validation.CCMValidation.DisbursementValidation"))
            {
                tr.Log("Calling ValidateInquiry");
                var validateInquiryResult = _validationManager.ValidateInquiry();
                result.AppendResult(validateInquiryResult);
                tr.Log($"ValidateInquiry result: {result.Result}");

                tr.Log($"_app.IsAddon = {_app.IsAddon}");
                if (!_app.IsAddon)
                {
                    tr.Log("Calling ValidateAddPerson");
                    var validateAddPersonResult = _validationManager.ValidateAddPerson();
                    result.AppendResult(validateAddPersonResult);
                    tr.Log($"ValidateAddPerson result: {validateAddPersonResult.Result}");

                    tr.Log("Calling ValidateAddOrganization");
                    var validateAddOrganizationResult = _validationManager.ValidateAddOrganization();
                    result.AppendResult(validateAddOrganizationResult);
                    tr.Log($"ValidateAddOrganization result: {validateAddOrganizationResult.Result}");

                    tr.Log("Calling ValidateAddAccount");
                    var validateAddAccountResult = _validationManager.ValidateAddAccount();
                    result.AppendResult(validateAddAccountResult);
                    tr.Log($"ValidateAddAccount result: {validateAddAccountResult.Result}");

                    tr.Log("Calling ValidateAddAccountPartyRelationship");
                    var validateAddAccountPartyRelationship = _validationManager.ValidateAddAccountPartyRelationship();
                    result.AppendResult(validateAddAccountPartyRelationship);
                    tr.Log($"ValidateAddAccountPartyRelationship result: {validateAddAccountPartyRelationship.Result}");

                    tr.Log("Calling ValidateAddCard");
                    var validateAddCardResult = _validationManager.ValidateAddCard();
                    result.AppendResult(validateAddCardResult);
                    tr.Log($"ValidateAddCard result: {validateAddCardResult.Result}");
                }
                else
                {
                    tr.Log("Calling ValidateAddPerson");
                    var validateAddPersonResult = _validationManager.ValidateAddPerson();
                    result.AppendResult(validateAddPersonResult);
                    tr.Log($"ValidateAddPerson result: {validateAddPersonResult.Result}");

                    tr.Log("Calling ValidateAddOrganization");
                    var validateAddOrganizationResult = _validationManager.ValidateAddOrganization();
                    result.AppendResult(validateAddOrganizationResult);
                    tr.Log($"ValidateAddOrganization result: {validateAddOrganizationResult.Result}");

                    tr.Log("Calling ValidateUpdateAccount");
                    var validateUpdateAccountResult = _validationManager.ValidateUpdateAccount();
                    result.AppendResult(validateUpdateAccountResult);
                    tr.Log($"ValidateUpdateAccount result: {validateUpdateAccountResult.Result}");

                    tr.Log("Calling ValidateAddAccountPartyRelationship");
                    var validateAddAccountPartyRelationshipResult = _validationManager.ValidateAddAccountPartyRelationship();
                    result.AppendResult(validateAddAccountPartyRelationshipResult);
                    tr.Log($"ValidateAddAccountPartyRelationship result: {validateAddAccountPartyRelationshipResult.Result}");

                    tr.Log("Calling ValidateAddCard");
                    var validateAddCardResult = _validationManager.ValidateAddCard();
                    result.AppendResult(validateAddCardResult);
                    tr.Log($"ValidateAddCard result: {validateAddCardResult.Result}");
                }
            }

            return result;
        }
    }
}
