using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DTO.Host;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Core.Lookups;
using Akcelerant.Lending.Client.Services;
using Akcelerant.Lending.Data.DTO.Applications;
using Akcelerant.Lending.Data.DTO.Service;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;
using LMS.Connector.CCM.Validation;

namespace LMS.Connector.CCM.Service
{
    /// <summary>
    /// Services to interface with CCM Origination
    /// </summary>
    public class Api : IDisburseApplication, IApi, IDisposable
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

        public Api()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public Api(string userToken)
        {
            _userToken = userToken;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public Api(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        #region "IDisburseApplication implementation"

        public BalanceConsolidationResponse BalanceConsolidation(BalanceConsolidationRequest request)
        {
            var result = new BalanceConsolidationResponse();

            return result;
        }

        /// <summary>
        /// Disburses the Application to CCM.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public BaseResult DisburseApplication(string userToken, ref Application app)
        {
            var result = new BaseResult();
            _app = app;
            _userToken = userToken;

            using (var tr = new Tracer("LMS.Connector.CCM.Service.Api.DisburseApplication"))
            {
                var restStrategy = new RestStrategy(app, userToken);

                var soapRepository = new SoapRepository(userToken);
                var soapStrategy = new SoapStrategy(app, userToken, soapRepository);
                soapStrategy.Repository = soapRepository;

                var ccm = new CCMOrigination(app, userToken, restStrategy, soapStrategy);

                try
                {
                    result = ccm.DisburseApplication();
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Service.Api.DisburseApplication");
                    result.AddError(ex.Message);
                }

                if (result.Result)
                {
                    result.AddMessage(MessageType.Success, "Loan product disbursed successfully.");
                    result.AddMessage(MessageType.Success, "Application has been disbursed.");

                    app.IsLoanDisbursed = true;
                    app.StatusId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicationStatus.Disbursed;
                    app.DisbursementUserId = app.LoggedInUser.UserId;

                    tr.Log($"ApplicationId {app.ApplicationId} DisbursementUserId => {app.LoggedInUser.UserId}");
                }

                tr.Log($"ApplicationId {app.ApplicationId} IsLoanDisbursed => {app.IsLoanDisbursed}");
                tr.Log($"ApplicationId {app.ApplicationId} StatusId => {app.StatusId}");
                tr.Log($"LMS.Connector.CCM.Service.Api.DisburseApplication result => {result.Result}");
            }

            return result;
        }

        /// <summary>
        /// Checks for required fields prior to sending calls to the CCM service.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public BaseResult DisbursementValidation(string userToken, ref Application app)
        {
            var result = new BaseResult();
            _app = app;
            _userToken = userToken;

            using (var tr = new Tracer("LMS.Connector.CCM.Service.Api.DisbursementValidation"))
            {
                var ccm = new CCMValidation(app, userToken);

                result.AppendResult(ccm.DisbursementValidation());

                tr.Log($"LMS.Connector.CCM.Service.Api.DisbursementValidation result => {result.Result}");
            }

            return result;
        }

        public BaseResult TestConnection(IList<HostParameter> connectorParams)
        {
            var result = TestConnections(connectorParams);

            return result;
        }

        #endregion

        #region "IApi implementation"

        public BaseResult TestConnections(IList<HostParameter> connectorParams)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Service.Api.TestConnections"))
            {
                try
                {
                    var userName = connectorParams.SingleOrDefault(p => p.Name == "CCM_USERNAME")?.Value;
                    var password = connectorParams.SingleOrDefault(p => p.Name == "CCM_PASSWORD")?.Value;
                    var facility = connectorParams.SingleOrDefault(p => p.Name == "CCM_FACILITY")?.Value;
                    var soapServiceUrl = connectorParams.SingleOrDefault(p => p.Name == "CCM_SOAP_SERVICE_URL")?.Value;
                    var restServiceUrl = connectorParams.SingleOrDefault(p => p.Name == "CCM_REST_SERVICE_URL")?.Value;
                    _userToken = connectorParams.SingleOrDefault(p => p.Name == "USERTOKEN")?.Value;

                    // Set up fake primary applicant for testing connection
                    var app = GetTestApplication();

                    var soapCredentials = new Credentials()
                    {
                        BaseUrl = soapServiceUrl,
                        Username = userName,
                        Password = password,
                        Facility = facility
                    };
                    tr.LogObject(soapCredentials);

                    var restCredentials = new Credentials()
                    {
                        BaseUrl = restServiceUrl,
                        Username = userName,
                        Password = password,
                        Facility = facility
                    };
                    tr.LogObject(restCredentials);

                    var soapStrategy = new SoapStrategy(app, _userToken, soapCredentials);

                    var restStrategy = new RestStrategy(app, _userToken, restCredentials);

                    var ccm = new CCMOrigination(app, _userToken, restStrategy, soapStrategy);

                    result = ccm.TestConnections(userName, password, facility, soapServiceUrl, restServiceUrl);
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Service.Api.TestConnections");
                    result.AddError(ex.Message);
                    throw;
                }

                tr.Log($"LMS.Connector.CCM.Service.Api.TestConnections result => {result.Result}");
            }

            return result;
        }

        #endregion

        #region "IDisposable implementation"

        private bool _disposed = false;

        public void Dispose()
        {
            /*** Do not change this code.  Put cleanup code in Dispose(bool) method! ***/
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //TODO: dispose managed state (managed objects).
                }

                //TODO: free unmanaged resources (unmanaged objects) and override the destructor.
                //TODO: set large fields to null.
            }

            this._disposed = true;
        }

        ~Api()
        {
            Dispose(false);
        }

        #endregion

        private Application GetTestApplication()
        {
            var app = new Application()
            {
                ApplicationId = 123456,
                CreditCardNumber = "99999999999999",
                FinalLoanAmount = 1m,
                Applicants = new List<Applicant>()
                {
                    new Applicant()
                    {
                        ApplicantTypeId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary,
                        PersonNumber = "99999999",
                        TIN = "999999999",
                        Addresses = new List<Address>()
                        {
                            new Address()
                            {
                                AddressTypeId = (new LookupComponent(_userToken)).GetLookupIdByTypeAndCode(LookupTypes.AddressType, LookupCodes.AddressType.Current),
                                Address1 = "123 Oak Court"
                            }
                        }
                    }
                }
            };

            return app;
        }
    }
}