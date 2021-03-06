﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Core.Lookups;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Helpers;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors.Soap
{
    public class UpdatePersonBehavior : Behavior, IUpdatePerson
    {
        private Application _app;
        private string _userToken;
        private UpdatePerson _person;
        private MessageResponse _messageResponse;

        public UpdatePerson Person
        {
            get
            {
                return _person;
            }
            set
            {
                _person = value;
            }
        }

        public UpdatePersonBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = new SoapRepository(userToken);
            _lmsRepository = new LmsRepository(userToken);
        }

        public UpdatePersonBehavior(Application app, string userToken, ISoapRepository serviceRepository)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
            _lmsRepository = new LmsRepository(userToken);
        }

        public UpdatePersonBehavior(Application app, string userToken, ISoapRepository serviceRepository, ILmsRepository lmsRepository)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
            _lmsRepository = lmsRepository;
        }

        #region "Interface implementations"

        public MessageResponse MessageResponse
        {
            get
            {
                return _messageResponse;
            }
            set
            {
                _messageResponse = value;
            }
        }

        public BaseResult UpdatePerson(LmsPerson lmsPerson)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Soap.UpdatePersonBehavior.UpdatePerson"))
            {
                if (lmsPerson.Applicant != null)
                {
                    tr.Log($"UpdatePerson for ApplicantId {lmsPerson.Applicant.ApplicantId}, PersonNumber => {lmsPerson.Applicant.PersonNumber}");
                }
                else if (lmsPerson.AuthorizedUser != null)
                {
                    tr.Log($"UpdatePerson for AuthorizedUserId {lmsPerson.AuthorizedUser.AuthorizedUserId}, PersonNumber => {lmsPerson.AuthorizedUser.PersonNumber}");
                }

                tr.Log($"UpdatePerson _person null? => {_person == null}");
                if (_person == null)
                {
                    tr.Log("Call GetDto() to get new _person");
                    _person = GetDto(lmsPerson);
                }
                tr.LogObject(_person);

                try
                {
                    tr.Log("Calling ISoapRepository.UpdatePerson");
                    _messageResponse = _soapRepository.UpdatePerson(_person, _app);

                    tr.Log($"_messageResponse.ResponseCode = {_messageResponse?.ResponseCode}");
                    tr.Log($"_messageResponse.ErrorMessage = {_messageResponse?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Soap.UpdatePersonBehavior.UpdatePerson");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call SOAP Repository UpdatePerson(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTO
                    _person = null;
                }

                if (_messageResponse?.ResponseCode != "Success" && _messageResponse?.ErrorMessage?.Length > 0)
                {
                    result.Result = false;
                    result.AddMessage(MessageType.Error, _messageResponse.ErrorMessage);

                    return result;
                }
            }

            return result;
        }

        public UpdatePerson GetDto(LmsPerson lmsPerson)
        {
            var person = new UpdatePerson()
            {
                Message = GetMessage(lmsPerson)
            };

            return person;
        }

        #endregion

        #region "DTO Builder Methods"

        private Dto.Soap.Message GetMessage(LmsPerson lmsPerson)
        {
            var message = new Dto.Soap.Message()
            {
                DataUpdate = GetDataUpdate(lmsPerson)
            };

            return message;
        }

        private DataUpdate GetDataUpdate(LmsPerson lmsPerson)
        {
            var dataUpdate = new DataUpdate()
            {
                TraceNumber = _app.ApplicationId.ToString(),
                ProcessingCode = "ExternalUpdateRequest",
                Source = "LoanOrigination",
                UpdateAction = "Modify",
                Person = GetPerson(lmsPerson),
                ModifiedFields = GetModifiedFields(lmsPerson)
            };

            return dataUpdate;
        }

        private Person GetPerson(LmsPerson lmsPerson)
        {
            var applicant = lmsPerson.Applicant;
            var authorizedUser = lmsPerson.AuthorizedUser;

            var person = new Person()
            {
                PrimaryAddress = GetPrimaryAddress(lmsPerson),
                PrimaryEmail = GetPrimaryEmail(lmsPerson),
                PrimaryPhone = GetPrimaryPhone(lmsPerson)
            };

            if (applicant != null && authorizedUser == null)
            {
                person.PartyNumber = applicant.PersonNumber;
                person.TaxIdNumber = applicant.TIN;
                person.InstitutionRelationShipType = (applicant.IsCustomer) ? "Customer" : "Other";
                person.LastName = applicant.LastName;
                person.FirstName = applicant.FirstName;
                person.MiddleName = applicant.MiddleName;
                person.DateOfBirth = applicant.BirthDate.HasValue ? applicant.BirthDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                person.DateOfDeath = string.Empty;
                person.Title = string.Empty;
                person.NameSuffix = applicant.Suffix;
                person.MothersMaidenName = applicant.MothersMaidenName;
            }
            else if (applicant == null && authorizedUser != null)
            {
                person.PartyNumber = authorizedUser.PersonNumber;
                person.TaxIdNumber = authorizedUser.TIN;
                person.LastName = authorizedUser.LastName;
                person.FirstName = authorizedUser.FirstName;
                person.MiddleName = authorizedUser.MiddleName;
                person.DateOfBirth = authorizedUser.BirthDate.HasValue ? authorizedUser.BirthDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                person.DateOfDeath = string.Empty;
                person.Title = string.Empty;
                person.NameSuffix = authorizedUser.Suffix;
                person.MothersMaidenName = authorizedUser.MothersMaidenName;
            }

            return person;
        }

        private PrimaryAddress GetPrimaryAddress(LmsPerson lmsPerson)
        {
            var applicant = lmsPerson.Applicant;
            var authorizedUser = lmsPerson.AuthorizedUser;
            Address currentAddress = null;

            if (applicant != null && authorizedUser == null)
            {
                currentAddress = applicant.Addresses.SingleOrDefault(
                    a => a.AddressTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.AddressType, LookupCodes.AddressType.Current)
                );
            }
            else if (applicant == null && authorizedUser != null)
            {
                currentAddress = authorizedUser.Addresses.SingleOrDefault(
                    a => a.AddressTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.AddressType, LookupCodes.AddressType.Current)
                );
            }

            var primaryAddress = new PrimaryAddress()
            {
                AddressLine1 = currentAddress?.Address1,
                AddressLine2 = currentAddress?.Address2,
                City = currentAddress?.City,
                StateProvince = _lmsRepository.GetLookupCodeById((int)currentAddress?.StateId.GetValueOrDefault(), LookupTypes.StateCode),
                PostalCode = currentAddress?.PostalCode,
                CountryCode = _lmsRepository.GetLookupCodeById((int)currentAddress?.CountryId.GetValueOrDefault(), LookupTypes.CountryCode),
                AddressType = GetAddressType(currentAddress)
            };

            return primaryAddress;
        }

        private PrimaryEmail GetPrimaryEmail(LmsPerson lmsPerson)
        {
            var applicant = lmsPerson.Applicant;
            var authorizedUser = lmsPerson.AuthorizedUser;
            var primaryEmail = new PrimaryEmail();

            if (applicant != null && authorizedUser == null)
            {
                primaryEmail.EmailAddress = applicant.Email;
            }
            else if (applicant == null && authorizedUser != null)
            {
                primaryEmail.EmailAddress = authorizedUser.Email;
            }

            return primaryEmail;
        }

        private PrimaryPhone GetPrimaryPhone(LmsPerson lmsPerson)
        {
            var applicant = lmsPerson.Applicant;
            var authorizedUser = lmsPerson.AuthorizedUser;
            var primaryPhone = new PrimaryPhone();

            if (applicant != null && authorizedUser == null)
            {
                var phoneMobile = applicant.Phones.SingleOrDefault(
                    p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Mobile)
                );

                var phoneHome = applicant.Phones.SingleOrDefault(
                    p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Home)
                );

                if (phoneMobile != null)
                {
                    primaryPhone.CountryCallingCode = string.Empty;
                    primaryPhone.CityAreaCode = phoneMobile.PhoneNumberRaw.GetPhoneNumberAreaCode();
                    primaryPhone.LocalPhoneNumber = phoneMobile.PhoneNumberRaw.GetPhoneNumberMajor() + phoneMobile.PhoneNumberRaw.GetPhoneNumberMinor();
                    primaryPhone.Extension = phoneMobile.Extension;

                    var textInfo = new CultureInfo("en-US", false).TextInfo;
                    var phoneType = _lmsRepository.GetLookupCodeById(phoneMobile.PhoneTypeId, LookupTypes.PhoneType);
                    primaryPhone.PhoneType = textInfo.ToTitleCase(phoneType);
                }
                else if (phoneHome != null && phoneMobile == null)
                {
                    primaryPhone.CountryCallingCode = string.Empty;
                    primaryPhone.CityAreaCode = phoneHome.PhoneNumberRaw.GetPhoneNumberAreaCode();
                    primaryPhone.LocalPhoneNumber = phoneHome.PhoneNumberRaw.GetPhoneNumberMajor() + phoneHome.PhoneNumberRaw.GetPhoneNumberMinor();
                    primaryPhone.Extension = phoneHome.Extension;

                    var textInfo = new CultureInfo("en-US", false).TextInfo;
                    var phoneType = _lmsRepository.GetLookupCodeById(phoneHome.PhoneTypeId, LookupTypes.PhoneType);
                    primaryPhone.PhoneType = textInfo.ToTitleCase(phoneType);
                }
                else
                {
                    // Set primaryPhone values via Rules
                }
            }
            else if (applicant == null && authorizedUser != null)
            {
                var phoneMobile = authorizedUser.Phones.SingleOrDefault(
                    p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Mobile)
                );
                var phoneHome = authorizedUser.Phones.SingleOrDefault(p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Home));

                if (phoneMobile != null)
                {
                    primaryPhone.CountryCallingCode = string.Empty;
                    primaryPhone.CityAreaCode = phoneMobile.PhoneNumberRaw.GetPhoneNumberAreaCode();
                    primaryPhone.LocalPhoneNumber = phoneMobile.PhoneNumberRaw.GetPhoneNumberMajor() + phoneMobile.PhoneNumberRaw.GetPhoneNumberMinor();
                    primaryPhone.Extension = phoneMobile.Extension;

                    var textInfo = new CultureInfo("en-US", false).TextInfo;
                    var phoneType = _lmsRepository.GetLookupCodeById(phoneMobile.PhoneTypeId, LookupTypes.PhoneType);
                    primaryPhone.PhoneType = textInfo.ToTitleCase(phoneType);
                }
                else if (phoneHome != null && phoneMobile == null)
                {
                    primaryPhone.CountryCallingCode = string.Empty;
                    primaryPhone.CityAreaCode = phoneHome.PhoneNumberRaw.GetPhoneNumberAreaCode();
                    primaryPhone.LocalPhoneNumber = phoneHome.PhoneNumberRaw.GetPhoneNumberMajor() + phoneHome.PhoneNumberRaw.GetPhoneNumberMinor();
                    primaryPhone.Extension = phoneHome.Extension;

                    var textInfo = new CultureInfo("en-US", false).TextInfo;
                    var phoneType = _lmsRepository.GetLookupCodeById(phoneHome.PhoneTypeId, LookupTypes.PhoneType);
                    primaryPhone.PhoneType = textInfo.ToTitleCase(phoneType);
                }
                else
                {
                    // Set primaryPhone values via Rules
                }
            }

            return primaryPhone;
        }

        private string GetAddressType(Address address)
        {
            var addressTypeCode = _lmsRepository.GetLookupCodeById((int)address?.ClassificationId.GetValueOrDefault(), LookupTypes.AddressClassification);
            string addressType = string.Empty;

            switch (addressTypeCode)
            {
                case "1":
                case "3":
                case "5":
                    addressType = "Home";
                    break;
                case "2":
                    addressType = "POBox";
                    break;
                case "4":
                    addressType = "Military";
                    break;
                default:
                    break;
            }

            return addressType;
        }

        public List<ModifiedFields> GetModifiedFields(LmsPerson lmsPerson)
        {
            List<ModifiedFields> modifiedFieldsList = null;
            ModifiedFields modifiedFields = null;

            modifiedFields = GetPersonField(modifiedFields, lmsPerson);
            if (modifiedFields != null)
            {
                (modifiedFieldsList ?? (modifiedFieldsList = new List<ModifiedFields>())).Add(modifiedFields);
            }

            modifiedFields = GetAddressField(modifiedFields, lmsPerson);
            if (modifiedFields != null)
            {
                (modifiedFieldsList ?? (modifiedFieldsList = new List<ModifiedFields>())).Add(modifiedFields);
            }

            modifiedFields = GetEmailField(modifiedFields, lmsPerson);
            if (modifiedFields != null)
            {
                (modifiedFieldsList ?? (modifiedFieldsList = new List<ModifiedFields>())).Add(modifiedFields);
            }

            modifiedFields = GetPhoneField(modifiedFields, lmsPerson);
            if (modifiedFields != null)
            {
                (modifiedFieldsList ?? (modifiedFieldsList = new List<ModifiedFields>())).Add(modifiedFields);
            }

            return modifiedFieldsList;
        }

        public ModifiedFields GetPersonField(ModifiedFields modifiedFields, LmsPerson lmsPerson)
        {
            var personFieldHVs = _app.HostValues.Where
            (
                hv => hv.Field1.Contains("UpdatePerson.Message.DataUpdate.ModifiedFields.PersonField")
            );

            if (personFieldHVs.Any())
            {
                if (modifiedFields == null)
                {
                    modifiedFields = new ModifiedFields();
                }

                var personFields = new List<string>();

                // Add PersonField host values to Dto
                foreach (var personFieldHV in personFieldHVs.ToList())
                {
                    personFields.Add(personFieldHV.Value);

                    // Remove this host value from the application entity since it was already manually added
                    // and doesn't need to be "re-added" by the host value translator.
                    _app.HostValues.Remove(personFieldHV);
                }

                modifiedFields.PersonField = personFields;
            }

            return modifiedFields;
        }

        /// <summary>
        /// Manually creates a collection of AddressFields that will be added as a property of ModifiedFields.
        /// </summary>
        /// <param name="modifiedFields"></param>
        /// <param name="lmsPerson"></param>
        /// <remarks>
        /// This method already has access to application-level host values.
        /// Optionally giving it the ability to access applicant-level or authorized user-level host values.
        /// </remarks>
        /// <returns></returns>
        public ModifiedFields GetAddressField(ModifiedFields modifiedFields, LmsPerson lmsPerson)
        {
            var addressFieldHVs = _app.HostValues.Where
            (
                hv => hv.Field1.Contains("UpdatePerson.Message.DataUpdate.ModifiedFields.AddressField")
            );

            if (addressFieldHVs.Any())
            {
                if (modifiedFields == null)
                {
                    modifiedFields = new ModifiedFields();
                }

                var addressFields = new List<string>();

                // Add AddressField host values to Dto
                foreach (var addressFieldHV in addressFieldHVs.ToList())
                {
                    addressFields.Add(addressFieldHV.Value);

                    // Remove this host value from the application entity since it was already manually added
                    // and doesn't need to be "re-added" by the host value translator.
                    _app.HostValues.Remove(addressFieldHV);
                }

                modifiedFields.AddressField = addressFields;
            }

            return modifiedFields;
        }

        public ModifiedFields GetEmailField(ModifiedFields modifiedFields, LmsPerson lmsPerson)
        {
            var emailFieldHVs = _app.HostValues.Where
            (
                hv => hv.Field1.Contains("UpdatePerson.Message.DataUpdate.ModifiedFields.EmailField")
            );

            if (emailFieldHVs.Any())
            {
                if (modifiedFields == null)
                {
                    modifiedFields = new ModifiedFields();
                }

                var emailFields = new List<string>();

                // Add EmailField host values to Dto
                foreach (var emailFieldHV in emailFieldHVs.ToList())
                {
                    emailFields.Add(emailFieldHV.Value);

                    // Remove this host value from the application entity since it was already manually added
                    // and doesn't need to be "re-added" by the host value translator.
                    _app.HostValues.Remove(emailFieldHV);
                }

                modifiedFields.EmailField = emailFields;
            }

            return modifiedFields;
        }

        public ModifiedFields GetPhoneField(ModifiedFields modifiedFields, LmsPerson lmsPerson)
        {
            var addressFieldHVs = _app.HostValues.Where
            (
                hv => hv.Field1.Contains("UpdatePerson.Message.DataUpdate.ModifiedFields.AddressField")
            );

            if (addressFieldHVs.Any())
            {
                if (modifiedFields == null)
                {
                    modifiedFields = new ModifiedFields();
                }

                var addressFields = new List<string>();

                // Add AddressField host values to Dto
                foreach (var addressFieldHV in addressFieldHVs.ToList())
                {
                    addressFields.Add(addressFieldHV.Value);

                    // Remove this host value from the application entity since it was already manually added
                    // and doesn't need to be "re-added" by the host value translator.
                    _app.HostValues.Remove(addressFieldHV);
                }

                modifiedFields.AddressField = addressFields;
            }

            return modifiedFields;
        }

        #endregion
    }
}
