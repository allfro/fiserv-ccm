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
    public class AddPersonBehaviorTests
    {
        private AddPerson _person;
        private MessageResponse _messageResponse;
        private Application _app;
        private string _userToken;

        [SetUp]
        public void SetUp()
        {
            _person = GetAddPerson();
            _app = GetApplication();
            _userToken = "aBc123";
        }

        [Test]
        public void AddPerson_GivenAPersonThatExistsInDNA_ButNotInCCM_ShouldBeAbleToAddThatPersonInCCM()
        {
            // ARRANGE
            var applicant = GetApplicant();
            var lmsPerson = new LmsPerson()
            {
                Applicant = applicant
            };

            var address = applicant.Addresses.FirstOrDefault();
            var phone = applicant.Phones.FirstOrDefault();

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
                _app.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                _person.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                applicant.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                _person.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                address.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                _person.Message?.HostValueParentNode
            );

            messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                messageXml,
                phone.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                _person.Message?.HostValueParentNode
            );

            var processMessageNodeRequest = new ProcessMessageNodeRequest()
            {
                CredentialsHeader = credentialsHeader,
                request = GetXmlNode(messageXml)
            };

            var stubServiceRepo = Substitute.For<ISoapRepository>();
            stubServiceRepo.GetProcessMessageNodeRequest(credentialsHeader, messageXml).Returns(processMessageNodeRequest);

            _messageResponse = GetMessageResponseSuccess();

            stubServiceRepo.AddPerson(_person, _app, lmsPerson, address, phone).Returns(_messageResponse);
            var stubLmsRepo = Substitute.For<ILmsRepository>();

            var mockBehavior = new AddPersonBehavior(_app, _userToken, stubServiceRepo, stubLmsRepo);
            mockBehavior.Person = _person;
            mockBehavior.CurrentAddress = address;
            mockBehavior.MainPhone = phone;

            // ACT
            var result = mockBehavior.AddPerson(lmsPerson);

            // ASSERT
            Assert.AreEqual(0, result.Messages.Count(m => m.Type == MessageType.Error));
            Assert.IsTrue(result.Result);
            Assert.AreEqual("Success", mockBehavior.MessageResponse.ResponseCode);
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

        public AddPerson GetAddPerson()
        {
            var addPerson = new AddPerson()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "554120039",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Add",
                        Person = new Person()
                        {
                            PartyNumber = "998976",
                            TaxIdNumber = "199991234",  //199-99-1234
                            PrimaryAddress = new PrimaryAddress()
                            {
                                AddressLine1 = "123 North South Street",
                                City = "Westboro",
                                StateProvince = "IN",
                                PostalCode = "48906",
                                CountryCode = "US",
                                AddressType = "Home"
                            },
                            PrimaryEmail = new PrimaryEmail()
                            {
                                EmailAddress = "hello@helloworld.com"
                            },
                            PrimaryPhone = new PrimaryPhone()
                            {
                                CityAreaCode = "765",
                                LocalPhoneNumber = "6658897",
                                Description = "My home phone",
                                PhoneType = "Home"
                            },
                            LastName = "Bush",
                            FirstName = "Georgia",
                            DateOfBirth = "1945-04-06",
                            MothersMaidenName = "Crunkle"
                        }
                    }
                }
            };

            return addPerson;
        }

        public MessageResponse GetMessageResponseSuccess()
        {
            var messageResponse = new MessageResponse()
            {
                TraceNumber = "554120039",
                ResponseCode = "Success",
                PersonPartyId = "998976"
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
                PersonNumber = "998976",    // This person MUST already exist in DNA, and thus already have a PersonNumber
                TIN = "199991234",  // 199-99-1234
                ApplicantTypeId = (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary,
                IsOrganization = false,
                FirstName = "Georgia",
                LastName = "Bush",
                MothersMaidenName = "Crunkle",
                BirthDate = new DateTime(1945, 4, 6),   // "1945-04-06"
                Email = "hello@helloworld.com",
                Addresses = new List<Address>()
                {
                    new Address()
                    {
                        AddressTypeId = 311,    // Current
                        ClassificationId = 1728,    // Normal
                        Address1 = "123 North South Street",
                        City = "Westboro",
                        StateId = 357,  // IN
                        PostalCode = "48906",
                        CountryId = 638,    // US
                    }
                },
                Phones = new List<Phone>()
                {
                    new Phone()
                    {
                        PhoneTypeId = 318,  // Home
                        PhoneNumber = "765-665-8897",
                        HostValues = new List<HostValue>()
                        {
                            new HostValue("AddPerson.Message.DataUpdate.Person.PrimaryPhone.Description", "My home phone")
                        }
                    }
                },
                CreditCardDesignId = 2000014
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
