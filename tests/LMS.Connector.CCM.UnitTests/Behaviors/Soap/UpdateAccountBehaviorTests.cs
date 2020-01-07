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
    public class UpdateAccountBehaviorTests
    {
        private UpdateAccount _account;
        private MessageResponse _messageResponse;
        private Application _app;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {
            _account = GetUpdateAccount();
            _app = GetApplication();
            _userToken = "aBc123";
        }

        [Test]
        public void UpdateAccount_WhenApplicationIsAnAddon_AndPrimaryPersonExistsInCCM_ShouldSuccessfullyUpdateCreditLimitInCCM()
        {
            // ARRANGE
            _app.IsAddon = true;
            var primaryApplicant = GetApplicant();

            var credentials = new Credentials()
            {
                BaseUrl = "https://some.bank.or.cu/api",
                Username = "validUsername",
                Password = "validPassword",
                Facility = "validFacility"
            };

            var credentialsHeader = GetCredentialsHeader(credentials);

            var messageXml = _account.Message?.SerializeToXmlString();

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                _app.HostValues.Where(hv => hv.Field1.StartsWith("UpdateAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                primaryApplicant.HostValues.Where(hv => hv.Field1.StartsWith("UpdateAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseSuccess();

            stubServiceRepo.UpdateAccount(_account, _app, primaryApplicant).Returns(_messageResponse);

            var mockBehavior = new UpdateAccountBehavior(_app, _userToken, stubServiceRepo);
            mockBehavior.Account = _account;

            // ACT
            var result = mockBehavior.UpdateAccount(primaryApplicant);

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("Success", mockBehavior.MessageResponse.ResponseCode);
        }

        [TearDown]
        public void TearDown()
        {
            _account = null;
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

        public UpdateAccount GetUpdateAccount()
        {
            var account = new UpdateAccount()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "1266523457",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Modify",
                        Account = new Account()
                        {
                            AccountNumber = "9000000000007",
                            LoanOfficerName = "Steve Higgs",
                            AccountOpenDate = "2019-05-20",
                            ProductName = "Test Gold",
                            RateClass = "Promo Test Gold",
                            CreditLimit = 40000.00m,
                            TaxOwnerPartyId = "5597",
                            TaxOwnerPartyType = "Person",
                            UserFields = new List<UserField>()
                            {
                                new UserField() { Name = "LoanOfficerType", Value = "HomeOffice" },
                                new UserField() { Name = "OriginationMethod", Value = "IASystems" }
                            }
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

        public MessageResponse GetMessageResponseSuccess()
        {
            var messageResponse = new MessageResponse()
            {
                TraceNumber = "1266523457997",
                ResponseCode = "Success"
            };

            return messageResponse;
        }

        public Application GetApplication()
        {
            var app = new Application()
            {
                ApplicationId = 1266523457,
                DisbursedDate = new DateTime(2007, 2, 10),  // "2007-02-10"
                IsAddon = true,
                FinalLoanAmount = 40000.00m,
                FinalDecisionUserId = 19    // Fake UserId for "Steve Higgs"
            };

            return app;
        }

        public Applicant GetApplicant()
        {
            var applicant = new Applicant()
            {
                PersonNumber = "5597",
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
