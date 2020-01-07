using System.Collections.Generic;
using System.Linq;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Core.Lookups;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;

namespace LMS.Connector.CCM
{
    /// <summary>
    /// Encapsulates top-level CCM behavior calls.
    /// </summary>
    public class CCM
    {
        private Application _app;
        private string _userToken;

        public Application Application
        {
            get
            {
                return _app;
            }
            set
            {
                _app = value;
            }
        }

        public string UserToken
        {
            get
            {
                return _userToken;
            }
            set
            {
                _userToken = value;
            }
        }

        public CCM()
        {

        }

        public CCM(Application application, string userToken)
        {
            _app = application;
            _userToken = userToken;
        }

        /// <summary>
        /// Makes an Inquiry behavior call to determine if a person exists in CCM, and also gets RelationshipInfo objects.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="personNumber"></param>
        /// <param name="errorMessage"></param>
        /// <param name="relationshipInfos"></param>
        /// <returns></returns>
        public BaseResult MakeInquiry(ClientStrategy strategy, string personNumber, out string errorMessage, out IList<RelationshipInfo> relationshipInfos)
        {
            var result = new BaseResult();

            var inquiryResult = strategy.Inquiry(personNumber, out errorMessage, out relationshipInfos);
            result.AppendResult(inquiryResult);

            if (!inquiryResult.Result)
            {
                result.AddMessage(MessageType.Warning, $"An error occured making Inquiry for personNumber {personNumber}.");
            }

            return result;
        }

        /// <summary>
        /// Adds a person to CCM using AddPerson behavior call.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="applicant"></param>
        /// <returns></returns>
        public BaseResult AddPerson(ClientStrategy strategy, Applicant applicant)
        {
            var result = new BaseResult();
            var lmsPerson = new LmsPerson()
            {
                Applicant = applicant
            };

            var addPersonStrategyResult = strategy.AddPerson(lmsPerson);
            result.AppendResult(addPersonStrategyResult);

            if (!addPersonStrategyResult.Result)
            {
                if (applicant.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Joint)
                {
                    string applicantTypeName = LookupCodes.ApplicantType.Joint;
                    result.AddMessage(MessageType.Warning, $"An error occured adding the {applicantTypeName} Applicant Type. {applicant.FullName} will not be added to CCM.");
                }
                else
                {
                    result.AddError("An error occurred creating the credit card record for the loan. The credit card will not be created in CCM.");

                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds an organization to CCM using AddOrganization behavior call.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="applicant"></param>
        /// <returns></returns>
        public BaseResult AddOrganization(ClientStrategy strategy, Applicant applicant)
        {
            var result = new BaseResult();

            var addOrganizationStrategyResult = strategy.AddOrganization(applicant);
            result.AppendResult(addOrganizationStrategyResult);

            if (!addOrganizationStrategyResult.Result)
            {
                if (applicant.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Joint)
                {
                    string applicantTypeName = LookupCodes.ApplicantType.Joint;
                    result.AddMessage(MessageType.Warning, $"An error occured adding the {applicantTypeName} Applicant Type. {applicant.OrganizationName} will not be added to CCM.");
                }
                else
                {
                    result.AddError("An error occurred creating the credit card record for the loan. The credit card will not be created in CCM.");

                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds an authorized user to CCM using AddPerson behavior call.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="authorizedUser"></param>
        /// <returns></returns>
        public BaseResult AddAuthorizedUser(ClientStrategy strategy, AuthorizedUser authorizedUser)
        {
            var result = new BaseResult();
            var lmsPerson = new LmsPerson()
            {
                AuthorizedUser = authorizedUser
            };

            var addAuthorizedUserStrategyResult = strategy.AddPerson(lmsPerson);
            result.AppendResult(addAuthorizedUserStrategyResult);

            if (!addAuthorizedUserStrategyResult.Result)
            {
                result.AddMessage(MessageType.Warning, $"An error occured adding the AUTHORIZED USER. {authorizedUser.FirstName} {authorizedUser.LastName} will not be added to CCM.");
            }

            return result;
        }

        /// <summary>
        /// Creates an account in CCM for the primary applicant using AddAccount behavior call. Then sets the applicant's account number in LMS.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="applicant"></param>
        /// <returns></returns>
        public BaseResult AddAccount(ClientStrategy strategy, Applicant applicant)
        {
            var result = new BaseResult();
            string accountNumber = string.Empty;

            var addAccountStrategyResult = strategy.AddAccount(applicant, out accountNumber);
            result.AppendResult(addAccountStrategyResult);

            if (addAccountStrategyResult.Result)
            {
                // Store the CCM AccountNumber received from the response to the application CreditCardNumber field
                _app.CreditCardNumber = accountNumber;
            }
            else
            {
                result.AddError("An error occurred creating the credit card record for the loan. The credit card will not be created in CCM.");
            }

            return result;
        }

        /// <summary>
        /// Updates the account of a primary applicant in CCM using UpdateAccount behavior call.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="applicant"></param>
        /// <returns></returns>
        public BaseResult UpdateAccount(ClientStrategy strategy, Applicant applicant)
        {
            var result = new BaseResult();

            var updateAccountResult = strategy.UpdateAccount(applicant);
            result.AppendResult(updateAccountResult);

            if (!result.Result)
            {
                result.AddError("An error occured increasing the line of credit. The update will not be reflected in CCM.");
            }

            return result;
        }

        /// <summary>
        /// Adds a party relationship for a joint applicant in CCM using AddAccountPartyRelationship behavior call.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="applicant"></param>
        /// <returns></returns>
        public BaseResult AddAccountPartyRelationship(ClientStrategy strategy, Applicant applicant)
        {
            var result = new BaseResult();
            var addAccountPartyRelationshipStrategyResult = strategy.AddAccountPartyRelationship(applicant);
            result.AppendResult(addAccountPartyRelationshipStrategyResult);

            if (!addAccountPartyRelationshipStrategyResult.Result)
            {
                var jointApplicantName = (applicant.IsOrganization) ? applicant.OrganizationName : applicant.FullName;
                result.AddMessage(MessageType.Warning, $"An error occured adding the Joint. {jointApplicantName} will not be added on CCM.");
            }

            return result;
        }

        /// <summary>
        /// Creates a card for the primary or joint applicant using AddCard behavior call. Then sets the applicant's card number in LMS.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="primaryOrJointApplicant"></param>
        /// <returns></returns>
        public BaseResult AddCard(ClientStrategy strategy, Applicant primaryOrJointApplicant)
        {
            var result = new BaseResult();
            var lmsPerson = new LmsPerson()
            {
                Applicant = primaryOrJointApplicant
            };
            string cardNumber = string.Empty;

            var addCardStrategyResult = strategy.AddCard(lmsPerson, out cardNumber);
            result.AppendResult(addCardStrategyResult);

            if (addCardStrategyResult.Result)
            {
                // Store the CCM AccountNumber received from the response to the primary or joint applicant CardNumber field
                var applicant = _app.Applicants.SingleOrDefault(a => a.ApplicantId == primaryOrJointApplicant.ApplicantId);
                applicant.CardNumber = cardNumber;
            }
            else
            {
                var applicantName = (primaryOrJointApplicant.IsOrganization) ? primaryOrJointApplicant.OrganizationName : primaryOrJointApplicant.FullName;
                result.AddMessage(MessageType.Warning, $"An error occured creating the plastic card. The card will not be created on CCM for {applicantName}.");
                result.Result = true;
            }

            return result;
        }

        /// <summary>
        /// Creates a card for the authorized user using AddCard behavior call. Then sets the authorized user's card number in LMS.
        /// </summary>
        /// <param name="strategy"></param>
        /// <param name="authorizedUser"></param>
        /// <returns></returns>
        public BaseResult AddCard(ClientStrategy strategy, AuthorizedUser authorizedUser)
        {
            var result = new BaseResult();
            var lmsPerson = new LmsPerson()
            {
                AuthorizedUser = authorizedUser
            };
            string cardNumber = string.Empty;

            var addCardResult = strategy.AddCard(lmsPerson, out cardNumber);
            result.AppendResult(addCardResult);

            if (addCardResult.Result)
            {
                //Store the CCM AccountNumber received from the response to the authorized user CardNumber field
                var authUser = _app.AuthorizedUsers.SingleOrDefault(a => a.AuthorizedUserId == authorizedUser.AuthorizedUserId);
                authUser.CardNumber = cardNumber;
            }
            else
            {
                result.AddMessage(MessageType.Warning, $"An error occured creating the plastic card. The card will not be created on CCM for {authorizedUser.FirstName} {authorizedUser.LastName}.");
                result.Result = true;
            }

            return result;
        }
    }
}
