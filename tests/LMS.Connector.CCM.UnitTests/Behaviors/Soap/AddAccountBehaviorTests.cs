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
    public class AddAccountBehaviorTests
    {
        private AddAccount _account;
        private MessageResponse _messageResponse;
        private Application _app;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {
            _account = GetAddAccountDto();
            _app = GetApplication();
            _userToken = "aBc123";
        }

        [Test]
        public void AddAccount_GivenAPersonThatExistsInCCM_ShouldBeAbleToCreateACreditCardAccountInCCM()
        {
            // ARRANGE
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
                _app.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                primaryApplicant.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
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

            stubServiceRepo.AddAccount(_account, _app, primaryApplicant).Returns(_messageResponse);
            var stubLmsRepo = Substitute.For<ILmsRepository>();
            stubLmsRepo.GetUserFullNameById(_app.FinalDecisionUserId.GetValueOrDefault()).Returns("Steve Higgs");

            var mockBehavior = new AddAccountBehavior(_app, _userToken, stubServiceRepo, stubLmsRepo);
            mockBehavior.Account = _account;

            // ACT
            var result = mockBehavior.AddAccount(primaryApplicant);
            var accountNumber = mockBehavior.MessageResponse.AccountNumber;

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("Success", mockBehavior.MessageResponse.ResponseCode);
            Assert.AreEqual("9000000000007", accountNumber);
        }

        [Test]
        public void AddAccount_GivenAPersonThatDoesNotExistInCCM_ShouldNotBeAbleToCreateACreditCardAccountInCCM()
        {
            // ARRANGE
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
                _app.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                primaryApplicant.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseFail();

            stubServiceRepo.AddAccount(_account, _app, primaryApplicant).Returns(_messageResponse);
            var stubLmsRepo = Substitute.For<ILmsRepository>();
            stubLmsRepo.GetUserFullNameById(_app.FinalDecisionUserId.GetValueOrDefault()).Returns("Steve Higgs");

            var mockBehavior = new AddAccountBehavior(_app, _userToken, stubServiceRepo, stubLmsRepo);
            mockBehavior.Account = _account;

            // ACT
            var result = mockBehavior.AddAccount(primaryApplicant);
            var accountNumber = mockBehavior.MessageResponse.AccountNumber;

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("SystemMalfunction", mockBehavior.MessageResponse.ResponseCode);
        }

        [Test]
        public void AddAccount_GivenAnOrganizationThatExistsInCCM_ShouldBeAbleToCreateACreditCardAccountInCCM()
        {
            // ARRANGE
            var primaryApplicantOrg = GetOrganization();

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
                _app.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                primaryApplicantOrg.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
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

            stubServiceRepo.AddAccount(_account, _app, primaryApplicantOrg).Returns(_messageResponse);

            var stubLmsRepo = Substitute.For<ILmsRepository>();
            stubLmsRepo.GetUserFullNameById(_app.FinalDecisionUserId.GetValueOrDefault()).Returns("Steve Higgs");

            var mockBehavior = new AddAccountBehavior(_app, _userToken, stubServiceRepo, stubLmsRepo);
            mockBehavior.Account = _account;

            // ACT
            var result = mockBehavior.AddAccount(primaryApplicantOrg);
            var accountNumber = mockBehavior.MessageResponse.AccountNumber;

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("Success", mockBehavior.MessageResponse.ResponseCode);
            Assert.AreEqual("9000000000007", accountNumber);
        }

        [Test]
        public void AddAccount_GivenAnOrganizationThatDoesNotExistInCCM_ShouldNotBeAbleToCreateACreditCardAccountInCCM()
        {
            // ARRANGE
            var primaryApplicantOrg = GetOrganization();

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
                _app.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                primaryApplicantOrg.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseFail();

            stubServiceRepo.AddAccount(_account, _app, primaryApplicantOrg).Returns(_messageResponse);
            var stubLmsRepo = Substitute.For<ILmsRepository>();
            stubLmsRepo.GetUserFullNameById(_app.FinalDecisionUserId.GetValueOrDefault()).Returns("Steve Higgs");

            var mockBehavior = new AddAccountBehavior(_app, _userToken, stubServiceRepo, stubLmsRepo);
            mockBehavior.Account = _account;

            // ACT
            var result = mockBehavior.AddAccount(primaryApplicantOrg);
            var accountNumber = mockBehavior.MessageResponse.AccountNumber;

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("SystemMalfunction", mockBehavior.MessageResponse.ResponseCode);
        }

        [Test]
        public void GetUserFields_WhenUserFieldHostValuesAreAdded_ShouldOutputACollectionOfUserFields()
        {
            // ARRANGE
            List<UserField> userFieldList;
            var applicant = GetApplicant();
            var numberOfUserFieldNameHVs = _app.HostValues.Count(hv => hv.Field1.Equals("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Name"));
            var numberOfUserFieldValueHVs = _app.HostValues.Count(hv => hv.Field1.Equals("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Value"));
            var numberOfActualUserFieldHV = numberOfUserFieldNameHVs + numberOfUserFieldValueHVs;

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            var stubLmsRepo = Substitute.For<ILmsRepository>();
            var mockBehavior = new AddAccountBehavior(_app, _userToken, stubServiceRepo, stubLmsRepo);

            // ACT
            userFieldList = mockBehavior.GetUserFields(applicant);

            // ASSERT
            Assert.IsNotNull(userFieldList);
            Assert.AreEqual(numberOfUserFieldNameHVs, userFieldList.Count(u => !string.IsNullOrWhiteSpace(u.Name)));
            Assert.AreEqual(numberOfUserFieldValueHVs, userFieldList.Count(u => !string.IsNullOrWhiteSpace(u.Value)));
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

        public AddAccount GetAddAccountDto()
        {
            var account = new AddAccount()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "33333333",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Add",
                        Account = new Account()
                        {
                            LoanOfficerName = "Steve Higgs",
                            AccountOpenDate = "2019-05-20",
                            //ProductName = "Test Gold",
                            RateClass = "Promo Test Gold",
                            CreditLimit = 20000.00m,
                            TaxOwnerPartyId = "5597",
                            TaxOwnerPartyType = "Person",
                            UserFields = new List<UserField>()
                            {
                                new UserField() { Name = "LoanOfficerType", Value = "HomeOffice" },
                                new UserField() { Name = "OriginationMethod", Value = "IASystems" }
                            }
                        }
                    }
                }
            };

            return account;
        }

        public AddAccount GetAddAccountOrganizationDto()
        {
            var account = new AddAccount()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "33333333",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Add",
                        Account = new Account()
                        {
                            LoanOfficerName = "Steve Higgs",
                            AccountOpenDate = "2019-05-20",
                            //ProductName = "Test Gold",
                            RateClass = "Promo Test Gold",
                            CreditLimit = 20000.00m,
                            TaxOwnerPartyId = "5597",
                            TaxOwnerPartyType = "Person",
                            UserFields = new List<UserField>()
                            {
                                new UserField() { Name = "LoanOfficerType", Value = "HomeOffice" },
                                new UserField() { Name = "OriginationMethod", Value = "IASystems" }
                            }
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
                TraceNumber = "33333333",
                ResponseCode = "Success",
                AccountNumber = "9000000000007"
            };

            return messageResponse;
        }

        public MessageResponse GetMessageResponseFail()
        {
            var messageResponse = new MessageResponse()
            {
                TraceNumber = "33333333",
                ResponseCode = "SystemMalfunction"
            };

            return messageResponse;
        }

        public Application GetApplication()
        {
            var app = new Application()
            {
                ApplicationId = 33333333,
                DisbursedDate = new DateTime(2019, 5, 20),  // "2019-05-20"
                FinalLoanAmount = 20000.00m,
                FinalDecisionUserId = 19,   // Fake UserId for "Steve Higgs"
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddAccount.Message.DataUpdate.Account.ProductName", "Test Gold"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "CompetitorPayoffAmt", "Competitor Payoff Amt"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "CompetitorPayoffAmt", "LEVEL1-2000-9999"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "RateCreditScore", "Rate Credit Score"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "RateCreditScore", "00P02 Score"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "TemenosApplicationNbr", "Temenos Application Number"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "TemenosApplicationNbr", "33333333"),
                }
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
