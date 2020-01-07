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
    public class TestConnectionBehaviorTests
    {
        private UpdatePerson _person;
        private MessageResponse _messageResponse;
        private Application _app;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {

            _person = GetUpdatePerson();
            _app = GetApplication();
            _userToken = "aBc123";
        }

        [Test]
        public void TestConnection_WhenCalled_ShouldCallRepositoryUpdatePerson()
        {
            // ARRANGE
            var messageResponse = new MessageResponse()
            {
                ErrorMessage = "CardNotFound"
            };

            var stubRepo = Substitute.For<ISoapRepository>();
            stubRepo.UpdatePerson(Arg.Any<UpdatePerson>(), Arg.Any<Application>()).Returns(messageResponse);

            var mockBehavior = new TestConnectionBehavior(_app, "userToken123", stubRepo);

            // ACT
            var dontCare = mockBehavior.TestConnection("fooServiceUrl", "barUserName", "fooPassword", "barFacility");

            // ASSERT
            mockBehavior.SoapRepository.Received().UpdatePerson(Arg.Any<UpdatePerson>(), Arg.Any<Application>());
        }

        [Test]
        public void TestConnection_WhenGivenValidCredentials_ShouldEstablishConnectivityWithCCM()
        {
            // ARRANGE
            _app.IsAddon = true;

            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validPassword",
                Facility = "validFacility"
            };

            var credentialsHeader = GetCredentialsHeader(credentials);

            var messageXml = _person.Message?.SerializeToXmlString();

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                _app.HostValues.Where(hv => hv.Field1.StartsWith("UpdatePerson.")).ToList(),
                _person.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseSystemMalfunction();

            stubServiceRepo.UpdatePerson(_person, _app).Returns(_messageResponse);

            var mockBehavior = new TestConnectionBehavior(_app, _userToken, stubServiceRepo);
            mockBehavior.Person = _person;

            // ACT
            var result = mockBehavior.TestConnection("fooServiceUrl", "barUserName", "fooPassword", "barFacility");

            // ASSERT
            Assert.IsTrue(result.Result);
            Assert.AreEqual("SystemMalfunction", mockBehavior.MessageResponse.ResponseCode);
            Assert.AreEqual("Modify Party request failed. Party 99999999 not found.", mockBehavior.MessageResponse.ErrorMessage);
            Assert.IsTrue(mockBehavior.ConnectionEstablished);
        }

        [TearDown]
        public void TearDown()
        {
            _person = null;
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

        public UpdatePerson GetUpdatePerson()
        {
            var person = new UpdatePerson()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "123456",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Modify",
                        Person = new Person()
                        {
                            PartyNumber = "99999999",
                            TaxIdNumber = "999999999",
                            PrimaryAddress = new PrimaryAddress()
                            {
                                AddressLine1 = "123 Oak Court"
                            }
                        },
                        ModifiedFields = new ModifiedFields()
                        {
                            AddressField = "AddressLine1"
                        }
                    }
                }
            };

            return person;
        }

        public UpdateAccount GetUpdateAccount()
        {
            var account = new UpdateAccount()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "1",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Modify",
                        Account = new Account()
                        {
                            AccountNumber = "99999999999999",
                            CreditLimit = 1m
                        },
                        ModifiedFields = new ModifiedFields()
                        {
                            AccountField = "CreditLimit"
                        }
                    }
                }
            };

            return account;
        }

        public MessageResponse GetMessageResponseSystemMalfunction()
        {
            var messageResponse = new MessageResponse()
            {
                TraceNumber = "123456",
                ResponseCode = "SystemMalfunction",
                ErrorMessage = "Modify Party request failed. Party 99999999 not found."
            };

            return messageResponse;
        }

        public Application GetApplication()
        {
            var app = new Application()
            {
                ApplicationId = 123456,
                DisbursedDate = new DateTime(2007, 2, 10),  // "2007-02-10"
                IsAddon = true,
                FinalLoanAmount = 40000.00m,
                FinalDecisionUserId = 19,   // Fake UserId for "Steve Higgs"
                Applicants = new List<Applicant>()
            };

            app.Applicants.Add(GetApplicant());

            return app;
        }

        public Applicant GetApplicant()
        {
            var applicant = new Applicant()
            {
                PersonNumber = "99999999",
                ApplicantTypeId = (int)Values.ApplicantType.Primary,
                IsOrganization = false
            };

            return applicant;
        }

        public Applicant GetOrganization()
        {
            var applicant = new Applicant()
            {
                PersonNumber = "5597",
                ApplicantTypeId = (int)Values.ApplicantType.Primary,
                IsOrganization = true
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
