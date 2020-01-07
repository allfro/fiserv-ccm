using System;
using System.Collections.Generic;
using System.Linq;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Lookups;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM
{
    /// <summary>
    /// Calls the CCM behaviors for origination.
    /// </summary>
    public class CCMOrigination
    {
        private Application _app;
        private string _userToken;
        private CCM _ccm;
        private SoapStrategy _soapStrategy;
        private RestStrategy _restStrategy;
        private ISoapRepository _soapRepository;
        private IRestRepository _restRepository;
        private Credentials _soapCredentials;
        private Credentials _restCredentials;

        /**
         * Keeps track of applicants and authorized users added to the CCM account.
         * Members in these lists will need a card created for them.
         * */
        private List<Applicant> _applicantsAddedToCCM = new List<Applicant>();
        private List<AuthorizedUser> _authorizedUsersAddedToCCM = new List<AuthorizedUser>();

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

        public CCM CCM
        {
            get
            {
                return _ccm;
            }
            set
            {
                _ccm = value;
            }
        }

        public SoapStrategy SoapStrategy
        {
            get
            {
                return _soapStrategy;
            }
            set
            {
                _soapStrategy = value;
            }
        }

        public RestStrategy RestStrategy
        {
            get
            {
                return _restStrategy;
            }
            set
            {
                _restStrategy = value;
            }
        }

        public ISoapRepository SoapRepository
        {
            get
            {
                return _soapRepository;
            }
            set
            {
                _soapRepository = value;
            }
        }

        public IRestRepository RestRepository
        {
            get
            {
                return _restRepository;
            }
            set
            {
                _restRepository = value;
            }
        }

        public Credentials SoapCredentials
        {
            get
            {
                return _soapCredentials;
            }
            set
            {
                _soapCredentials = value;
            }
        }

        public Credentials RestCredentials
        {
            get
            {
                return _restCredentials;
            }
            set
            {
                _restCredentials = value;
            }
        }

        public CCMOrigination()
        {
            _ccm = new CCM();
        }

        public CCMOrigination(string userToken)
        {
            _app = new Application();
            _userToken = userToken;
            _ccm = new CCM(_app, userToken);
            _restStrategy = new RestStrategy(_app, userToken);
            _soapRepository = new SoapRepository(userToken);
            _soapStrategy = new SoapStrategy(_app, userToken, _soapRepository);
        }

        public CCMOrigination(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _ccm = new CCM(app, userToken);
            _restStrategy = new RestStrategy(app, userToken);
            _soapStrategy = new SoapStrategy(app, userToken);
        }

        public CCMOrigination(Application app, string userToken, SoapStrategy soapStrategy)
        {
            _app = app;
            _userToken = userToken;
            _ccm = new CCM(app, userToken);
            _restStrategy = new RestStrategy(app, userToken);
            _soapStrategy = soapStrategy;
        }

        public CCMOrigination(Application app, string userToken, RestStrategy restStrategy)
        {
            _app = app;
            _userToken = userToken;
            _ccm = new CCM(app, userToken);
            _restStrategy = restStrategy;
            _soapStrategy = new SoapStrategy(app, userToken);
        }

        public CCMOrigination(Application app, string userToken, RestStrategy restStrategy, SoapStrategy soapStrategy)
        {
            _app = app;
            _userToken = userToken;
            _ccm = new CCM(app, userToken);
            _restStrategy = restStrategy;
            _soapStrategy = soapStrategy;
        }

        public CCMOrigination(Application app, string userToken, CCM ccm, RestStrategy restStrategy, SoapStrategy soapStrategy)
        {
            _app = app;
            _userToken = userToken;
            _ccm = ccm;
            _restStrategy = restStrategy;
            _soapStrategy = soapStrategy;
        }

        #region "Test / Check Connectivity"

        /// <summary>
        /// Tests the connections to CCM SOAP and REST services.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="facility"></param>
        /// <param name="soapServiceUrl"></param>
        /// <param name="restServiceUrl"></param>
        /// <returns></returns>
        public BaseResult TestConnections(string userName, string password, string facility, string soapServiceUrl, string restServiceUrl)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.CCMOrigination.TestConnections"))
            {
                try
                {
                    var soapConnectionResult = TestSoapConnection(soapServiceUrl, userName, password, facility);
                    result.AppendResult(soapConnectionResult);
                    tr.Log($"SOAP connection result = {soapConnectionResult.Result}");

                    var restConnectionResult = TestRestConnection(restServiceUrl, userName, password, facility);
                    result.AppendResult(restConnectionResult);
                    tr.Log($"REST connection result = {restConnectionResult.Result}");

                    result.Result = soapConnectionResult.Result && restConnectionResult.Result;
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.CCMOrigination.TestConnections");
                    result.AddMessage(MessageType.Error, ex.Message);
                    tr.LogException(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Tests the connection to CCM SOAP service using TestConnection behavior call.
        /// </summary>
        /// <param name="soapServiceUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="facility"></param>
        /// <returns></returns>
        public BaseResult TestSoapConnection(string soapServiceUrl, string userName, string password, string facility)
        {
            var result = new BaseResult();

            var soapConnectionResult = _soapStrategy.TestConnection(soapServiceUrl, userName, password, facility);
            result.AppendResult(soapConnectionResult);

            return result;
        }

        /// <summary>
        /// Tests the connection to CCM REST service using TestConnection behavior call.
        /// </summary>
        /// <param name="restServiceUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="facility"></param>
        /// <returns></returns>
        public BaseResult TestRestConnection(string restServiceUrl, string userName, string password, string facility)
        {
            var result = new BaseResult();

            var restConnectionResult = _restStrategy.TestConnection(restServiceUrl, userName, password, facility);
            result.AppendResult(restConnectionResult);

            return result;
        }

        #endregion

        #region "Multi-step Disbursement process"

        /// <summary>
        /// The multi-step process to create a new credit card application and update existing credit cards.
        /// </summary>
        /// <returns></returns>
        public BaseResult DisburseApplication()
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.CCMOrigination.DisburseApplication"))
            {
                tr.Log($"Application {_app.ApplicationId} IsAddon => {_app.IsAddon}");

                #region "AddOn == false"
                if (!_app.IsAddon)
                {
                    var primaryJointGuarantorApplicants = _app.Applicants.Where(
                        a =>
                            a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary
                            || a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Joint
                            || a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Guarantor
                    );

                    result = DisburseNonAddOn(primaryJointGuarantorApplicants);
                }
                #endregion
                #region "AddOn == true"
                else
                {
                    var primaryApplicant = _app.Applicants.SingleOrDefault(
                        a => a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary
                    );

                    var jointGuarantorApplicants = _app.Applicants.Where(
                        a => a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Joint
                        || a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Guarantor
                    );

                    result = DisburseAddOn(primaryApplicant, jointGuarantorApplicants);
                }
                #endregion
            }

            return result;
        }

        /// <summary>
        /// Process for disbursing an application that is not an Add-On.
        /// </summary>
        /// <param name="primaryJointGuarantorApplicants"></param>
        /// <returns></returns>
        public BaseResult DisburseNonAddOn(IEnumerable<Applicant> primaryJointGuarantorApplicants)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.CCMOrigination.DisburseNonAddOn"))
            {
                // Primary, joint, or guarantor applicants
                if (primaryJointGuarantorApplicants?.Any() == true)
                {
                    tr.Log($"Number of primary, joint, or guarantor applicants in ApplicationId {_app.ApplicationId} => {primaryJointGuarantorApplicants.Count()}");

                    foreach (var primaryJointGuarantorApplicant in primaryJointGuarantorApplicants)
                    {
                        /*********************
                         * Call AddPerson/AddOrganization on this person or organization, respectively.
                         */
                        tr.Log($"Adding ApplicantId {primaryJointGuarantorApplicant.ApplicantId} as a person or organization in CCM");
                        tr.Log($"ApplicantId {primaryJointGuarantorApplicant.ApplicantId} is ApplicantTypeId {primaryJointGuarantorApplicant.ApplicantTypeId}");

                        if (!primaryJointGuarantorApplicant.IsOrganization)
                        {
                            // Add the person applicant to CCM
                            var addPersonResult = _ccm.AddPerson(_soapStrategy, primaryJointGuarantorApplicant);
                            result.AppendResult(addPersonResult);
                            tr.Log($"AddPerson result for ApplicantId {primaryJointGuarantorApplicant.ApplicantId} => {addPersonResult.Result}");

                            if (addPersonResult.Result)
                            {
                                // Add this applicant to list of applicants that will need a card created
                                _applicantsAddedToCCM.Add(primaryJointGuarantorApplicant);
                            }
                        }
                        else
                        {
                            // Guarantors cannot be organizations
                            if (primaryJointGuarantorApplicant.ApplicantTypeId != (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Guarantor)
                            {
                                var primaryJointApplicant = primaryJointGuarantorApplicant;

                                // Add the organization applicant to CCM
                                var addOrganizationResult = _ccm.AddOrganization(_soapStrategy, primaryJointApplicant);
                                result.AppendResult(addOrganizationResult);
                                tr.Log($"AddOrganization result for ApplicantId {primaryJointApplicant.ApplicantId} => {addOrganizationResult.Result}");
                            }
                        }

                        if (!result.Result)
                        {
                            // Return to calling service if failure to AddPerson/AddOrganization on primary applicant
                            if (primaryJointGuarantorApplicant.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary)
                            {
                                _app.StatusId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicationStatus.DisbursementError;

                                return result;
                            }
                        }
                    }

                    tr.Log($"_applicantsAddedToCCM.Count = {_applicantsAddedToCCM.Count}");

                    if (_app.AuthorizedUsers?.Count > 0)
                    {
                        tr.Log($"Number of authorized users in ApplicationId {_app.ApplicationId} => {_app.AuthorizedUsers.Count}");

                        foreach (var authorizedUser in _app.AuthorizedUsers)
                        {
                            /*********************
                             * Call AddPerson on this authorized user.
                             */
                            tr.Log($"Adding AuthorizedUserId {authorizedUser.AuthorizedUserId} as a person in CCM");

                            // Add the authorized user to CCM
                            var addAuthorizedUserResult = _ccm.AddAuthorizedUser(_soapStrategy, authorizedUser);
                            result.AppendResult(addAuthorizedUserResult);
                            tr.Log($"AddAuthorizedUser result for AuthorizedUserId {authorizedUser.AuthorizedUserId} => {addAuthorizedUserResult.Result}");

                            if (addAuthorizedUserResult.Result)
                            {
                                // Add this authorized user to the list of authorized users who will need a card created
                                _authorizedUsersAddedToCCM.Add(authorizedUser);
                            }
                        }
                    }

                    tr.Log($"_authorizedUsersAddedToCCM.Count = {_authorizedUsersAddedToCCM.Count}");

                    /*********************
                     * Add Account to create a new credit card account from an application that has a primary applicant.
                     */
                    var primaryApplicant = primaryJointGuarantorApplicants.SingleOrDefault(
                        a => a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary
                    );

                    if (primaryApplicant != null)
                    {
                        tr.Log($"Creating an account for ApplicantId {primaryApplicant.ApplicantId} in CCM");

                        var addAccountResult = _ccm.AddAccount(_soapStrategy, primaryApplicant);
                        result.AppendResult(addAccountResult);
                        tr.Log($"AddAccount result for ApplicantId {primaryApplicant.ApplicationId} = {addAccountResult.Result}");

                        if (!addAccountResult.Result)
                        {
                            // Return to calling service if failure to AddAccount
                            _app.StatusId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicationStatus.DisbursementError;

                            return result;
                        }
                    }
                    else
                    {
                        /*
                         * No primary applicant in the application
                         */
                        tr.Log("No primary applicant in the application");

                        // Return to calling service if failure to AddAccount
                        _app.StatusId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicationStatus.DisbursementError;

                        return result;
                    }

                    /*********************
                     * Add Account Party Relationship for all joint and guarantor applicants.
                     */
                    var jointGuarantorApplicants = _app.Applicants.Where(
                        a => a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Joint
                        || a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Guarantor
                    );

                    if (jointGuarantorApplicants?.Any() == true)
                    {
                        foreach (var jointGuarantorApplicant in jointGuarantorApplicants)
                        {
                            tr.Log($"Creating an Account Party Relationship for ApplicantId {jointGuarantorApplicant.ApplicantId} in CCM");

                            // Add joint or guarantor to the account
                            var addAccountPartyRelationshipResult = _ccm.AddAccountPartyRelationship(_soapStrategy, jointGuarantorApplicant);
                            result.AppendResult(addAccountPartyRelationshipResult);
                            tr.Log($"AddAccountPartyRelationship result for ApplicantId {jointGuarantorApplicant.ApplicationId} => {addAccountPartyRelationshipResult.Result}");
                        }
                    }
                    else
                    {
                        /*
                         * No joint or gurantor applicants in application
                         */
                        tr.Log("There are no joint or gurantor applicants in the application");
                    }

                    /*********************
                     * Add Card for each non-organization primary or joint applicant that was successfully added to the credit card account (CCM account).
                     */
                    if (_applicantsAddedToCCM.Count > 0)
                    {
                        tr.Log($"applicantsAddedToCCM.Count = {_applicantsAddedToCCM.Count}");
                        var nonOrgPrimaryOrJointApplicants = _applicantsAddedToCCM.Where(
                            a => !a.IsOrganization &&
                            (
                                a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary
                                || a.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Joint
                            )
                        );

                        if (nonOrgPrimaryOrJointApplicants?.Any() == true)
                        {
                            foreach (var nonOrgPrimaryOrJointApplicant in nonOrgPrimaryOrJointApplicants)
                            {
                                /*********************
                                 * Add Card for the non-organization applicant.
                                 */
                                tr.Log($"Adding a card for ApplicantId {nonOrgPrimaryOrJointApplicant.ApplicantId} in CCM");

                                var addCardResult = _ccm.AddCard(_soapStrategy, nonOrgPrimaryOrJointApplicant);
                                result.AppendResult(addCardResult);
                                tr.Log($"AddCard result for ApplicantId {nonOrgPrimaryOrJointApplicant.ApplicationId} => {addCardResult.Result}");
                            }
                        }
                        else
                        {
                            tr.Log($"No cards will be created since there were no non-organization primary or joint applicants sucessfully added to CCM account");
                        }
                    }
                    else
                    {
                        tr.Log("No calls to AddCard for applicants since there were no applicants that were successfully added to CCM account");
                    }

                    /*********************
                     * Add Card for each authorized user that was successfully added to the credit card account (CCM account).
                     */
                    if (_authorizedUsersAddedToCCM.Count > 0)
                    {
                        tr.Log($"authorizedUsersAddedToCCM.Count = {_authorizedUsersAddedToCCM.Count}");

                        foreach (var authorizedUser in _authorizedUsersAddedToCCM)
                        {
                            /*********************
                             * Add Card for the authorized user.
                             */
                            tr.Log($"Adding a card for AuthorizedUserId {authorizedUser.AuthorizedUserId} in CCM");

                            var addCardResult = _ccm.AddCard(_soapStrategy, authorizedUser);
                            result.AppendResult(addCardResult);
                            tr.Log($"AddCard result for AuthorizedUserId {authorizedUser.AuthorizedUserId} => {addCardResult.Result}");
                        }
                    }
                    else
                    {
                        tr.Log("No calls to AddCard for authorized users since there were no authorized users that were successfully added to CCM account");
                    }
                }
                else
                {
                    /*
                     * No primary, joint, or guarantor applicants in the application
                     */
                    result.Result = false;
                    result.AddError("There are no primary, joint, or guarantor applicants in the application");
                    tr.Log("There are no primary, joint, or guarantor applicants in the application");
                }
            }

            return result;
        }

        /// <summary>
        /// Process for disbursing an application that is an Add On.
        /// </summary>
        /// <param name="primaryApplicant"></param>
        /// <param name="jointGuarantorApplicants"></param>
        /// <returns></returns>
        public BaseResult DisburseAddOn(Applicant primaryApplicant, IEnumerable<Applicant> jointGuarantorApplicants)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.CCMOrigination.DisburseAddOn"))
            {
                // Joint or Guarantor applicants
                if (jointGuarantorApplicants?.Any() == true)
                {
                    tr.Log($"Number of joint or guarantor applicants in ApplicationId {_app.ApplicationId} => {jointGuarantorApplicants.Count()}");

                    foreach (var jointGuarantorApplicant in jointGuarantorApplicants)
                    {
                        /*********************
                         * Call AddPerson/AddOrganization on this joint or guarantor applicant (person or organization).
                         */
                        tr.Log($"Adding ApplicantId {jointGuarantorApplicant.ApplicantId} as a person or organization in CCM");
                        tr.Log($"ApplicantId {jointGuarantorApplicant.ApplicantId} is ApplicantTypeId {jointGuarantorApplicant.ApplicantTypeId}");

                        if (!jointGuarantorApplicant.IsOrganization)
                        {
                            // Add the person applicant to CCM
                            var addPersonResult = _ccm.AddPerson(_soapStrategy, jointGuarantorApplicant);
                            result.AppendResult(addPersonResult);
                            tr.Log($"AddPerson result for ApplicantId {jointGuarantorApplicant.ApplicantId} => {addPersonResult.Result}");
                        }
                        else
                        {
                            // Guarantors cannot be organizations
                            if (jointGuarantorApplicant.ApplicantTypeId != (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Guarantor)
                            {
                                var jointApplicant = jointGuarantorApplicant;

                                // Add the organization applicant to CCM
                                var addOrganizationResult = _ccm.AddOrganization(_soapStrategy, jointApplicant);
                                result.AppendResult(addOrganizationResult);
                                tr.Log($"AddOrganization result for ApplicantId {jointApplicant.ApplicantId} => {addOrganizationResult.Result}");
                            }
                        }
                    }
                }
                else
                {
                    /*
                     * No joint or guarantor applicants in application
                     */
                    tr.Log("There are no joint or guarantor applicants in the application");
                }

                // Authorized users
                if (_app.AuthorizedUsers?.Any() == true)
                {
                    tr.Log($"Number of authorized users in ApplicationId {_app.ApplicationId} => {_app.AuthorizedUsers.Count}");

                    foreach (var authorizedUser in _app.AuthorizedUsers)
                    {
                        /*********************
                         * Call AddPerson passing this authorized user.
                         */
                        tr.Log($"Adding AuthorizedUserId {authorizedUser.AuthorizedUserId} as a person in CCM");

                        // Add the authorized user to CCM
                        var addAuthorizedUserResult = _ccm.AddAuthorizedUser(_soapStrategy, authorizedUser);
                        result.AppendResult(addAuthorizedUserResult);
                        tr.Log($"AddAuthorizedUser result for AuthorizedUserId {authorizedUser.AuthorizedUserId} => {addAuthorizedUserResult.Result}");
                    }
                }
                else
                {
                    /*
                     * No authorized users in application
                     */
                    tr.Log("There are no authorized users in the application");
                }

                /*********************
                 * Update Account to modify the credit limit of primary applicant.
                 */
                tr.Log($"Calling UpdateAccount for ApplicantId {primaryApplicant.ApplicantId}");

                // Make service call to Update Account to update the credit limit of the primary applicant
                var updateAccountResult = _ccm.UpdateAccount(_soapStrategy, primaryApplicant);
                result.AppendResult(updateAccountResult);
                tr.Log($"UpdateAccount result for ApplicantId {primaryApplicant.ApplicantId} => {updateAccountResult.Result}");

                if (!updateAccountResult.Result)
                {
                    _app.StatusId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicationStatus.DisbursementError;
                }

                /*********************
                 * Make inquiry call on each joint or guarantor applicant to determine if that joint or guarantor should be added to
                 * account party relationship, and if that joint needs a credit card.
                 */
                foreach (var jointGuarantorApplicant in jointGuarantorApplicants)
                {
                    string errorMessage = string.Empty;

                    // These are the partyIds that are related to jointGuarantorApplicant
                    IList<RelationshipInfo> relationshipInfos = null;

                    var inquiryResult = _ccm.MakeInquiry(_restStrategy, jointGuarantorApplicant.PersonNumber, out errorMessage, out relationshipInfos);
                    result.AppendResult(inquiryResult);
                    tr.Log($"Inquiry result for ApplicantId {jointGuarantorApplicant.ApplicantId} => {inquiryResult.Result}");
                    tr.Log($"Inquiry errorMessage = {errorMessage}");
                    tr.Log($"Inquiry relationshipInfos.Count = {relationshipInfos?.Count}");

                    if (errorMessage.Equals("Relationship not found", StringComparison.InvariantCulture))
                    {
                        /*********************
                         * Applicant was not found in CCM, so call AddAccountPartyRelationship on this joint or guarantor applicant.
                         * Add joint or guarantor to the credit card account if there is NOT an accountNumber in the relationshipInfos of Inquiry response
                         * that equals Application > Credit Card Number.
                         */
                        tr.Log($"Creating an Account Party Relationship for ApplicantId {jointGuarantorApplicant.ApplicantId} in CCM");

                        var addAccountPartyRelationshipResult = _ccm.AddAccountPartyRelationship(_soapStrategy, jointGuarantorApplicant);
                        result.AppendResult(addAccountPartyRelationshipResult);
                        tr.Log($"AddAccountPartyRelationship result for ApplicantId {jointGuarantorApplicant.ApplicantId} => {addAccountPartyRelationshipResult.Result}");

                        /*********************
                         * Add Card for the non-organization joint applicant that was successfully added to the CCM account.
                         */
                        var isNonOrgJointApplicant =
                            !jointGuarantorApplicant.IsOrganization &&
                            jointGuarantorApplicant.ApplicantTypeId.GetValueOrDefault() == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Joint;

                        if (addAccountPartyRelationshipResult.Result && isNonOrgJointApplicant)
                        {
                            var jointApplicant = jointGuarantorApplicant;

                            tr.Log($"Adding a card for ApplicantId {jointApplicant.ApplicantId} in CCM");

                            var addCardResult = _ccm.AddCard(_soapStrategy, jointApplicant);
                            result.AppendResult(addCardResult);
                            tr.Log($"AddCard result for ApplicantId {jointApplicant.ApplicationId} => {addCardResult.Result}");
                        }
                        else
                        {
                            tr.Log($"No card will be created for ApplicantId {jointGuarantorApplicant.ApplicantId} since it was NOT sucessfully added to account party relationship or is a guarantor applicant");
                        }
                    }
                    else
                    {
                        /*
                         * Person/Organization already exists in CCM
                         */
                        tr.Log($"ApplicantId {jointGuarantorApplicant.ApplicantId} already exists in CCM");
                    }
                }

                /*********************
                 * Make inquiry call on each authorized user to determine if that authorized user needs a credit card.
                 */
                foreach (var authorizedUser in _app.AuthorizedUsers)
                {
                    string errorMessage = string.Empty;

                    // These are the partyIds that are related to authorizedUser
                    IList<RelationshipInfo> relationshipInfos = null;

                    var inquiryResult = _ccm.MakeInquiry(_restStrategy, authorizedUser.PersonNumber, out errorMessage, out relationshipInfos);
                    result.AppendResult(inquiryResult);
                    tr.Log($"Inquiry result for AuthorizedUserId {authorizedUser.AuthorizedUserId} => {inquiryResult.Result}");
                    tr.Log($"Inquiry errorMessage = {errorMessage}");
                    tr.Log($"Inquiry relationshipInfos.Count = {relationshipInfos?.Count}");

                    if (errorMessage.Equals("Relationship not found", StringComparison.InvariantCulture))
                    {
                        /*********************
                         * Add authorized user to the credit card account if there is NOT an accountNumber in the relationshipInfos of Inquiry response
                         * that equals Application > Credit Card Number.
                         */
                        tr.Log($"Adding a card for AuthorizedUserId {authorizedUser.AuthorizedUserId} in CCM");

                        var addCardResult = _ccm.AddCard(_soapStrategy, authorizedUser);
                        result.AppendResult(addCardResult);
                        tr.Log($"AddCard result for AuthorizedUserId = {authorizedUser.AuthorizedUserId} => {addCardResult.Result}");

                    }
                    else
                    {
                        /*
                         * Authorized user already exists in CCM
                         */
                        tr.Log($"AuthorizedUserId {authorizedUser.AuthorizedUserId} already exists in CCM");
                    }
                }
            }

            return result;
        }

        #endregion

    }
}
