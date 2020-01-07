using System.Collections.Generic;
using Akcelerant.Core.Client.Tracing;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Proxies;

namespace LMS.Connector.CCM.Repositories
{
    public class RestRepository : IRestRepository
    {
        private const string SOLUTION_CODE = "CCM_ORIGINATION";
        private const string PARAMETER_CODE = "CCM_REST_SERVICE_URL";

        private string _userToken;
        private ILmsRepository _lmsRepository;
        private Credentials _credentials;
        private IServiceProxy _proxy;

        private static string _authToken = string.Empty;

        public RestRepository()
        {

        }

        public RestRepository(string userToken)
        {
            _lmsRepository = new LmsRepository(userToken);
            _credentials = GetServiceCredentials();
            _proxy = new RestProxy(_credentials);
        }

        public RestRepository(string userToken, Credentials credentials)
        {
            _lmsRepository = new LmsRepository(userToken);
            _credentials = credentials;
            _proxy = new RestProxy(credentials);
        }

        public RestRepository(string userToken, Credentials credentials, IServiceProxy proxy)
        {
            _lmsRepository = new LmsRepository(userToken);
            _credentials = credentials;
            _proxy = proxy;
        }

        public RestRepository(string userToken, Credentials credentials, IServiceProxy proxy, ILmsRepository lmsRepository)
        {
            _lmsRepository = lmsRepository;
            _credentials = credentials;
            _proxy = proxy;
        }

        #region "Interface implementations"

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

        public ILmsRepository LmsRepository
        {
            get
            {
                return _lmsRepository;
            }
            set
            {
                _lmsRepository = value;
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

        public IServiceProxy Proxy
        {
            get
            {
                return _proxy;
            }
            set
            {
                _proxy = value;
            }
        }

        public IList<RelationshipInfo> MakeInquiry(PartyRelationshipsInquiry inquiry)
        {
            IList<RelationshipInfo> relationshipInfos = null;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.RestRepository.MakeInquiry"))
            {
                tr.Log("Calling IServiceProxy.MakeInquiry");
                var inquiryResponse = _proxy.MakeInquiry(inquiry);
                tr.LogObject(inquiryResponse);

                relationshipInfos = inquiryResponse?.RelationshipInfos;
                tr.LogObject(relationshipInfos);
            }

            return relationshipInfos;
        }

        public bool TestConnection(Session session)
        {
            string authToken = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.RestRepository.TestConnection"))
            {
                tr.LogObject(session);

                tr.Log("Calling IRestRepository.GetAuthToken");
                authToken = GetAuthToken(session);
                tr.Log($"authToken.Length = {authToken.Length}");
            }

            return (authToken.Length > 0) ? true : false;
        }

        public string GetAuthToken(Session session)
        {
            string authToken = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.RestRepository.GetAuthToken"))
            {
                tr.LogObject(session);

                tr.Log("Calling IServiceProxy.GetAuthToken");
                authToken = _proxy.GetAuthToken(session);
                tr.Log($"authToken = {authToken}");

                _authToken = authToken;
            }

            return authToken;
        }

        #endregion

        #region "Utility methods"

        public Credentials GetServiceCredentials()
        {
            Credentials credentials = null;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.RestRepository.GetServiceCredentials"))
            {
                tr.Log($"SOLUTION_CODE = {SOLUTION_CODE}, PARAMETER_CODE = {PARAMETER_CODE}");
                tr.Log("Calling ILmsRepository.GetServiceCredentials");
                credentials = _lmsRepository.GetServiceCredentials(SOLUTION_CODE, PARAMETER_CODE);
                tr.LogObject(credentials);
            }

            return credentials;
        }

        #endregion
    }
}
