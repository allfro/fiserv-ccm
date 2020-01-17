using System;
using System.Collections.Generic;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors.Rest
{
    public class InquiryBehavior : Behavior, IInquiry
    {
        private Application _app;
        private string _userToken;
        private PartyRelationshipsInquiry _inquiry;
        private IList<RelationshipInfo> _relationshipInfos;
        private string _errorMessage;

        public PartyRelationshipsInquiry PartyRelationshipInquiry
        {
            get
            {
                return _inquiry;
            }
            set
            {
                _inquiry = value;
            }
        }

        public InquiryBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _restRepository = new RestRepository(userToken);

        }

        public InquiryBehavior(Application app, string userToken, IRestRepository repo)
        {
            _app = app;
            _userToken = userToken;
            _restRepository = repo;
        }

        #region "Interface implementations"

        public IList<RelationshipInfo> RelationshipInfos
        {
            get
            {
                return _relationshipInfos;
            }
            set
            {
                _relationshipInfos = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        public BaseResult Inquiry(string personNumber)
        {
            var result = new BaseResult();
            _errorMessage = "Relationship not found";

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Rest.InquiryBehavior.Inquiry"))
            {
                tr.Log($"Inquiry personNumber = {personNumber}");

                tr.Log($"PartyRelationshipsInquiry _inquiry null? => {_inquiry == null}");
                if (_inquiry == null)
                {
                    tr.Log("Call GetDto() to get new _inquiry");
                    _inquiry = GetDto(personNumber);
                }
                tr.LogObject(_inquiry);

                try
                {
                    tr.Log("Calling IRestRepository.MakeInquiry");
                    _relationshipInfos = _restRepository.MakeInquiry(_inquiry);
                    tr.LogObject(_relationshipInfos);
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Rest.InquiryBehavior.Inquiry");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call REST Repository MakeInquiry(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTO
                    _inquiry = null;
                }

                if (_relationshipInfos?.Count > 0)
                {
                    int i = 0;
                    tr.Log($"Iterating through _relationshipInfos to determine if party has an account in CCM (i.e. relationshipInfo[i].AccountNumber == _app.CreditCardNumber <{_app.CreditCardNumber}>)");
                    foreach (var relationshipInfo in _relationshipInfos)
                    {
                        tr.Log($"_relationshipInfos[{i}].AccountNumber = {relationshipInfo.AccountNumber}");
                        if (relationshipInfo.AccountNumber == _app.CreditCardNumber)
                        {
                            _errorMessage = "Found";
                            tr.Log($"Found in _relationshipInfos[{i}]");

                            break;
                        }

                        i++;
                    }
                }

                tr.Log($"Inquiry error message: {_errorMessage}");
            }

            return result;
        }

        public PartyRelationshipsInquiry GetDto(string personNumber)
        {
            var inquiry = new PartyRelationshipsInquiry()
            {
                PartyId = Convert.ToInt32(personNumber)
            };

            return inquiry;
        }

        #endregion
    }
}
