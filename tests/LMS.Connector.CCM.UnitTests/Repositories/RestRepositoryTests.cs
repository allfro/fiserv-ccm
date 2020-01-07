using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using LMS.Connector.CCM.Dto;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Proxies;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.UnitTests.Repositories
{
    [TestFixture]
    public class RestRepositoryTests
    {
        private string _userToken;

        public IServiceProxy GetServiceProxy()
        {
            return Substitute.For<IServiceProxy>();
        }

        [SetUp]
        public void SetUp()
        {
            _userToken = "924EE51F-E2AF-41A5-A1FA-8229EE2AAB88";
        }

        [Test]
        public void TestConnection_WhenCalled_ShouldCallProxyGetAuthToken()
        {
            // Arrange
            var stubProxy = GetServiceProxy();
            var credentials = new Credentials();
            var repo = new RestRepository(_userToken, credentials, stubProxy);
            var header = new Header();
            var session = new Session();

            // Act
            var dontCare = repo.TestConnection(session);

            // Assert
            stubProxy.Received().GetAuthToken(Arg.Any<Session>());
        }

        [TestCase(111)]
        public void MakeInquiry_WhenAPersonDoesNotExistInCore_ShouldReturnPartyNotFound(int personNumber)
        {
            // Arrange
            var inquiry = new PartyRelationshipsInquiry();
            inquiry.PartyId = personNumber;

            var mockRepo = Substitute.For<IRestRepository>();

            var stubProxy = GetServiceProxy();

            var relationshipInfos = new List<RelationshipInfo>()
            {
                new RelationshipInfo() { PartyId = "222"},
                new RelationshipInfo() { PartyId = "333"}
            };

            var partyRelationshipResponse = new PartyRelationshipsInquiryResponse()
            {
                RelationshipInfos = relationshipInfos
            };

            stubProxy.MakeInquiry(inquiry).Returns(partyRelationshipResponse);

            var serviceResponse = stubProxy.MakeInquiry(inquiry);
            var relationships = serviceResponse?.RelationshipInfos;

            // Act
            mockRepo.MakeInquiry(inquiry).Returns(relationships);
            var relationshipInfosList = mockRepo.MakeInquiry(inquiry);

            string result = string.Empty;

            foreach (var relationshipInfo in relationshipInfosList)
            {
                if (relationshipInfo.PartyId.Equals(inquiry.PartyId.ToString(), StringComparison.InvariantCulture))
                {
                    result = "Found";
                    break;
                }
                else
                {
                    result = "Party not found";
                }
            }

            // Assert
            Assert.AreEqual("Party not found", result);
        }

        [TestCase(111)]
        public void MakeInquiry_WhenAPersonExistsInCore_ShouldReturnFound(int personNumber)
        {
            // Arrange
            var inquiry = new PartyRelationshipsInquiry();
            inquiry.PartyId = personNumber;

            var mockRepo = Substitute.For<IRestRepository>();

            var stubProxy = GetServiceProxy();

            var relationshipInfos = new List<RelationshipInfo>()
            {
                new RelationshipInfo() { PartyId = "111"},
                new RelationshipInfo() { PartyId = "333"}
            };

            var partyRelationshipResponse = new PartyRelationshipsInquiryResponse()
            {
                RelationshipInfos = relationshipInfos
            };

            stubProxy.MakeInquiry(inquiry).Returns(partyRelationshipResponse);

            var serviceResponse = stubProxy.MakeInquiry(inquiry);
            var relationships = serviceResponse?.RelationshipInfos;

            // Act
            mockRepo.MakeInquiry(inquiry).Returns(relationships);
            var relationshipInfosList = mockRepo.MakeInquiry(inquiry);

            string result = string.Empty;

            foreach (var relationshipInfo in relationshipInfosList)
            {
                if (relationshipInfo.PartyId.Equals(inquiry.PartyId.ToString(), StringComparison.InvariantCulture))
                {
                    result = "Found";
                    break;
                }
                else
                {
                    result = "Party not found";
                }
            }

            // Assert
            Assert.AreEqual("Found", result);
        }

        [Test]
        public void MakeInquiry_WhenCalled_ShouldCallProxyMakeInquiry()
        {
            // Arrange
            var mockProxy = GetServiceProxy();
            var credentials = new Credentials();
            var repo = new RestRepository(_userToken, credentials, mockProxy);
            var inquiry = new PartyRelationshipsInquiry();

            // Act
            var dontCare = repo.MakeInquiry(inquiry);

            // Assert
            mockProxy.Received().MakeInquiry(Arg.Any<PartyRelationshipsInquiry>());
        }
    }
}
