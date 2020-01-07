using System;
using System.Linq;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Core.Lookups;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors.Soap
{
    public class AddCardBehavior : Behavior, IAddCard
    {
        private Application _app;
        private string _userToken;
        private AddCard _card;
        private MessageResponse _messageResponse;

        public AddCard Card
        {
            get
            {
                return _card;
            }
            set
            {
                _card = value;
            }
        }

        public AddCardBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = new SoapRepository(userToken);
            _lmsRepository = new LmsRepository(userToken);
        }

        public AddCardBehavior(Application app, string userToken, ISoapRepository serviceRepository)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
            _lmsRepository = new LmsRepository(userToken);
        }

        public AddCardBehavior(Application app, string userToken, ISoapRepository serviceRepository, ILmsRepository lmsRepository)
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

        public BaseResult AddCard(LmsPerson lmsPerson)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Soap.AddCardBehavior.AddCard"))
            {
                if (lmsPerson.Applicant != null)
                {
                    tr.Log($"AddCard for ApplicantId {lmsPerson.Applicant.ApplicantId}, PersonNumber => {lmsPerson.Applicant.PersonNumber}");
                }
                else if (lmsPerson.AuthorizedUser != null)
                {
                    tr.Log($"AddCard for AuthorizedUserId {lmsPerson.AuthorizedUser.AuthorizedUserId}, PersonNumber => {lmsPerson.AuthorizedUser.PersonNumber}");
                }

                tr.Log($"AddCard _card null? => {_card == null}");
                if (_card == null)
                {
                    tr.Log("Call GetDto() to get new _card");
                    _card = GetDto(lmsPerson);
                }
                tr.LogObject(_card);

                try
                {
                    tr.Log("Calling ISoapRepository.Addcard");
                    _messageResponse = _soapRepository.AddCard(_card, _app, lmsPerson);

                    tr.Log($"_messageResponse.CardNumber = {_messageResponse?.CardNumber}");
                    tr.Log($"_messageResponse.ResponseCode = {_messageResponse?.ResponseCode}");
                    tr.Log($"_messageResponse.ErrorMessage = {_messageResponse?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Soap.AddCardBehavior.AddCard");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call SOAP Repository AddCard(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTO
                    _card = null;
                }

                if (_messageResponse?.ResponseCode != "Success" && _messageResponse?.ErrorMessage?.Length > 0)
                {
                    result.Result = false;
                    result.AddMessage(MessageType.Warning, _messageResponse.ErrorMessage);
                }
            }

            return result;
        }

        public AddCard GetDto(LmsPerson lmsPerson)
        {
            var card = new AddCard()
            {
                Message = GetMessage(lmsPerson)
            };

            return card;
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
                UpdateTarget = "Card",
                Card = GetCard(lmsPerson)
            };

            return dataUpdate;
        }

        private Card GetCard(LmsPerson lmsPerson)
        {
            var applicant = lmsPerson.Applicant;
            var authorizedUser = lmsPerson.AuthorizedUser;

            var card = new Card()
            {
                AccountNumber = _app.CreditCardNumber,
                CardOrderType = _app.IsInstantIssueCard ? "InstantIssue" : "Batch"
            };

            if (applicant != null && authorizedUser == null)
            {
                card.EmbossingLine1 = applicant.HostValues.Any(h => h.Field1.Equals("AddCard.Message.DataUpdate.Card.EmbossingLine1")) ? string.Empty : null;
                card.EmbossingLine2 = applicant.HostValues.Any(h => h.Field1.Equals("AddCard.Message.DataUpdate.Card.EmbossingLine2")) ? string.Empty : null;
                card.CardholderPartyNumber = applicant.PersonNumber;
                card.CardholderPartyType = applicant.IsOrganization ? "Organization" : "Person";
                card.PinOffset = applicant.HostValues.Any(h => h.Field1.Equals("AddCard.Message.DataUpdate.Card.PinOffset")) ? string.Empty : null;
                card.Reissue = applicant.HostValues.Any(h => h.Field1.Equals("AddCard.Message.DataUpdate.Card.Reissue")) ? string.Empty : null;
                card.CardDesignName = _lmsRepository.GetLookupCodeById(applicant.CreditCardDesignId.GetValueOrDefault(), LookupTypes.CardDesignType);
            }
            else if (applicant == null && authorizedUser != null)
            {
                card.EmbossingLine1 = authorizedUser.HostValues.Any(h => h.Field1.Equals("AddCard.Message.DataUpdate.Card.EmbossingLine1")) ? string.Empty : null;
                card.EmbossingLine2 = authorizedUser.HostValues.Any(h => h.Field1.Equals("AddCard.Message.DataUpdate.Card.EmbossingLine2")) ? string.Empty : null;
                card.CardholderPartyNumber = authorizedUser.PersonNumber;
                card.CardholderPartyType = "Person";
                card.PinOffset = authorizedUser.HostValues.Any(h => h.Field1.Equals("AddCard.Message.DataUpdate.Card.PinOffset")) ? string.Empty : null;
                card.Reissue = authorizedUser.HostValues.Any(h => h.Field1.Equals("AddCard.Message.DataUpdate.Card.Reissue")) ? string.Empty : null;
                card.CardDesignName = _lmsRepository.GetLookupCodeById(authorizedUser.CreditCardDesignId.GetValueOrDefault(), LookupTypes.CardDesignType);
            }

            return card;
        }

        #endregion
    }
}
