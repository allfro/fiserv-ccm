using System;
using System.Collections.Generic;
using System.Linq;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors.Soap
{
    public class UpdateAccountBehavior : Behavior, IUpdateAccount
    {
        private Application _app;
        private string _userToken;
        private UpdateAccount _account;
        private MessageResponse _messageResponse;

        public UpdateAccount Account
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

        public UpdateAccountBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = new SoapRepository(userToken);
        }

        public UpdateAccountBehavior(Application app, string userToken, ISoapRepository serviceRepository)
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

        public BaseResult UpdateAccount(Applicant primaryApplicant)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Soap.UpdateAccountBehavior.UpdateAccount"))
            {
                tr.Log($"UpdateAccount for ApplicantId {primaryApplicant.ApplicantId}, PersonNumber => {primaryApplicant.PersonNumber}");

                tr.Log($"UpdateAccount _account null? => {_account == null}");
                if (_account == null)
                {
                    tr.Log("Call GetDto() to get new _account");
                    _account = GetDto(primaryApplicant);
                }
                tr.LogObject(_account);

                try
                {
                    tr.Log("Calling ISoapRepository.UpdateAccount");
                    _messageResponse = _soapRepository.UpdateAccount(_account, _app, primaryApplicant);

                    tr.Log($"_messageResponse.ResponseCode = {_messageResponse?.ResponseCode}");
                    tr.Log($"_messageResponse.ErrorMessage = {_messageResponse?.ErrorMessage}");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Soap.UpdateAccountBehavior.UpdateAccount");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call SOAP Repository UpdateAccount(): {ex.Message}");
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

        public UpdateAccount GetDto(Applicant primaryApplicant)
        {
            var account = new UpdateAccount()
            {
                Message = GetMessage(primaryApplicant)
            };

            return account;
        }

        #endregion

        #region "DTO Builder Methods"

        private Dto.Soap.Message GetMessage(Applicant primaryApplicant)
        {
            var message = new Dto.Soap.Message()
            {
                DataUpdate = GetDataUpdate(primaryApplicant)
            };

            return message;
        }

        private DataUpdate GetDataUpdate(Applicant primaryApplicant)
        {
            var dataUpdate = new DataUpdate()
            {
                TraceNumber = _app.ApplicationId.ToString(),
                ProcessingCode = "ExternalUpdateRequest",
                Source = "LoanOrigination",
                UpdateAction = "Modify",
                UpdateTarget = "Account",
                Account = GetAccount(primaryApplicant),
                ModifiedFields = GetModifiedFields(primaryApplicant)
            };

            return dataUpdate;
        }

        private Account GetAccount(Applicant primaryApplicant)
        {
            var account = new Account()
            {
                AccountNumber = _app.CreditCardNumber,
                LoanOfficerName = _app.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.LoanOfficerName")) ? string.Empty : null,
                AccountOpenDate = _app.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.AccountOpenDate")) ? string.Empty : null,
                LockoutStatus = _app.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.LockoutStatus")) ? string.Empty : null,
                LockoutReason = _app.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.LockoutReason")) ? string.Empty : null,
                RateClass = _app.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.RateClass")) ? string.Empty : null,
                PromotionalRateClass = _app.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.PromotionalRateClass")) ? string.Empty : null,
                PromotionalRateClassStartDate = _app.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.PromotionalRateClassStartDate")) ? string.Empty : null,
                PromotionalRateClassEndDate = _app.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.PromotionalRateClassEndDate")) ? string.Empty : null,
                CreditLimit = _app.FinalLoanAmount.GetValueOrDefault(),
                TaxOwnerPartyId = primaryApplicant.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.TaxOwnerPartyId")) ? string.Empty : null,
                TaxOwnerPartyType = primaryApplicant.HostValues.Any(h => h.Field1.Equals("UpdateAccount.Message.DataUpdate.Account.TaxOwnerPartyType")) ? string.Empty : null,
                UserFields = GetUserFields(primaryApplicant)
            };

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

            var userFieldHVs = _app.HostValues.Where(hv => hv.Field1.Contains("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField"));
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

        private ModifiedFields GetModifiedFields(Applicant primaryApplicant)
        {
            var modifiedFields = new ModifiedFields()
            {
                AccountField = "CreditLimit"
            };

            return modifiedFields;
        }

        #endregion
    }
}
