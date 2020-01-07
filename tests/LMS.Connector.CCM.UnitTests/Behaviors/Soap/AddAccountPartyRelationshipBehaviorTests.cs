using Akcelerant.Core.Data.DTO.Host;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using Akcelerant.Lending.Lookups.Constants;
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
    public class AddAccountPartyRelationshipBehaviorTests
    {
        private AddAccountPartyRelationship _accountPartyRelationship;
        private MessageResponse _messageResponse;
        private Application _app;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {
            _accountPartyRelationship = GetAddAccountPartyRelationship();
            _app = GetApplication();
            _userToken = "aBc123";
        }

        [Test]
        public void AddAccountPartyRelationship_GivenAJointPersonThatExistsInCCM_ShouldBeAbleToAddThatJointToTheAccountInCCM()
        {
            // ARRANGE
            var jointApplicant = GetApplicant();

            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validPassword",
                Facility = "validFacility"
            };

            var credentialsHeader = GetCredentialsHeader(credentials);

            var messageXml = _accountPartyRelationship.Message?.SerializeToXmlString();

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                _app.HostValues.Where(hv => hv.Field1.StartsWith("AddAccountPartyRelationship.")).ToList(),
                _accountPartyRelationship.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                jointApplicant.HostValues.Where(hv => hv.Field1.StartsWith("AddAccountPartyRelationship.")).ToList(),
                _accountPartyRelationship.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseSuccess();

            stubServiceRepo.AddAccountPartyRelationship(_accountPartyRelationship, _app, jointApplicant).Returns(_messageResponse);

            var mockBehavior = new AddAccountPartyRelationshipBehavior(_app, _userToken, stubServiceRepo);
            mockBehavior.AccountPartyRelationship = _accountPartyRelationship;

            // ACT
            var result = mockBehavior.AddAccountPartyRelationship(jointApplicant);

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("Success", mockBehavior.MessageResponse.ResponseCode);
        }

        [Test]
        [Ignore("Flawed")]
        public void AddAccountPartyRelationship_GivenAJointPersonThatExistsInCCM_ShouldNotBeAbleToAddThatJointToTheAccountInCCM()
        {
            // ARRANGE
            var jointApplicant = GetApplicant();

            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validPassword",
                Facility = "validFacility"
            };

            var credentialsHeader = GetCredentialsHeader(credentials);

            var messageXml = _accountPartyRelationship.Message?.SerializeToXmlString();

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
               messageXml,
               _app.HostValues.Where(hv => hv.Field1.StartsWith("AddAccountPartyRelationship.")).ToList(),
               _accountPartyRelationship.Message?.HostValueParentNode
           );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                jointApplicant.HostValues.Where(hv => hv.Field1.StartsWith("AddAccountPartyRelationship.")).ToList(),
                _accountPartyRelationship.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseFail();

            stubServiceRepo.AddAccountPartyRelationship(_accountPartyRelationship, _app, jointApplicant).Returns(_messageResponse);

            var mockBehavior = new AddAccountPartyRelationshipBehavior(_app, _userToken, stubServiceRepo);
            mockBehavior.AccountPartyRelationship = _accountPartyRelationship;

            // ACT
            var result = mockBehavior.AddAccountPartyRelationship(jointApplicant);

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("SystemMalfunction", mockBehavior.MessageResponse.ResponseCode);
        }

        [TearDown]
        public void TearDown()
        {
            _accountPartyRelationship = null;
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

        public AddAccountPartyRelationship GetAddAccountPartyRelationship()
        {
            var accountPartyRelationship = new AddAccountPartyRelationship()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "1266523457",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Indeterminate",
                        AccountPartyRelationships = new List<AccountPartyRelationship>()
                        {
                            new AccountPartyRelationship()
                            {
                                UpdateAction = "Add",
                                AccountNumber = "9000000008000",
                                PartyNumber = "5597",
                                PartyType = "Person",
                                AccountRelationshipType = "CoMaker",
                                OriginationDate = "2007-02-10"
                            }
                        }
                    }
                }
            };

            return accountPartyRelationship;
        }

        public MessageResponse GetMessageResponseSuccess()
        {
            var messageResponse = new MessageResponse()
            {
                TraceNumber = "1266523457997",
                ResponseCode = "Success"
            };

            return messageResponse;
        }

        public MessageResponse GetMessageResponseFail()
        {
            var messageResponse = new MessageResponse()
            {
                TraceNumber = "1266523457997",
                ResponseCode = "SystemMalfunction"
            };

            return messageResponse;
        }

        public Application GetApplication()
        {
            var app = new Application()
            {
                ApplicationId = 1266523457,
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
                ApplicantTypeId = (int)Values.ApplicantType.Joint,
                IsOrganization = false,
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddAccountPartyRelationship.Message.DataUpdate.AccountPartyRelationships.AccountPartyRelationship.SendStatement", "True")
                }
            };

            return applicant;
        }

        public Applicant GetOrganization()
        {
            var applicant = new Applicant()
            {
                PersonNumber = "5597",
                ApplicantTypeId = (int)Values.ApplicantType.Joint,
                IsOrganization = true,
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddAccountPartyRelationship.Message.DataUpdate.AccountPartyRelationships.AccountPartyRelationship.SendStatement", "True")
                }
            };

            return applicant;
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
