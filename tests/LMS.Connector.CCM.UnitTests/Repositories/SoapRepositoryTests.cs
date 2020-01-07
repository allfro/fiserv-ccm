using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using NSubstitute;
using Akcelerant.Core.Data.DTO.Host;
using Akcelerant.Lending.Data.DTO.Applications;
using Akcelerant.Lending.Lookups.Constants;
using LMS.Connector.CCM.CCMSoapWebService;
using LMS.Connector.CCM.Dto;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Helpers;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;
using LMS.Core.HostValues.Utility.Translator.Xml;

namespace LMS.Connector.CCM.UnitTests.Repositories
{
    [TestFixture]
    public class SoapRepositoryTests
    {
        [TestCase(668254, "79396")]
        public void AddAccount_GivenValidCredentialsAndDto_ShouldBeAbleToAddTheAccountSuccessfully(int applicationId, string personNumber)
        {
            // ARRANGE
            var app = GetApplication(applicationId);
            var applicant = GetApplicant();
            var addAccount = GetAddAccount(personNumber);
            var credentials = GetCredentials();
            string messageXml = addAccount.Message?.SerializeToXmlString();

            var stubSoapClient = Substitute.For<CcmWebServiceSoap>();
            //var stubSoapClient = new CcmWebServiceSoapClient("CcmWebServiceSoap", "https://dna-ccmapp-ttd.uccu.com/ccm-Test-ws/ccmwebservice/ccmwebservice.asmx");

            var repo = new SoapRepository("ABC123", credentials, stubSoapClient);

            var credentialsHeader = new CredentialsHeader()
            {
                Username = credentials.Username,
                Password = credentials.Password,
                Facility = credentials.Facility,
                CultureId = "en"
            };

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(messageXml, app.HostValues, addAccount.Message?.HostValueParentNode);
            messageXml = HostValueTranslator.UpdateRequestWithHostValues(messageXml, applicant.HostValues, addAccount.Message?.HostValueParentNode);

            var processMessageNodeRequest = new ProcessMessageNodeRequest();
            processMessageNodeRequest.CredentialsHeader = credentialsHeader;
            processMessageNodeRequest.request = GetXmlNode(messageXml);
            repo.ProcessMessageNodeRequest = processMessageNodeRequest;

            var messageResponse = new MessageResponse()
            {
                TraceNumber = applicationId.ToString(),
                ResponseCode = "Success",
                AccountNumber = "2001158945704"
            };
            var messageResponseXml = messageResponse.SerializeToXmlString();

            var processMessageNodeResponse = new ProcessMessageNodeResponse()
            {
                ProcessMessageNodeResult = GetXmlNode(messageResponseXml)
            };

            repo.SoapClient.ProcessMessageNode(processMessageNodeRequest).Returns(processMessageNodeResponse);

            // ACT
            var messageNodeResponse = repo.ProcessMessage(messageXml);

            // ASSERT
            Assert.IsNotNull(repo.MessageResponse);
            Assert.AreEqual("Success", repo.MessageResponse.ResponseCode);
        }

        [TestCase(668254, "79396")]
        [Ignore("Not ready")]
        public void AddAccount_GivenValidCredentialsAndDto_ShouldAddTheAccountSuccessfully(int applicationId, string personNumber)
        {
            // ARRANGE
            var app = GetApplication(applicationId);
            var applicant = GetApplicant();
            var addAccount = GetAddAccount(personNumber);
            var credentials = GetCredentials();
            string messageXml = addAccount.Message?.SerializeToXmlString();

            var stubSoapClient = Substitute.For<CcmWebServiceSoap>();
            //var stubSoapClient = new CcmWebServiceSoapClient("CcmWebServiceSoap", "https://dna-ccmapp-ttd.uccu.com/ccm-Test-ws/ccmwebservice/ccmwebservice.asmx");

            var repo = new SoapRepository("ABC123", credentials, stubSoapClient);

            var credentialsHeader = new CredentialsHeader()
            {
                Username = credentials.Username,
                Password = credentials.Password,
                Facility = credentials.Facility,
                CultureId = "en"
            };

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(messageXml, app.HostValues, addAccount.Message?.HostValueParentNode);
            messageXml = HostValueTranslator.UpdateRequestWithHostValues(messageXml, applicant.HostValues, addAccount.Message?.HostValueParentNode);

            var processMessageNodeRequest = new ProcessMessageNodeRequest();
            processMessageNodeRequest.CredentialsHeader = credentialsHeader;
            processMessageNodeRequest.request = GetXmlNode(messageXml);
            repo.ProcessMessageNodeRequest = processMessageNodeRequest;

            var messageResponse = new MessageResponse()
            {
                TraceNumber = applicationId.ToString(),
                ResponseCode = "Success",
                AccountNumber = "2001158945704"
            };
            var messageResponseXml = messageResponse.SerializeToXmlString();

            var processMessageNodeResponse = new ProcessMessageNodeResponse()
            {
                ProcessMessageNodeResult = GetXmlNode(messageResponseXml)
            };

            repo.SoapClient.ProcessMessageNode(processMessageNodeRequest).Returns(processMessageNodeResponse);

            // ACT
            var messageNodeResponse = repo.ProcessMessage(messageXml);

            // ASSERT
            Assert.IsNotNull(repo.MessageResponse);
            Assert.AreEqual("Success", repo.MessageResponse.ResponseCode);
        }

        public AddAccount GetAddAccount(string partyId)
        {
            var account = new AddAccount()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = partyId,
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Add",
                        UpdateTarget = "Account",
                        Account = new Account()
                        {
                            LoanOfficerName = "Admin Admin",
                            AccountOpenDate = "2019-07-29",
                            ProductName = "",
                            CreditLimit = 10000.00m,
                            TaxOwnerPartyId = "79396",
                            TaxOwnerPartyType = "Person"
                        }
                    }
                }
            };

            return account;
        }

        public Credentials GetCredentials()
        {
            var credentials = new Credentials()
            {
                Username = "CCMTEMENOS",
                Password = "1Testing!@",
                Facility = "Main",
                BaseUrl = "https://dna-ccmapp-ttd.uccu.com/ccm-Test-ws/ccmwebservice/ccmwebservice.asmx"
            };

            return credentials;
        }

        public Applicant GetApplicant()
        {
            var applicant = new Applicant()
            {
                PersonNumber = "79396",
                ApplicantTypeId = (int)Values.ApplicantType.Primary,
                IsOrganization = false
            };

            return applicant;
        }

        public Application GetApplication(int applicationId)
        {
            var app = new Application()
            {
                ApplicationId = 668254,
                DisbursedDate = new DateTime(2019, 7, 29),  // "2019-07-29"
                FinalLoanAmount = 10000.00m,
                FinalDecisionUserId = 1,   // Fake UserId for "Admin Admin"
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddAccount.Message.DataUpdate.Account.ProductName", "Visa Platinum Credit"),
                }
            };

            return app;
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
