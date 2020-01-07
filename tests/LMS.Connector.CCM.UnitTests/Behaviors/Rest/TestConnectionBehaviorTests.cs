using LMS.Connector.CCM.Behaviors.Rest;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;
using NSubstitute;
using NUnit.Framework;

namespace LMS.Connector.CCM.UnitTests.Behaviors.Rest
{
    [TestFixture]
    public class TestConnectionBehaviorTests
    {
        private Credentials _credentials;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {
            _credentials = GetCredentials();
            _userToken = "924EE51F-E2AF-41A5-A1FA-8229EE2AAB88";
        }

        [Test]
        public void TestConnection_WhenCalled_ShouldCallRepositoryTestConnection()
        {
            // ARRANGE
            var mockRepo = Substitute.For<IRestRepository>();
            mockRepo.Credentials = _credentials;
            var stubBehavior = new TestConnectionBehavior(_userToken, mockRepo);


            // ACT
            var dontCare = stubBehavior.TestConnection("fooServiceUrl", "barUserName", "fooPassword", "barFacility");

            // ASSERT
            mockRepo.Received().TestConnection(Arg.Any<Session>());
        }

        [Test]
        public void TestConnection_WhenGivenValidCredentials_ShouldSuccessfullyEstablishAConnection()
        {
            // ARRANGE
            var stubRepo = Substitute.For<IRestRepository>();
            stubRepo.Credentials = Arg.Any<Credentials>();

            var header = new Header() { ContentType = "application/json" };
            var session = new Session()
            {
                UserName = "validUsername",
                Password = "validPassword",
                Authentication = "CCM"
            };

            stubRepo.TestConnection(session).Returns(true);

            var mockBehavior = new TestConnectionBehavior(_userToken, stubRepo);
            mockBehavior.Session = session;

            // ACT
            var result = mockBehavior.TestConnection("fooServiceUrl", "validUsername", "validPassword", "barFacility");

            // ASSERT
            Assert.AreEqual(true, mockBehavior.ConnectionEstablished);
        }

        [TearDown]
        public void TearDown()
        {
            _credentials = null;
        }

        public Credentials GetCredentials()
        {
            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api/",
                Username = "CCMUsername",
                Password = "CCMPassword",
                Facility = "Facility"
            };

            return credentials;
        }
    }
}
