using Akcelerant.Core.Data.DTO.Host;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Behaviors.Soap;
using LMS.Connector.CCM.CCMSoapWebService;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Helpers;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;
using LMS.Core.HostValues.Utility.Translator.Xml;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace LMS.Connector.CCM.UnitTests.Behaviors.Soap
{
    [TestFixture]
    public class AddCardBehaviorTests
    {
        private AddCard _card;
        private MessageResponse _messageResponse;
        private Application _app;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {
            _card = GetAddCard();
            _app = GetApplication();
            _userToken = "aBc123";
        }

        [Test]
        public void AddCard_GivenAPersonThatHasAnAccountInCCM_ShouldBeAbleToCreateAPlasticCard()
        {
            // ARRANGE
            var applicant = GetApplicant();
            var lmsPerson = new LmsPerson()
            {
                Applicant = applicant
            };

            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validPassword",
                Facility = "validFacility"
            };

            var credentialsHeader = GetCredentialsHeader(credentials);

            var messageXml = _card.Message?.SerializeToXmlString();

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                applicant.HostValues.Where(hv => hv.Field1.StartsWith("AddCard.")).ToList(),
                _card.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseSuccess();

            stubServiceRepo.AddCard(_card, _app, lmsPerson).Returns(_messageResponse);

            var stubLmsRepo = Substitute.For<ILmsRepository>();

            var mockBehavior = new AddCardBehavior(_app, _userToken, stubServiceRepo, stubLmsRepo);
            mockBehavior.Card = _card;

            // ACT
            var result = mockBehavior.AddCard(lmsPerson);
            var cardNumber = mockBehavior.MessageResponse?.CardNumber;

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("Success", mockBehavior.MessageResponse.ResponseCode);
            Assert.AreEqual("4398790000000001", cardNumber);
        }

        [Test]
        public void AddCard_GivenAPersonThatDoesNotHaveAnAccountInCCM_ShouldNotBeAbleToCreateAPlasticCard()
        {
            // ARRANGE
            var applicant = GetApplicant();
            var lmsPerson = new LmsPerson()
            {
                Applicant = applicant
            };

            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validPassword",
                Facility = "validFacility"
            };

            var credentialsHeader = GetCredentialsHeader(credentials);

            var messageXml = _card.Message?.SerializeToXmlString();

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
               messageXml,
               applicant.HostValues.Where(hv => hv.Field1.StartsWith("AddCard.")).ToList(),
               _card.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseFail();

            stubServiceRepo.AddCard(_card, _app, lmsPerson).Returns(_messageResponse);

            var stubLmsRepo = Substitute.For<ILmsRepository>();

            var mockBehavior = new AddCardBehavior(_app, _userToken, stubServiceRepo, stubLmsRepo);
            mockBehavior.Card = _card;

            // ACT
            var result = mockBehavior.AddCard(lmsPerson);
            var cardNumber = mockBehavior.MessageResponse?.CardNumber;

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("SystemMalfunction", mockBehavior.MessageResponse.ResponseCode);
        }

        [TearDown]
        public void TearDown()
        {
            _card = null;
            _app = null;
        }

        public CredentialsHeader GetCredentialsHeader(Credentials credentials)
        {
            var credentialsHeader = new CredentialsHeader()
            {
                Username = credentials.Username,
                Password = credentials.Password,
                Facility = credentials.Facility,
                CultureId = "en"
            };

            return credentialsHeader;
        }

        public AddCard GetAddCard()
        {
            var addCard = new AddCard()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "554120039",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Add",
                        Card = new Card()
                        {
                            AccountNumber = "9000000008000",
                            CardholderPartyNumber = "5597",
                            CardholderPartyType = "Person"
                        }
                    }
                }
            };

            return addCard;
        }

        public MessageResponse GetMessageResponseSuccess()
        {
            var messageResponse = new MessageResponse()
            {
                TraceNumber = "554120039",
                ResponseCode = "Success",
                CardNumber = "4398790000000001"
            };

            return messageResponse;
        }

        public MessageResponse GetMessageResponseFail()
        {
            var messageResponse = new MessageResponse()
            {
                TraceNumber = "554120039",
                ResponseCode = "SystemMalfunction"
            };

            return messageResponse;
        }

        public Application GetApplication()
        {
            var app = new Application()
            {
                ApplicationId = 554120039,
                DisbursedDate = new DateTime(2007, 2, 10),  // "2007-02-10"
                FinalLoanAmount = 20000.00m,
                FinalDecisionUserId = 19    // Fake UserId for "Steve Higgs"
            };

            return app;
        }

        public Applicant GetApplicant()
        {
            var applicant = new Applicant()
            {
                PersonNumber = "5597",
                ApplicantTypeId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary,
                IsOrganization = false,
                CreditCardDesignId = 2000014,
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddCard.Message.DataUpdate.Card.EmbossingLine1", "Andrew Simmons")
                }
            };

            return applicant;
        }

        public Applicant GetOrganization()
        {
            var applicant = new Applicant()
            {
                PersonNumber = "5597",
                ApplicantTypeId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary,
                IsOrganization = true,
                OrganizationName = "Simmons Textiles",
                CreditCardDesignId = 2000014,
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddCard.Message.DataUpdate.Card.EmbossingLine1", "Andrew Simmons")
                }
            };

            return applicant;
        }

        public AuthorizedUser GetAuthorizedUser()
        {
            var authorizedUser = new AuthorizedUser()
            {
                PersonNumber = "5599",
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddCard.Message.DataUpdate.Card.EmbossingLine2", "Simmons Textiles")
                }
            };

            return authorizedUser;
        }

        public XmlNode GetXmlNode(string request)
        {
            var xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(request);

            var newNode = xmlDoc.DocumentElement;

            return newNode;
        }
    }
}
