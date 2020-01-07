using System;
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
    public class AddPersonBehavior : Behavior, IAddPerson
    {
        private Application _app;
        private string _userToken;
        private AddPerson _person;
        private Address _currentAddress;
        private Phone _mainPhone;
        private MessageResponse _messageResponse;

        public AddPerson Person
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

        public Address CurrentAddress
        {
            get
            {
                return _currentAddress;
            }
            set
            {
                _currentAddress = value;
            }
        }

        public Phone MainPhone
        {
            get
            {
                return _mainPhone;
            }
            set
            {
                _mainPhone = value;
            }
        }

        public AddPersonBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = new SoapRepository(userToken);
            _lmsRepository = new LmsRepository(userToken);
        }

        public AddPersonBehavior(Application app, string userToken, ISoapRepository serviceRepository)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
            _lmsRepository = new LmsRepository(userToken);
        }

        public AddPersonBehavior(Application app, string userToken, ISoapRepository serviceRepository, ILmsRepository lmsRepository)
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

        public BaseResult AddPerson(LmsPerson lmsPerson)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Soap.AddPersonBehavior.AddPerson"))
            {
                if (lmsPerson.Applicant != null)
                {
                    tr.Log($"AddPerson for ApplicantId {lmsPerson.Applicant.ApplicantId}, PersonNumber => {lmsPerson.Applicant.PersonNumber}");

                    tr.Log($"Address _currentAddress null? => {_currentAddress == null}");
                    if (_currentAddress == null)
                    {
                        tr.Log("Call GetCurrentAddress() to get new _currentAddress");
                        _currentAddress = GetCurrentAddress(lmsPerson.Applicant);
                    }

                    tr.Log($"Phone _mainPhone null? => {_mainPhone == null}");
                    if (_mainPhone == null)
                    {
                        tr.Log("Call GetMainPhone() to get new _mainPhone");
                        _mainPhone = GetMainPhone(lmsPerson.Applicant);
                    }
                }
                else if (lmsPerson.Applicant == null && lmsPerson.AuthorizedUser != null)
                {
                    tr.Log($"AddPerson for AuthorizedUserId {lmsPerson.AuthorizedUser.AuthorizedUserId}, PersonNumber {lmsPerson.AuthorizedUser.PersonNumber}");

                    tr.Log($"Address _currentAddress null? => {_currentAddress == null}");
                    if (_currentAddress == null)
                    {
                        tr.Log("Call GetCurrentAddress() to get new _currentAddress");
                        _currentAddress = GetCurrentAddress(lmsPerson.AuthorizedUser);
                    }

                    tr.Log($"Phone _mainPhone null? => {_mainPhone == null}");
                    if (_mainPhone == null)
                    {
                        tr.Log("Call GetMainPhone() to get new _mainPhone");
                        _mainPhone = GetMainPhone(lmsPerson.AuthorizedUser);
                    }
                }
                tr.LogObject(_currentAddress);
                tr.LogObject(_mainPhone);

                tr.Log($"AddPerson _person null? => {_person == null}");
                if (_person == null)
                {
                    tr.Log("Call GetDto() to get new _person");
                    _person = GetDto(lmsPerson);
                }
                tr.LogObject(_person);

                try
                {
                    tr.Log("Calling ISoapRepository.AddPerson");
                    _messageResponse = _soapRepository.AddPerson(_person, _app, lmsPerson, _currentAddress, _mainPhone);

                    tr.Log($"_messageResponse.PersonPartyId = {_messageResponse?.PersonPartyId}");
                    tr.Log($"_messageResponse.ResponseCode = {_messageResponse?.ResponseCode}");
                    tr.Log($"_messageResponse.ErrorMessage = {_messageResponse?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Soap.AddPersonBehavior.AddPerson");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call SOAP Repository AddPerson(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTOs
                    _currentAddress = null;
                    _mainPhone = null;
                    _person = null;
                }

                if (_messageResponse?.ResponseCode != "Success" && _messageResponse?.ErrorMessage?.Length > 0)
                {
                    if (_messageResponse?.ErrorMessage.IndexOf("already exists", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        result.Result = true;
                    }
                    else
                    {
                        result.Result = false;
                        result.AddMessage(MessageType.Error, _messageResponse.ErrorMessage);
                    }
                }
            }

            return result;
        }

        public AddPerson GetDto(LmsPerson lmsPerson)
        {
            var person = new AddPerson()
            {
                Message = GetMessage(lmsPerson)
            };

            return person;
        }

        public Address GetCurrentAddress(Applicant applicant)
        {
            var currentAddress = applicant.Addresses.FirstOrDefault(
                a => a.AddressTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.AddressType, LookupCodes.AddressType.Current)
            );

            return currentAddress;
        }

        public Address GetCurrentAddress(AuthorizedUser authorizedUser)
        {
            var currentAddress = authorizedUser.Addresses.FirstOrDefault(
                a => a.AddressTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.AddressType, LookupCodes.AddressType.Current)
            );

            return currentAddress;
        }

        public Phone GetMainPhone(Applicant applicant)
        {
            Phone mainPhone = null;

            var phoneMobile = applicant.Phones.FirstOrDefault(
                p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Mobile)
            );

            var phoneHome = applicant.Phones.FirstOrDefault(
                p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Home)
            );

            if (phoneMobile != null)
            {
                mainPhone = phoneMobile;
            }
            else if (phoneHome != null && phoneMobile == null)
            {
                mainPhone = phoneHome;
            }
            else
            {
                mainPhone = applicant.Phones.FirstOrDefault();
            }

            return mainPhone;
        }

        public Phone GetMainPhone(AuthorizedUser authorizedUser)
        {
            Phone mainPhone = null;

            var phoneMobile = authorizedUser.Phones.FirstOrDefault(
                p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Mobile)
            );

            var phoneHome = authorizedUser.Phones.FirstOrDefault(
                p => p.PhoneTypeId == _lmsRepository.GetLookupIdByTypeAndCode(LookupTypes.PhoneType, LookupCodes.PhoneType.Home)
            );

            if (phoneMobile != null)
            {
                mainPhone = phoneMobile;
            }
            else if (phoneHome != null && phoneMobile == null)
            {
                mainPhone = phoneHome;
            }
            else
            {
                mainPhone = authorizedUser.Phones.FirstOrDefault();
            }

            return mainPhone;
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
                UpdateAction = "Add",
                Person = GetPerson(lmsPerson)
            };

            return dataUpdate;
        }

        private Person GetPerson(LmsPerson lmsPerson)
        {
            var applicant = lmsPerson.Applicant;
            var authorizedUser = lmsPerson.AuthorizedUser;

            var person = new Person()
            {
                PrimaryAddress = GetPrimaryAddress(),
                PrimaryEmail = GetPrimaryEmail(lmsPerson),
                PrimaryPhone = GetPrimaryPhone()
            };

            if (applicant != null && authorizedUser == null)
            {
                person.PartyNumber = applicant.PersonNumber;
                person.TaxIdNumber = applicant.TIN;
                person.InstitutionRelationShipType = applicant.IsCustomer ? "Customer" : "Other";
                person.LastName = applicant.LastName;
                person.FirstName = applicant.FirstName;
                person.MiddleName = applicant.MiddleName;
                person.DateOfBirth = applicant.BirthDate.HasValue ? applicant.BirthDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                person.Title = applicant.HostValues.Any(h => h.Field1.Equals("AddPerson.Message.DataUpdate.Person.Title")) ? string.Empty : null;
                person.NameSuffix = string.IsNullOrWhiteSpace(applicant.Suffix) ? null : applicant.Suffix;
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
                person.Title = authorizedUser.HostValues.Any(h => h.Field1.Equals("AddPerson.Message.DataUpdate.Person.Title")) ? string.Empty : null;
                person.NameSuffix = string.IsNullOrWhiteSpace(authorizedUser.Suffix) ? null : authorizedUser.Suffix;
                person.MothersMaidenName = authorizedUser.MothersMaidenName;
            }

            return person;
        }

        private PrimaryAddress GetPrimaryAddress()
        {
            var primaryAddress = new PrimaryAddress()
            {
                AddressLine1 = _currentAddress?.Address1,
                AddressLine2 = _currentAddress?.Address2,
                City = _currentAddress?.City,
                StateProvince = _lmsRepository.GetLookupCodeById((int)_currentAddress?.StateId.GetValueOrDefault(), LookupTypes.StateCode),
                PostalCode = _currentAddress?.PostalCode,
                CountryCode = _lmsRepository.GetLookupCodeById((int)_currentAddress?.CountryId.GetValueOrDefault(), LookupTypes.CountryCode),
                AddressType = _currentAddress.ClassificationId.HasValue ? GetAddressType(_currentAddress) : null
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

        private PrimaryPhone GetPrimaryPhone()
        {
            var primaryPhone = new PrimaryPhone();

            primaryPhone.CountryCallingCode = _mainPhone.HostValues.Any(h => h.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.CountryCallingCode")) ? string.Empty : null;
            primaryPhone.CityAreaCode = _mainPhone.PhoneNumberRaw.GetPhoneNumberAreaCode();
            primaryPhone.LocalPhoneNumber = _mainPhone.PhoneNumberRaw.GetPhoneNumberMajor() + _mainPhone.PhoneNumberRaw.GetPhoneNumberMinor();
            primaryPhone.Extension = _mainPhone.Extension;
            primaryPhone.Description = _mainPhone.HostValues.Any(h => h.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.Description")) ? string.Empty : null;

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            var phoneType = _lmsRepository.GetPhoneType(_mainPhone.PhoneTypeId);
            primaryPhone.PhoneType = phoneType;

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

        #endregion
    }
}
