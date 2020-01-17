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
    public class UpdateAccountBehaviorTests
    {
        private UpdateAccount _account;
        private MessageResponse _messageResponse;
        private Application _applicationAddOn;
        private Applicant _primaryApplicant;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void UpdateAccount_WhenApplicationIsAnAddon_AndPrimaryPersonExistsInCCM_ShouldSuccessfullyUpdateCreditLimitInCCM()
        {
            // ARRANGE
            _applicationAddOn = GetApplication_AddOn();
            _primaryApplicant = GetApplicant();
            _userToken = "aBc123";
            _account = GetUpdateAccountDto(_applicationAddOn, _primaryApplicant, _userToken);
            _applicationAddOn.IsAddon = true;

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
                _applicationAddOn.HostValues.Where(hv => hv.Field1.StartsWith("UpdateAccount.")).ToList(),
                _account.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                _primaryApplicant.HostValues.Where(hv => hv.Field1.StartsWith("UpdateAccount.")).ToList(),
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

            stubServiceRepo.UpdateAccount(_account, _applicationAddOn, _primaryApplicant).Returns(_messageResponse);

            var mockBehavior = new UpdateAccountBehavior(_applicationAddOn, _userToken, stubServiceRepo);
            mockBehavior.Account = _account;

            // ACT
            var result = mockBehavior.UpdateAccount(_primaryApplicant);

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("Success", mockBehavior.MessageResponse.ResponseCode);
        }

        [TearDown]
        public void TearDown()
        {
            _account = null;
            _applicationAddOn = null;
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

        public UpdateAccount GetUpdateAccountDto(Application app, Applicant applicant, string userToken)
        {
            var stubServiceRepo = Substitute.For<ISoapRepository>();
            var stubLmsRepo = Substitute.For<ILmsRepository>();
            var fakeBehavior = new UpdateAccountBehavior(app, userToken, stubServiceRepo, stubLmsRepo);

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
                            UserFields = fakeBehavior.GetUserFields(applicant)
                        },
                        ModifiedFields = fakeBehavior.GetModifiedFields(applicant)
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

        public Application GetApplication_AddOn()
        {
            var app = new Application()
            {
                ApplicationId = 1266523457,
                CreditCardNumber = "163276318",
                DisbursedDate = new DateTime(2007, 2, 10),  // "2007-02-10"
                IsAddon = true,
                FinalLoanAmount = 40000.00m,
                FinalDecisionUserId = 19,   // Fake UserId for "Steve Higgs"
                HostValues = new List<HostValue>()
                {
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.ProductName", "Visa Platinum Credit"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.RateClass", "A Credit Tier"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "CompetitorPayoffAmt", "Competitor Payoff Amt"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "CompetitorPayoffAmt", "LEVEL1-2000-9999"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "RateCreditScore", "Rate Credit Score"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "RateCreditScore", "00P02 Score"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "TemenosApplicationNbr", "Temenos Application Number"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "TemenosApplicationNbr", "33333333"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "LoanOfficerType", "LoanOfficerType"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "LoanOfficerType", "HomeOffice"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "OriginationMethod", "OriginationMethod"),
                    new HostValue("UpdateAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "OriginationMethod", "IASystems"),
                    new HostValue("UpdateAccount.Message.DataUpdate.ModifiedFields.AccountField", "RateClass")
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
