using System;
using System.Collections.Generic;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Helpers;
using LMS.Connector.CCM.Models;
using LMS.Core.Rest;

namespace LMS.Connector.CCM.Proxies
{
    public class RestProxy : IServiceProxy
    {
        private const string AUTHTOKEN_PREFIX = "AuthToken ";
        private const string API_VERSION = "v1";
        private const string AUTHENTICATION = "CCM";

        private Lazy<IAPI> _api;
        private Credentials _credentials;
        private string _baseUrl;

        private string _authToken = string.Empty;

        /// <summary>
        /// This constructor will be called when class object will be intialized.
        /// This will call RestProxy(Lazy&lt;IAPI&gt;).
        /// </summary>
        public RestProxy(Credentials credentials)
        {
            _api = new Lazy<IAPI>(() => new CCMApiExtender());
            _credentials = credentials;
            _baseUrl = _credentials.BaseUrl;
        }

        /// <summary>
        /// This will be called from default constructor.
        /// Used in unit test project to inject mocked objects of all dependencies to fake outside calls.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="credentials"></param>
        public RestProxy(Lazy<IAPI> api, Credentials credentials)
        {
            _api = api;
            _credentials = credentials;
            _baseUrl = credentials.BaseUrl;
        }

        #region "CCM REST API URL EndPoints"

        /// <summary>
        /// Base URL, API, and Version which to build paths to endpoints. Includes "/" at the end.
        /// </summary>
        /// <example>
        /// BaseUrlApi + "/parties/relationships" => https://this.com/ccm/api/v1/parties/relationships
        /// </example>
        public string BaseUrlApi
        {
            get { return $"{_baseUrl}api/{API_VERSION}"; }
        }

        /// <summary>
        /// Sessions
        /// </summary>
        public string SessionsUrl
        {
            get { return $"{BaseUrlApi}/sessions"; }
        }

        #endregion

        #region "Interface implementations"

        public Lazy<IAPI> Api
        {
            get
            {
                return _api;
            }
            set
            {
                _api = value;
            }
        }

        public Credentials Credentials
        {
            get
            {
                return _credentials;
            }
            set
            {
                _credentials = value;
            }
        }

        public string BaseUrl
        {
            get
            {
                return _baseUrl;
            }
            set
            {
                _baseUrl = value;
            }
        }

        public string GetAuthorization()
        {
            var authorization = AUTHTOKEN_PREFIX + GetAuthToken();

            return authorization;
        }

        public string GetAuthToken(Session session)
        {
            var authToken = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Proxies.RestProxy.GetAuthToken"))
            {
                tr.LogObject(session);

                _api.Value.URL = SessionsUrl;
                tr.Log($"URL: {_api.Value.URL}");

                try
                {
                    var sessionsResponse = _api.Value.Post<Session, SessionResponse>(session);
                    tr.LogObject(sessionsResponse);

                    authToken = sessionsResponse?.AuthToken;
                    tr.Log($"GetAuthToken(Session) authToken = {authToken}");

                    _authToken = authToken;
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Proxies.RestProxy.GetAuthToken");
                    throw;
                }

                return authToken;
            }
        }

        public PartyRelationshipsInquiryResponse MakeInquiry(PartyRelationshipsInquiry inquiry)
        {
            using (var tr = new Tracer("LMS.Connector.CCM.Proxies.RestProxy.MakeInquiry"))
            {
                var authToken = GetAuthorization();
                tr.Log($"MakeInquiry(PartyRelationshipsInquiry) authToken = {authToken}");

                _api.Value.Headers = new Dictionary<string, string>();
                _api.Value.Headers.Add("Authorization", authToken);
                _api.Value.URL = $"{BaseUrlApi}/parties/{inquiry.PartyId}/relationships";
                tr.Log($"URL: {_api.Value.URL}");

                int headerIndex = 0;
                tr.Log("api.Value.Headers:");
                foreach (var header in _api.Value.Headers)
                {
                    tr.Log($"Header[{headerIndex}] Key = {header.Key}, Value = {header.Value}");
                    headerIndex++;
                }

                PartyRelationshipsInquiryResponse inquiryResponse = null;

                try
                {
                    inquiryResponse = _api.Value.Get<PartyRelationshipsInquiryResponse>();
                    tr.LogObject(inquiryResponse);
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Proxies.RestProxy.MakeInquiry");
                    throw;
                }

                return inquiryResponse;
            }
        }

        #endregion

        #region "Helper methods"

        public void ResetApi()
        {
            _api.Value.Headers.Clear();
            _api.Value.Parameters.Clear();
            _api.Value.URL = string.Empty;
            _api.Value.ReturnType = API.DataType.Json;
        }

        public string GetAuthToken()
        {
            var session = new Session()
            {
                UserName = _credentials.Username,
                Password = _credentials.Password,
                Authentication = AUTHENTICATION
            };

            var authToken = GetAuthToken(session);

            return authToken;
        }

        #endregion
    }
}
