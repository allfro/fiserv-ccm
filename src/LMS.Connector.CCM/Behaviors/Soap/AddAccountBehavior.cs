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
    public class AddAccountBehavior : Behavior, IAddAccount
    {
        private Application _app;
        private string _userToken;
        private AddAccount _account;
        private MessageResponse _messageResponse;

        public AddAccount Account
        {
            get
            {
                return _account;
            }
            set
            {
                _account = value;
            }
        }

        public AddAccountBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = new SoapRepository(userToken);
            _lmsRepository = new LmsRepository(userToken);
        }

        public AddAccountBehavior(Application app, string userToken, ISoapRepository serviceRepository)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
            _lmsRepository = new LmsRepository(userToken);
        }

        public AddAccountBehavior(Application app, string userToken, ISoapRepository serviceRepository, ILmsRepository lmsRepository)
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

        public BaseResult AddAccount(Applicant applicant)
        {
            var result = new BaseResult();
            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Soap.AddAccountBehavior.AddAccount"))
            {
                tr.Log($"AddAccount for ApplicantId {applicant.ApplicantId}, PersonNumber => {applicant.PersonNumber}");

                tr.Log($"AddAccount _account null? => {_account == null}");
                if (_account == null)
                {
                    tr.Log("Call GetDto() to get new _account");
                    _account = GetDto(applicant);
                }
                tr.LogObject(_account);

                try
                {
                    tr.Log("Calling ISoapRepository.AddAccount");
                    _messageResponse = _soapRepository.AddAccount(_account, _app, applicant);

                    tr.Log($"_messageResponse.AccountNumber = {_messageResponse?.AccountNumber}");
                    tr.Log($"_messageResponse.ResponseCode = {_messageResponse?.ResponseCode}");
                    tr.Log($"_messageResponse.ErrorMessage = {_messageResponse?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Soap.AddAccountBehavior.AddAccount");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call SOAP Repository AddAcount(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTO
                    _account = null;
                }

                if (_messageResponse?.ResponseCode != "Success" && _messageResponse?.ErrorMessage?.Length > 0)
                {
                    result.Result = false;
                    result.AddMessage(MessageType.Error, _messageResponse.ErrorMessage);
                }
            }

            return result;
        }

        public AddAccount GetDto(Applicant applicant)
        {
            var account = new AddAccount()
            {
                Message = GetMessage(applicant)
            };

            return account;
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
                UpdateAction = "Add",
                UpdateTarget = "Account",
                Account = GetAccount(applicant)
            };

            return dataUpdate;
        }

        private Account GetAccount(Applicant applicant)
        {
            var account = new Account()
            {
                LoanOfficerName = _lmsRepository.GetUserFullNameById(_app.FinalDecisionUserId.GetValueOrDefault()),
                ProductName = _app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.ProductName")) ? string.Empty : null,
                RateClass = _app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.RateClass")) ? string.Empty : null,
                PromotionalRateClass = _app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.PromotionalRateClass")) ? string.Empty : null,
                PromotionalRateClassStartDate = _app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.PromotionalRateClassStartDate")) ? string.Empty : null,
                PromotionalRateClassEndDate = _app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.PromotionalRateClassEndDate")) ? string.Empty : null,
                CreditLimit = _app.FinalLoanAmount.GetValueOrDefault(),
                TaxOwnerPartyId = applicant.PersonNumber,
                TaxOwnerPartyType = applicant.IsOrganization ? "Organization" : "Person",
                UserFields = GetUserFields(applicant)
            };

            if (_app.DisbursedDate.HasValue)
            {
                account.AccountOpenDate = _app.DisbursedDate.Value.ToString("yyyy-MM-dd");
            }
            else if (_app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.AccountOpenDate")))
            {
                account.AccountOpenDate = string.Empty;
            }
            else
            {
                account.AccountOpenDate = null;
            }

            return account;
        }

        /// <summary>
        /// Manually creates a collection of UserFields, if needed, from entity host values.
        /// When a host value is added to the UserField collection, remove that host value from the entity from which it was defined.
        /// </summary>
        /// <param name="applicant"></param>
        /// <remarks>
        /// This method already has access to application-level host values.
        /// Optionally giving it the ability to access applicant-level host values.
        /// </remarks>
        /// <returns></returns>
        public List<UserField> GetUserFields(Applicant applicant)
        {
            List<UserField> userFieldList = null;

            var userFieldHVs = _app.HostValues.Where(hv => hv.Field1.Contains("AddAccount.Message.DataUpdate.Account.UserFields.UserField"));
            var userFieldHVGs = userFieldHVs.GroupBy(g => g.Field2);

            if (userFieldHVGs.Any())
            {
                userFieldList = new List<UserField>();

                foreach (var userFieldHVG in userFieldHVGs)
                {
                    var userField = new UserField();

                    foreach (var hv in userFieldHVG)
                    {
                        if (hv.Field1.Contains("UserField.Name"))
                            userField.Name = hv.Value;
                        else if (hv.Field1.Contains("UserField.Value"))
                            userField.Value = hv.Value;

                        /* Remove this host value from the entity since it was already manually added
                         * and doesn't need to be "re-added" by the host value translator.
                         */
                        _app.HostValues.Remove(hv);
                    }

                    userFieldList.Add(userField);
                }
            }

            return userFieldList;
        }

        #endregion
    }
}
