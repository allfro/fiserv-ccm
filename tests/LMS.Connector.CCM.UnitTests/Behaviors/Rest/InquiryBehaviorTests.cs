using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Behaviors.Rest;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Proxies;
using LMS.Connector.CCM.Repositories;
using LMS.Core.Rest;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LMS.Connector.CCM.UnitTests.Behaviors.Rest
{
    [TestFixture]
    public class InquiryBehaviorTests
    {
        private string _apiVersion = "v1";
        private string _creditCardAccountNumber = "1234567890";
        private Application _app;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {

            _app = GetApplication(_creditCardAccountNumber);
            _userToken = "924EE51F-E2AF-41A5-A1FA-8229EE2AAB88";
        }

        [TestCase(93291, 1234567890)]
        public void Inquiry_WhenAPerson_HasAnAccountNumberInCCM_ThatMatchesTheApplicationCreditCardNumber_Then_ShouldReturnFound
            (int existingPersonNumber, int existingAccountNumber)
        {
            // ARRANGE
            var stubProxy = Substitute.For<IServiceProxy>();
            stubProxy.GetAuthToken(Arg.Any<Session>()).Returns("71a4899f66ee2c2e30883e1c835eb5cf");
            stubProxy.GetAuthorization().Returns("AuthToken 71a4899f66ee2c2e30883e1c835eb5cf");

            var header = new Header()
            {
                ContentType = "application/json",
                Authorization = stubProxy.GetAuthorization()
            };

            var partyRelationshipsInquiry = new PartyRelationshipsInquiry()
            {
                ApiVersion = "v1",
                PartyId = existingPersonNumber
            };

            var request = new Request<PartyRelationshipsInquiry>()
            {
                Body = partyRelationshipsInquiry
            };

            var stubApi = Substitute.For<IAPI>();
            var stubLazyApi = new Lazy<IAPI>(() => stubApi);
            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validMPassword",
                Facility = "validFacility"
            };

            stubLazyApi.Value.URL = $"{credentials.BaseUrl}/{_apiVersion}/parties/{existingPersonNumber}/relationships";

            var relationshipInfos = new List<RelationshipInfo>()
            {
                new RelationshipInfo() { PartyId = "8675309", AccountNumber = "11111111" },
                new RelationshipInfo() { PartyId = existingPersonNumber.ToString(), AccountNumber = existingAccountNumber.ToString() }
            };

            var partyRelationshipInquiryResponse = new PartyRelationshipsInquiryResponse()
            {
                RelationshipInfos = relationshipInfos
            };

            stubLazyApi.Value.Get<PartyRelationshipsInquiryResponse>().Returns(partyRelationshipInquiryResponse);
            var lazyApiValue = stubLazyApi.Value.Get<PartyRelationshipsInquiryResponse>();

            stubProxy.MakeInquiry(partyRelationshipsInquiry).Returns(lazyApiValue);
            var proxyMakeInquiry = stubProxy.MakeInquiry(partyRelationshipsInquiry);

            var stubRepo = Substitute.For<IRestRepository>();
            stubRepo.Proxy = stubProxy;
            stubRepo.Proxy.MakeInquiry(partyRelationshipsInquiry).Returns(proxyMakeInquiry);

            var makeInquiryResult = stubRepo.Proxy.MakeInquiry(partyRelationshipsInquiry);
            var relationships = makeInquiryResult?.RelationshipInfos;
            stubRepo.MakeInquiry(partyRelationshipsInquiry).Returns(relationships);

            var mockBehavior = new InquiryBehavior(_app, _userToken, stubRepo);
            mockBehavior.PartyRelationshipInquiry = partyRelationshipsInquiry;

            // ACT
            var result = mockBehavior.Inquiry(existingAccountNumber.ToString());
            var errorMessage = mockBehavior.ErrorMessage;

            // ASSERT
            Assert.IsNotEmpty(errorMessage);
            Assert.AreEqual("Found", errorMessage);
        }

        [TestCase(93291)]
        public void Inquiry_WhenAPerson_HasAnAccountNumberInCCM_ThatMatchesTheApplicationCreditCardNumber_Then_ShouldReturnRelationshipNotFound
            (int nonExistingPersonNumber)
        {
            // ARRANGE
            var stubProxy = Substitute.For<IServiceProxy>();
            stubProxy.GetAuthToken(Arg.Any<Session>()).Returns("71a4899f66ee2c2e30883e1c835eb5cf");
            stubProxy.GetAuthorization().Returns("AuthToken 71a4899f66ee2c2e30883e1c835eb5cf");

            var header = new Header()
            {
                ContentType = "application/json",
                Authorization = stubProxy.GetAuthorization()
            };

            var partyRelationshipsInquiry = new PartyRelationshipsInquiry()
            {
                ApiVersion = "v1",
                PartyId = nonExistingPersonNumber
            };

            var request = new Request<PartyRelationshipsInquiry>()
            {
                Body = partyRelationshipsInquiry
            };

            var stubApi = Substitute.For<IAPI>();
            var stubLazyApi = new Lazy<IAPI>(() => stubApi);
            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validMPassword",
                Facility = "validFacility"
            };

            stubLazyApi.Value.URL = $"{credentials.BaseUrl}/{_apiVersion}/parties/{nonExistingPersonNumber}/relationships";

            var relationshipInfos = new List<RelationshipInfo>()
            {
                new RelationshipInfo() { PartyId = "8675309", AccountNumber = "11111111" },
                new RelationshipInfo() { PartyId = nonExistingPersonNumber.ToString(), AccountNumber = "22222222" }
            };

            var partyRelationshipInquiryResponse = new PartyRelationshipsInquiryResponse()
            {
                RelationshipInfos = relationshipInfos
            };

            stubLazyApi.Value.Get<PartyRelationshipsInquiryResponse>().Returns(partyRelationshipInquiryResponse);
            var lazyApiValue = stubLazyApi.Value.Get<PartyRelationshipsInquiryResponse>();

            stubProxy.MakeInquiry(partyRelationshipsInquiry).Returns(lazyApiValue);
            var proxyMakeInquiry = stubProxy.MakeInquiry(partyRelationshipsInquiry);

            var stubRepo = Substitute.For<IRestRepository>();
            stubRepo.Proxy = stubProxy;
            stubRepo.Proxy.MakeInquiry(partyRelationshipsInquiry).Returns(proxyMakeInquiry);

            var makeInquiryResult = stubRepo.Proxy.MakeInquiry(partyRelationshipsInquiry);
            var relationships = makeInquiryResult?.RelationshipInfos;
            stubRepo.MakeInquiry(partyRelationshipsInquiry).Returns(relationships);

            var mockInquiryBehavior = new InquiryBehavior(_app, _userToken, stubRepo);
            mockInquiryBehavior.PartyRelationshipInquiry = partyRelationshipsInquiry;

            // ACT
            var result = mockInquiryBehavior.Inquiry(nonExistingPersonNumber.ToString());
            var errorMessage = mockInquiryBehavior.ErrorMessage;

            // ASSERT
            Assert.IsNotEmpty(errorMessage);
            Assert.AreEqual("Relationship not found", errorMessage);
        }

        [TearDown]
        public void TearDown()
        {
            _app = null;
        }

        public Application GetApplication(string creditCardAccountNumber = "")
        {
            return new Application()
            {
                ApplicationId = 1,
                CreditCardNumber = creditCardAccountNumber
            };
        }
    }
}
