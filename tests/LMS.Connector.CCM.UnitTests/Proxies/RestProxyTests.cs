using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Proxies;
using LMS.Core.Rest;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using System;

namespace LMS.Connector.CCM.UnitTests.Proxies
{
    [TestFixture]
    public class RestProxyTests
    {
        [Test]
        public void GetAuthToken_WhenGivenValidCredentials_ShouldGetAnAuthTokenFromSessionsEndpoint()
        {
            // ARRANGE
            var stubApi = Substitute.For<IAPI>();
            var stubLazyApi = new Lazy<IAPI>(() => stubApi);
            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validMPassword",
                Facility = "validFacility"
            };

            var header = new Header()
            {
                ContentType = "application/json"
            };

            var session = new Session()
            {
                UserName = credentials.Username,
                Password = credentials.Password,
                Authentication = "CCM"
            };

            var sessionResponse = new SessionResponse()
            {
                AuthToken = "abc123"
            };

            stubLazyApi.Value.URL = $"{credentials.BaseUrl}/v1/ccmservice/sessions";

            stubLazyApi.Value.Post<Session, SessionResponse>(session).Returns(sessionResponse);

            var mockProxy = new RestProxy(stubLazyApi, credentials);

            // ACT
            var authToken = mockProxy.GetAuthToken(session);

            // ASSERT
            Assert.IsNotEmpty(authToken);
            Assert.AreEqual(sessionResponse.AuthToken, authToken);
        }

        [Test]
        public void GetAuthToken_GivenASession_ReturnsAnAuthToken()
        {
            var stubApi = Substitute.For<IAPI>();
            var stubLazyApi = new Lazy<IAPI>(() => stubApi);
            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validMPassword",
                Facility = "validFacility"
            };

            var session = new Session()
            {
                UserName = "validUsername",
                Password = "validMPassword",
                Authentication = "CCM"
            };

            var fakeSessionResponse = JsonConvert.DeserializeObject<SessionResponse>("{'authToken':'cc6320b637beb3949d522d2c32341fe5'}");

            stubLazyApi.Value.Post<Session, SessionResponse>(session).Returns(fakeSessionResponse);
            var lazyApiValue = stubLazyApi.Value.Post<Session, SessionResponse>(session);
        }

        [Test]
        public void MakeInquiry_GivenAPartyId_ShouldReturnAPartyRelationshipInquiryResponse()
        {
            var stubApi = Substitute.For<IAPI>();
            var stubLazyApi = new Lazy<IAPI>(() => stubApi);
            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validMPassword",
                Facility = "validFacility"
            };

            var session = new Session()
            {
                UserName = "validUsername",
                Password = "validMPassword",
                Authentication = "CCM"
            };

            var fakeSessionResponse = JsonConvert.DeserializeObject<SessionResponse>("{'authToken':'cc6320b637beb3949d522d2c32341fe5'}");

            stubLazyApi.Value.Post<Session, SessionResponse>(session).Returns(fakeSessionResponse);
            var lazyApiValue = stubLazyApi.Value.Post<Session, SessionResponse>(session);

            string _authToken = fakeSessionResponse?.AuthToken;

            string fakeLogString = $"GetAuthToken(Session) _authToken = {_authToken}";

            var proxy = new RestProxy(stubLazyApi, credentials);

            proxy.MakeInquiry(new PartyRelationshipsInquiry());
        }
    }
}
