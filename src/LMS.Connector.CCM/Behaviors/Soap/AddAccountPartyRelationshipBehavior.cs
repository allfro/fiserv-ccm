using System;
using System.Collections.Generic;
using System.Linq;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors.Soap
{
    public class AddAccountPartyRelationshipBehavior : Behavior, IAddAccountPartyRelationship
    {
        private Application _app;
        private string _userToken;
        private AddAccountPartyRelationship _accountPartyRelationship;
        private MessageResponse _messageResponse;

        public AddAccountPartyRelationship AccountPartyRelationship
        {
            get
            {
                return _accountPartyRelationship;
            }
            set
            {
                _accountPartyRelationship = value;
            }
        }

        public AddAccountPartyRelationshipBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = new SoapRepository(userToken);
        }
        public AddAccountPartyRelationshipBehavior(Application app, string userToken, ISoapRepository serviceRepository)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
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

        public BaseResult AddAccountPartyRelationship(Applicant applicant)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Soap.AddAccountPartyRelationshipBehavior.AddAccountPartyRelationship"))
            {
                tr.Log($"AddAccountPartyRelationship for ApplicantId {applicant.ApplicantId}, PersonNumber => {applicant.PersonNumber}");

                tr.Log($"AddAccountPartyRelationship _accountPartyRelationship null? => {_accountPartyRelationship == null}");
                if (_accountPartyRelationship == null)
                {
                    tr.Log("Call GetDto() to get new _accountPartyRelationship");
                    _accountPartyRelationship = GetDto(applicant);
                }
                tr.LogObject(_accountPartyRelationship);

                try
                {
                    tr.Log("Calling ISoapRepository.AddAccountPartyRelationship");
                    _messageResponse = _soapRepository.AddAccountPartyRelationship(_accountPartyRelationship, _app, applicant);

                    tr.Log($"_messageResponse.ResponseCode = {_messageResponse?.ResponseCode}");
                    tr.Log($"_messageResponse.ErrorMessage = {_messageResponse?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Soap.AddAccountPartyRelationshipBehavior.AddAccountPartyRelationship");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call SOAP Repository AddAccountPartyRelationship(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTO
                    _accountPartyRelationship = null;
                }

                if (_messageResponse?.ResponseCode != "Success" && _messageResponse?.ErrorMessage?.Length > 0)
                {
                    result.Result = false;
                    result.AddMessage(MessageType.Warning, _messageResponse.ErrorMessage);
                }
            }

            return result;
        }

        public AddAccountPartyRelationship GetDto(Applicant applicant)
        {
            var accountPartyRelationship = new AddAccountPartyRelationship()
            {
                Message = GetMessage(applicant)
            };

            return accountPartyRelationship;
        }

        #endregion

        #region "DTO Builder Methods"

        private Dto.Soap.Message GetMessage(Applicant applicant)
        {
            var message = new Dto.Soap.Message()
            {
                DataUpdate = GetDataUpdate(applicant)
            };

            return message;
        }

        private DataUpdate GetDataUpdate(Applicant applicant)
        {
            var dataUpdate = new DataUpdate()
            {
                TraceNumber = _app.ApplicationId.ToString(),
                ProcessingCode = "ExternalUpdateRequest",
                Source = "LoanOrigination",
                UpdateAction = "Indeterminate",
                UpdateTarget = "AccountPartyRelationships",
                AccountPartyRelationships = GetAccountPartyRelationships(applicant)
            };

            return dataUpdate;
        }

        private List<AccountPartyRelationship> GetAccountPartyRelationships(Applicant applicant)
        {
            var accountPartyRelationshipList = new List<AccountPartyRelationship>();

            var accountPartyRelationship = new AccountPartyRelationship()
            {
                UpdateAction = "Add",
                AccountNumber = _app.CreditCardNumber,
                PartyNumber = applicant.PersonNumber,
                PartyType = applicant.IsOrganization ? "Organization" : "Person",
                AccountRelationshipType = applicant.ApplicantTypeId == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Guarantor ? "CoMaker" : "Owner",
                SendStatement = applicant.HostValues.Any(h => h.Field1.Equals("AddAccountPartyRelationship.Message.DataUpdate.AccountPartyRelationships.AccountPartyRelationship.SendStatement")) ? string.Empty : null,
                StatementDeliveryChannel = applicant.HostValues.Any(h => h.Field1.Equals("AddAccountPartyRelationship.Message.DataUpdate.AccountPartyRelationships.AccountPartyRelationship.StatementDeliveryChannel")) ? string.Empty : null
            };

            if (_app.DisbursedDate.HasValue)
            {
                accountPartyRelationship.OriginationDate = _app.DisbursedDate.Value.ToString("yyyy-MM-dd");
            }
            else if (_app.HostValues.Any(h => h.Field1.Equals("AddAccountPartyRelationship.Message.DataUpdate.AccountPartyRelationships.AccountPartyRelationship.OriginationDate")))
            {
                accountPartyRelationship.OriginationDate = string.Empty;
            }
            else
            {
                accountPartyRelationship.OriginationDate = null;
            }

            accountPartyRelationshipList.Add(accountPartyRelationship);

            return accountPartyRelationshipList;
        }

        #endregion
    }
}
