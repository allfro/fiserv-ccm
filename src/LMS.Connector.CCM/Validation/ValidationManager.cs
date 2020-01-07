using System;
using System.Linq;
using Akcelerant.Core;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Core.Lookups;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Validation
{
    public class ValidationManager : IValidation
    {
        private Application _app;
        private string _userToken;
        private ILmsRepository _lmsRepository;
        private BaseResult _baseResult;

        public BaseResult BaseResult
        {
            get
            {
                return _baseResult;
            }
            set
            {
                _baseResult = value;
            }
        }

        public ValidationManager(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _lmsRepository = new LmsRepository(userToken);
            _baseResult = new BaseResult();
        }

        public ValidationManager(Application app, string userToken, ILmsRepository lmsRepository)
        {
            _app = app;
            _userToken = userToken;
            _lmsRepository = lmsRepository;
            _baseResult = new BaseResult();
        }

        public BaseResult ValidateInquiry()
        {
            var result = new BaseResult();

            try
            {
                foreach (var applicant in _app.Applicants)
                {
                    if (string.IsNullOrWhiteSpace(applicant.PersonNumber))
                    {
                        result.AddError("The Application > Applicants > Person Number is required");
                        return result;
                    }
                }

                foreach (var authorizedUser in _app.AuthorizedUsers)
                {
                    if (string.IsNullOrWhiteSpace(authorizedUser.PersonNumber))
                    {
                        result.AddError("The Application > Authorized User > Person Number is required");
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                result.AddError("Exception caught in LMS.Connector.CCM.Validation.ValidationManager.ValidateInquiry. See error log for more details.");
                Utility.LogError(ex, "LMS.Connector.CCM.Validation.ValidationManager.ValidateInquiry");
            }

            return result;
        }

        public BaseResult ValidateAddPerson()
        {
            var result = new BaseResult();

            try
            {
                if (_app.ApplicationId > 0)
                {
                    if (string.IsNullOrWhiteSpace(_app.ApplicationId.ToString()))
                        result.AddError("The Application > Application Id is required");

                    var persons = _app.Applicants.Where(a => !a.IsOrganization);

                    if (persons?.Any() == true)
                    {
                        foreach (var applicant in persons)
                        {
                            // Applicant
                            if (string.IsNullOrWhiteSpace(applicant.PersonNumber))
                                result.AddError("The Application > Applicants > Person Number is required");

                            if (string.IsNullOrWhiteSpace(applicant.LastName))
                                result.AddError("The Application > Applicants > Last Name is required");

                            if (string.IsNullOrWhiteSpace(applicant.FirstName))
                                result.AddError("The Application > Applicants > First Name is required");

                            var currentAddress = applicant.Addresses.FirstOrDefault(
                                a => a.AddressTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.AddressType, LookupCodes.AddressType.Current)
                            );

                            if (currentAddress != null)
                            {
                                if (string.IsNullOrWhiteSpace(currentAddress.Address1))
                                    result.AddError("The Application > Applicants > Addresses > Address 1 is required");

                                if (string.IsNullOrWhiteSpace(currentAddress.City))
                                    result.AddError("The Application > Applicants > Addresses > City is required");

                                if (!currentAddress.StateId.HasValue)
                                    result.AddError("The Application > Applicants > Addresses > State is required");

                                if (string.IsNullOrWhiteSpace(currentAddress.PostalCode))
                                    result.AddError("The Application > Applicants > Addresses > Postal Code is required");
                            }

                            var phoneMobile = applicant.Phones.FirstOrDefault(
                                p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Mobile)
                            );
                            var phoneHome = applicant.Phones.FirstOrDefault(
                                p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Home)
                            );
                            if (phoneMobile != null)
                            {
                                var cityAreaCodeHV = phoneMobile.HostValues.Where(
                                    hv => hv.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.CityAreaCode", StringComparison.InvariantCulture)
                                );
                                var localPhoneNumberHV = phoneMobile.HostValues.Where(
                                    hv => hv.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.LocalPhoneNumber", StringComparison.InvariantCulture)
                                );

                                if (cityAreaCodeHV?.Any() == true)
                                {
                                    if (!string.IsNullOrWhiteSpace(cityAreaCodeHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneMobile.PhoneNumber))
                                    {
                                        result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                    }
                                }
                                else if (localPhoneNumberHV?.Any() == true)
                                {
                                    if (!string.IsNullOrWhiteSpace(localPhoneNumberHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneMobile.PhoneNumber))
                                    {
                                        result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                    }
                                }
                            }
                            else if (phoneHome != null && phoneMobile == null)
                            {
                                var cityAreaCodeHV = phoneHome.HostValues.Where(
                                    hv => hv.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.CityAreaCode", StringComparison.InvariantCulture)
                                );
                                var localPhoneNumberHV = phoneHome.HostValues.Where(
                                    hv => hv.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.LocalPhoneNumber", StringComparison.InvariantCulture)
                                );

                                if (cityAreaCodeHV?.Any() == true)
                                {
                                    if (!string.IsNullOrWhiteSpace(cityAreaCodeHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneHome.PhoneNumber))
                                    {
                                        result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                    }
                                }
                                else if (localPhoneNumberHV?.Any() == true)
                                {
                                    if (!string.IsNullOrWhiteSpace(localPhoneNumberHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneHome.PhoneNumber))
                                    {
                                        result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                    }
                                }
                            }

                            if (!result.Result)
                            {
                                _baseResult.AppendResult(result);

                                return result;
                            }
                        }
                    }

                    foreach (var authUser in _app.AuthorizedUsers)
                    {
                        // Authorized User
                        if (string.IsNullOrWhiteSpace(authUser.PersonNumber))
                            result.AddError("The Application > Authorized User > Person Number is required");

                        if (string.IsNullOrWhiteSpace(authUser.LastName))
                            result.AddError("The Application > Authorized User > Last Name is required");

                        if (string.IsNullOrWhiteSpace(authUser.FirstName))
                            result.AddError("The Application > Authorized User > First Name is required");

                        var currentAddress = authUser.Addresses.FirstOrDefault(
                            a => a.AddressTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.AddressType, LookupCodes.AddressType.Current)
                        );

                        if (currentAddress != null)
                        {
                            if (string.IsNullOrWhiteSpace(currentAddress.Address1))
                                result.AddError("The Application > Authorized Users > Addresses > Address 1 is required");

                            if (string.IsNullOrWhiteSpace(currentAddress.City))
                                result.AddError("The Application > Authorized Users > Addresses > City is required");

                            if (!currentAddress.StateId.HasValue)
                                result.AddError("The Application > Authorized Users > Addresses > State is required");

                            if (string.IsNullOrWhiteSpace(currentAddress.PostalCode))
                                result.AddError("The Application > Authorized Users > Addresses > Postal Code is required");
                        }

                        var phoneMobile = authUser.Phones.FirstOrDefault(
                            p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Mobile)
                        );
                        var phoneHome = authUser.Phones.FirstOrDefault(
                            p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Home)
                        );
                        if (phoneMobile != null)
                        {
                            var cityAreaCodeHV = phoneMobile.HostValues.Where(
                                hv => hv.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.CityAreaCode", StringComparison.InvariantCulture)
                            );
                            var localPhoneNumberHV = phoneMobile.HostValues.Where(
                                hv => hv.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.LocalPhoneNumber", StringComparison.InvariantCulture)
                            );

                            if (cityAreaCodeHV?.Any() == true)
                            {
                                if (!string.IsNullOrWhiteSpace(cityAreaCodeHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneMobile.PhoneNumber))
                                {
                                    result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                }
                            }
                            else if (localPhoneNumberHV?.Any() == true)
                            {
                                if (!string.IsNullOrWhiteSpace(localPhoneNumberHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneMobile.PhoneNumber))
                                {
                                    result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                }
                            }
                        }
                        else if (phoneHome != null && phoneMobile == null)
                        {
                            var cityAreaCodeHV = phoneHome.HostValues.Where(
                                hv => hv.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.CityAreaCode", StringComparison.InvariantCulture)
                            );
                            var localPhoneNumberHV = phoneHome.HostValues.Where(
                                hv => hv.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.LocalPhoneNumber", StringComparison.InvariantCulture)
                            );

                            if (cityAreaCodeHV?.Any() == true)
                            {
                                if (!string.IsNullOrWhiteSpace(cityAreaCodeHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneHome.PhoneNumber))
                                {
                                    result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                }
                            }
                            else if (localPhoneNumberHV?.Any() == true)
                            {
                                if (!string.IsNullOrWhiteSpace(localPhoneNumberHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneHome.PhoneNumber))
                                {
                                    result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                }
                            }
                        }

                        if (!result.Result)
                        {
                            _baseResult.AppendResult(result);

                            return result;
                        }
                    }
                }
                else
                {
                    result.AddError("The Application > Application Id is required");
                }
            }
            catch (ArgumentNullException ane)
            {
                result.AddError($"Source or predicate is null: {ane.ParamName}");
                Utility.LogError(ane, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddPerson");
            }
            catch (InvalidOperationException ioe)
            {
                result.AddError($"The source sequence is empty or more than one element satisfies the condition in predicate: {ioe.Source}");
                Utility.LogError(ioe, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddPerson");
            }
            catch (Exception ex)
            {
                result.AddError("Exception caught in LMS.Connector.CCM.Validation.ValidationManager.ValidateAddPerson. See error log for more details.");
                Utility.LogError(ex, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddPerson");
            }

            _baseResult.AppendResult(result);

            return result;
        }

        public BaseResult ValidateAddOrganization()
        {
            var result = new BaseResult();

            try
            {
                if (_app.ApplicationId > 0)
                {
                    if (string.IsNullOrWhiteSpace(_app.ApplicationId.ToString()))
                        result.AddError("The Application > Application Id is required");

                    var organizations = _app.Applicants.Where(a => a.IsOrganization);

                    if (organizations?.Any() == true)
                    {
                        foreach (var organization in organizations)
                        {
                            // Applicant
                            if (string.IsNullOrWhiteSpace(organization.PersonNumber))
                                result.AddError("The Application > Applicants > Person Number is required");

                            if (string.IsNullOrWhiteSpace(organization.OrganizationName))
                                result.AddError("The Application > Applicants > Organization Name is required");

                            if (string.IsNullOrWhiteSpace(organization.Email))
                                result.AddError("The Application > Applicants > Email is required");

                            var currentAddress = organization.Addresses.FirstOrDefault(
                                a => a.AddressTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.AddressType, LookupCodes.AddressType.Current)
                            );

                            if (currentAddress != null)
                            {
                                if (string.IsNullOrWhiteSpace(currentAddress.Address1))
                                    result.AddError("The Application > Applicants > Addresses > Address 1 is required");

                                if (string.IsNullOrWhiteSpace(currentAddress.City))
                                    result.AddError("The Application > Applicants > Addresses > City is required");

                                if (!currentAddress.StateId.HasValue)
                                    result.AddError("The Application > Applicants > Addresses > State is required");

                                if (string.IsNullOrWhiteSpace(currentAddress.PostalCode))
                                    result.AddError("The Application > Applicants > Addresses > Postal Code is required");
                            }

                            var phoneMobile = organization.Phones.FirstOrDefault(
                                p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Mobile)
                            );
                            var phoneHome = organization.Phones.FirstOrDefault(
                                p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Home)
                            );
                            if (phoneMobile != null)
                            {
                                var cityAreaCodeHV = phoneMobile.HostValues.Where(
                                    hv => hv.Field1.Equals("AddOrganization.Message.DataUpdate.Organization.PrimaryPhone.CityAreaCode", StringComparison.InvariantCulture)
                                );
                                var localPhoneNumberHV = phoneMobile.HostValues.Where(
                                    hv => hv.Field1.Equals("AddOrganization.Message.DataUpdate.Organization.PrimaryPhone.LocalPhoneNumber", StringComparison.InvariantCulture)
                                );

                                if (cityAreaCodeHV?.Any() == true)
                                {
                                    if (!string.IsNullOrWhiteSpace(cityAreaCodeHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneMobile.PhoneNumber))
                                    {
                                        result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                    }
                                }
                                else if (localPhoneNumberHV?.Any() == true)
                                {
                                    if (!string.IsNullOrWhiteSpace(localPhoneNumberHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneMobile.PhoneNumber))
                                    {
                                        result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                    }
                                }
                            }
                            else if (phoneHome != null && phoneMobile == null)
                            {
                                var cityAreaCodeHV = phoneHome.HostValues.Where(
                                    hv => hv.Field1.Equals("AddOrganization.Message.DataUpdate.Organization.PrimaryPhone.CityAreaCode", StringComparison.InvariantCulture)
                                );
                                var localPhoneNumberHV = phoneHome.HostValues.Where(
                                    hv => hv.Field1.Equals("AddOrganization.Message.DataUpdate.Organization.PrimaryPhone.LocalPhoneNumber", StringComparison.InvariantCulture)
                                );

                                if (cityAreaCodeHV?.Any() == true)
                                {
                                    if (!string.IsNullOrWhiteSpace(cityAreaCodeHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneHome.PhoneNumber))
                                    {
                                        result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                    }
                                }
                                else if (localPhoneNumberHV?.Any() == true)
                                {
                                    if (!string.IsNullOrWhiteSpace(localPhoneNumberHV?.SingleOrDefault().Value) && string.IsNullOrWhiteSpace(phoneHome.PhoneNumber))
                                    {
                                        result.AddError("The Application > Applicants > Phones > Phone Number is required");
                                    }
                                }
                            }

                            if (!result.Result)
                            {
                                _baseResult.AppendResult(result);

                                return result;
                            }
                        }
                    }
                }
                else
                {
                    result.AddError("The Application > Application Id is required");
                }
            }
            catch (ArgumentNullException ane)
            {
                result.AddError($"Source or predicate is null: {ane.ParamName}");
                Utility.LogError(ane, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddOrganization");
            }
            catch (InvalidOperationException ioe)
            {
                result.AddError($"The source sequence is empty or more than one element satisfies the condition in predicate: {ioe.Source}");
                Utility.LogError(ioe, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddOrganization");
            }
            catch (Exception ex)
            {
                result.AddError("Exception caught in LMS.Connector.CCM.Validation.ValidationManager.ValidateAddOrganization. See error log for more details.");
                Utility.LogError(ex, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddOrganization");
            }

            _baseResult.AppendResult(result);

            return result;
        }

        public BaseResult ValidateAddAccount()
        {
            var result = new BaseResult();

            try
            {
                if (_app.ApplicationId > 0)
                {
                    if (string.IsNullOrWhiteSpace(_app.ApplicationId.ToString()))
                        result.AddError("The Application > Application Id is required");

                    if (!_app.FinalLoanAmount.HasValue)
                        result.AddError("The Application > Final Loan Amount is required");

                    var primaryApplicant = _app.Applicants.SingleOrDefault(
                        a => a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary
                    );

                    if (string.IsNullOrWhiteSpace(primaryApplicant?.PersonNumber))
                        result.AddError("The Application > Applicants > Person Number is required");

                    var productNameHV = _app.HostValues.Where(hv => hv.Field1.Equals("AddAccount.Message.DataUpdate.Account.ProductName", StringComparison.InvariantCulture));

                    if (productNameHV?.Any() == false || string.IsNullOrWhiteSpace(productNameHV?.SingleOrDefault().Value))
                    {
                        result.AddError(GetRulesOnlyErrorMessage("AddAccount.Message.DataUpdate.Account.ProductName"));
                    }
                }
                else
                {
                    result.AddError("The Application > Application Id is required");
                }
            }
            catch (ArgumentNullException ane)
            {
                result.AddError($"Source or predicate is null: {ane.ParamName}");
                Utility.LogError(ane, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddAccount");
            }
            catch (InvalidOperationException ioe)
            {
                result.AddError($"The source sequence is empty or more than one element satisfies the condition in predicate: {ioe.Source}");
                Utility.LogError(ioe, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddAccount");
            }
            catch (Exception ex)
            {
                result.AddError("Exception caught in LMS.Connector.CCM.Validation.ValidationManager.ValidateAddAccount. See error log for more details.");
                Utility.LogError(ex, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddAccount");
            }

            _baseResult.AppendResult(result);

            return result;
        }

        public BaseResult ValidateAddAccountPartyRelationship()
        {
            var result = new BaseResult();

            try
            {
                if (_app.ApplicationId > 0)
                {
                    if (string.IsNullOrWhiteSpace(_app.ApplicationId.ToString()))
                        result.AddError("The Application > Application Id is required");

                    var jointApplicants = _app.Applicants.Where(
                        a => a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Joint
                    );

                    if (jointApplicants?.Any() == true)
                    {
                        foreach (var jointApplicant in jointApplicants)
                        {

                            if (string.IsNullOrWhiteSpace(jointApplicant.PersonNumber))
                                result.AddError("The Application > Applicants > Person Number is required");

                            var sendStatementHV = jointApplicant.HostValues.Where(
                                hv => hv.Field1.Equals(
                                    "AddAccountPartyRelationship.Message.DataUpdate.AccountPartyRelationships.AccountPartyRelationship.SendStatement",
                                    StringComparison.InvariantCulture
                                )
                            );

                            if (sendStatementHV?.Any() == false || string.IsNullOrWhiteSpace(sendStatementHV?.SingleOrDefault().Value))
                            {
                                result.AddError(GetRulesOnlyErrorMessage("AddAccountPartyRelationship.Message.DataUpdate.AccountPartyRelationships.AccountPartyRelationship.SendStatement"));
                            }

                            if (!result.Result)
                            {
                                _baseResult.AppendResult(result);

                                return result;
                            }
                        }
                    }
                }
                else
                {
                    result.AddError("The Application > Application Id is required");
                }
            }
            catch (ArgumentNullException ane)
            {
                result.AddError($"Source or predicate is null: {ane.ParamName}");
                Utility.LogError(ane, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddAccountPartyRelationship");
            }
            catch (InvalidOperationException ioe)
            {
                result.AddError($"The source sequence is empty or more than one element satisfies the condition in predicate: {ioe.Source}");
                Utility.LogError(ioe, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddAccountPartyRelationship");
            }
            catch (Exception ex)
            {
                result.AddError("Exception caught in LMS.Connector.CCM.Validation.ValidationManager.ValidateAddAccountPartyRelationship. See error log for more details.");
                Utility.LogError(ex, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddAccountPartyRelationship");
            }

            _baseResult = result;

            return result;
        }

        public BaseResult ValidateAddCard()
        {
            var result = new BaseResult();

            try
            {
                if (_app.ApplicationId > 0)
                {
                    if (string.IsNullOrWhiteSpace(_app.ApplicationId.ToString()))
                        result.AddError("The Application > Application Id is required");

                    var persons = _app.Applicants.Where(a => !a.IsOrganization);

                    if (persons?.Any() == true)
                    {
                        foreach (var person in persons)
                        {
                            // Applicant
                            if (string.IsNullOrWhiteSpace(person.PersonNumber))
                                result.AddError("The Application > Applicants > Person Number is required");

                            var embossingLine1HV = person.HostValues.Where(hv => hv.Field1.Equals("AddCard.Message.DataUpdate.Card.EmbossingLine1", StringComparison.InvariantCulture));

                            if (embossingLine1HV?.Any() == false || string.IsNullOrWhiteSpace(embossingLine1HV?.SingleOrDefault().Value))
                            {
                                result.AddError(GetRulesOnlyErrorMessage("AddCard.Message.DataUpdate.Card.EmbossingLine1"));
                            }

                            if (!result.Result)
                            {
                                _baseResult.AppendResult(result);

                                return result;
                            }
                        }
                    }

                    foreach (var authUser in _app.AuthorizedUsers)
                    {
                        // Authorized User
                        if (string.IsNullOrWhiteSpace(authUser.PersonNumber))
                            result.AddError("The Application > Authorized User > Person Number is required");

                        var embossingLine1HV = authUser.HostValues.Where(hv => hv.Field1.Equals("AddCard.Message.DataUpdate.Card.EmbossingLine1", StringComparison.InvariantCulture));

                        if (embossingLine1HV?.Any() == false || string.IsNullOrWhiteSpace(embossingLine1HV.SingleOrDefault().Value))
                        {
                            result.AddError(GetRulesOnlyErrorMessage("AddCard.Message.DataUpdate.Card.EmbossingLine1"));
                        }

                        if (!result.Result)
                        {
                            _baseResult.AppendResult(result);

                            return result;
                        }
                    }
                }
                else
                {
                    result.AddError("The Application > Application Id is required");
                }
            }
            catch (ArgumentNullException ane)
            {
                result.AddError($"Source or predicate is null: {ane.ParamName}");
                Utility.LogError(ane, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddCard");
            }
            catch (InvalidOperationException ioe)
            {
                result.AddError($"The source sequence is empty or more than one element satisfies the condition in predicate: {ioe.Source}");
                Utility.LogError(ioe, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddCard");
            }
            catch (Exception ex)
            {
                result.AddError("Exception caught in LMS.Connector.CCM.Validation.ValidationManager.ValidateAddCard. See error log for more details.");
                Utility.LogError(ex, "LMS.Connector.CCM.Validation.ValidationManager.ValidateAddCard");
            }

            _baseResult.AppendResult(result);

            return result;
        }

        public BaseResult ValidateUpdateAccount()
        {
            var result = new BaseResult();

            try
            {
                if (_app.ApplicationId > 0)
                {
                    if (string.IsNullOrWhiteSpace(_app.ApplicationId.ToString()))
                        result.AddError("The Application > Application Id is required");

                    if (string.IsNullOrWhiteSpace(_app.CreditCardNumber))
                        result.AddError("The Application > Credit Card Number is required");

                    if (!_app.FinalLoanAmount.HasValue)
                        result.AddError("The Application > Final Loan Amount is required");
                }
                else
                {
                    result.AddError("The Application > Application Id is required");
                }
            }
            catch (Exception ex)
            {
                result.AddError("Exception caught in LMS.Connector.CCM.Validation.ValidationManager.ValidateUpdateAccount. See error log for more details.");
                Utility.LogError(ex, "LMS.Connector.CCM.Validation.ValidationManager.ValidateUpdateAccount");
            }

            _baseResult.AppendResult(result);

            return result;
        }

        public BaseResult ValidateUpdatePerson()
        {
            throw new NotImplementedException();
        }

        public string GetRulesOnlyErrorMessage(string fullHostValuePath, string category = "Disbursement")
        {
            return
                $"The {fullHostValuePath} was not found in the Host Value collection. Please ensure that a {category} rule is authored that sets this host value.";
        }
    }
}
