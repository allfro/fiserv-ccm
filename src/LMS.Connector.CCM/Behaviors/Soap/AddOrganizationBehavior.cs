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
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors.Soap
{
    public class AddOrganizationBehavior : Behavior, IAddOrganization
    {
        private Application _app;
        private string _userToken;
        private AddOrganization _organization;
        private Address _currentAddress;
        private Phone _mainPhone;
        private MessageResponse _messageResponse;

        public AddOrganization Organization
        {
            get
            {
                return _organization;
            }
            set
            {
                _organization = value;
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

        public AddOrganizationBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = new SoapRepository(userToken);
            _lmsRepository = new LmsRepository(userToken);
        }

        public AddOrganizationBehavior(Application app, string userToken, ISoapRepository serviceRepository)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
            _lmsRepository = new LmsRepository(userToken);
        }

        public AddOrganizationBehavior(Application app, string userToken, ISoapRepository serviceRepository, ILmsRepository lmsRepository)
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

        public BaseResult AddOrganization(Applicant organization)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Soap.AddOrganizationBehavior.AddOrganization"))
            {
                tr.Log($"AddOrganization for ApplicantId {organization.ApplicantId}, PersonNumber => {organization.PersonNumber}");

                tr.Log($"Address _currentAddress null? => {_currentAddress == null}");
                if (_currentAddress == null)
                {
                    tr.Log("Call GetCurrentAddress() to get new _currentAddress");
                    _currentAddress = GetCurrentAddress(organization);
                }
                tr.LogObject(_currentAddress);

                tr.Log($"Phone _mainPhone null? => {_mainPhone == null}");
                if (_mainPhone == null)
                {
                    tr.Log("Call GetMainPhone() to get new _mainPhone");
                    _mainPhone = GetMainPhone(organization);
                }
                tr.LogObject(_mainPhone);

                tr.Log($"AddOrganization _organization null? => {_organization == null}");
                if (_organization == null)
                {
                    tr.Log("Call GetDto() to get new _organization");
                    _organization = GetDto(organization);
                }
                tr.LogObject(_organization);

                try
                {
                    tr.Log("Calling ISoapRespository.AddOrganization");
                    _messageResponse = _soapRepository.AddOrganization(_organization, _app, organization, _currentAddress, _mainPhone);

                    tr.Log($"_messageResponse.OrganizationPartyId = {_messageResponse?.OrganizationPartyId}");
                    tr.Log($"_messageResponse.ResponseCode = {_messageResponse?.ResponseCode}");
                    tr.Log($"_messageResponse.ErrorMessage = {_messageResponse?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Soap.AddOrganizationBehavior.AddOrganization");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call SOAP Repository AddOrganization(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTOs
                    _currentAddress = null;
                    _mainPhone = null;
                    _organization = null;
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

        public AddOrganization GetDto(Applicant organization)
        {
            var addOrganization = new AddOrganization()
            {
                Message = GetMessage(organization)
            };

            return addOrganization;
        }

        public Address GetCurrentAddress(Applicant applicant)
        {
            var currentAddress = applicant.Addresses.FirstOrDefault(
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

        #endregion

        #region "DTO Builder Methods"

        private Dto.Soap.Message GetMessage(Applicant organization)
        {
            var message = new Dto.Soap.Message()
            {
                DataUpdate = GetDataUpdate(organization)
            };

            return message;
        }

        private DataUpdate GetDataUpdate(Applicant organization)
        {
            var dataUpdate = new DataUpdate()
            {
                TraceNumber = _app.ApplicationId.ToString(),
                ProcessingCode = "ExternalUpdateRequest",
                Source = "LoanOrigination",
                UpdateAction = "Add",
                Organization = GetOrganization(organization)
            };

            return dataUpdate;
        }

        private Organization GetOrganization(Applicant organization)
        {
            var org = new Organization()
            {
                PartyNumber = organization.PersonNumber,
                TaxIdNumber = organization.TIN,
                PrimaryAddress = GetPrimaryAddress(),
                PrimaryEmail = GetPrimaryEmail(organization),
                PrimaryPhone = GetPrimaryPhone(),
                InstitutionRelationShipType = organization.IsCustomer ? "Customer" : "Other",
                Name = organization.OrganizationName,
                BusinessOwnershipType = GetOrganizationType(organization),
                SicCode = organization.HostValues.Any(h => h.Field1.Equals("AddOrganization.Message.DataUpdate.Organization.SicCode")) ? string.Empty : null,
                NaicsCode = organization.HostValues.Any(h => h.Field1.Equals("AddOrganization.Message.DataUpdate.Organization.NaicsCode")) ? string.Empty : null
            };

            if (organization.DateBusinessEstablished.HasValue)
            {
                org.CreationDate = organization.DateBusinessEstablished.Value.ToString("yyyy-MM-dd");
            }
            else if (organization.HostValues.Any(h => h.Field1.Equals("AddOrganization.Message.DataUpdate.Organization.CreationDate")))
            {
                org.CreationDate = string.Empty;
            }
            else
            {
                org.CreationDate = null;
            }

            return org;
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

        private PrimaryEmail GetPrimaryEmail(Applicant organization)
        {
            var primaryEmail = new PrimaryEmail()
            {
                EmailAddress = organization.Email
            };

            return primaryEmail;
        }

        private PrimaryPhone GetPrimaryPhone()
        {
            var primaryPhone = new PrimaryPhone();

            primaryPhone.CountryCallingCode = _mainPhone.HostValues.Any(h => h.Field1.Equals("AddPerson.Message.DataUpdate.Person.PrimaryPhone.CountryCallingCode")) ? string.Empty : null;
            primaryPhone.CityAreaCode = _mainPhone.PhoneNumberRaw.GetPhoneNumberAreaCode();
            primaryPhone.LocalPhoneNumber = _mainPhone.PhoneNumberRaw.GetPhoneNumberMajor() + _mainPhone.PhoneNumberRaw.GetPhoneNumberMinor();
            primaryPhone.Extension = _mainPhone.Extension;

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
                    addressType = "Business";
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

        private string GetOrganizationType(Applicant organization)
        {
            var organizationTypeDesc = _lmsRepository.GetLookupDescById((int)organization?.OrganizationTypeId.GetValueOrDefault(), LookupTypes.OrganizationType);
            string organizationType = string.Empty;

            switch (organizationTypeDesc.ToUpper())
            {
                case "CORPORATION":
                case "LLC":
                case "OTHER":
                    organizationType = "Corporation";
                    break;
                case "PARTNERSHIP":
                    organizationType = "Partnership";
                    break;
                case "PROPRIETOR":
                    organizationType = "SoleProprietorship";
                    break;
                default:
                    break;
            }

            return organizationType;
        }

        #endregion
    }
}
