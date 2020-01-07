using Akcelerant.Core.Data.DTO.Host;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Behaviors.Soap;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Helpers;
using LMS.Connector.CCM.Repositories;
using LMS.Core.HostValues.Utility.Translator.Xml;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LMS.Connector.CCM.UnitTests.Helpers
{
    [TestFixture]
    public class XmlSerializationExtensionsTests
    {
        [Test]
        public void SerializeToXmlString_GivenADto_ShouldSerializeDtoInProperXmlFormat()
        {
            // ARRANGE
            var app = GetApplication();
            var applicant = GetApplicant();
            var userToken = "aBc123";

            var dto = GetAddAccount(app, applicant, userToken);

            // ACT
            var xml = dto.Message.SerializeToXmlString();

            xml = HostValueTranslator.UpdateRequestWithHostValues(
                xml,
                app.HostValues,
                dto.Message?.HostValueParentNode
            );

            xml = HostValueTranslator.UpdateRequestWithHostValues(
                xml,
                applicant.HostValues,
                dto.Message?.HostValueParentNode
            );

            // ASSERT
            Assert.IsNotEmpty(xml);
        }

        [Test]
        public void SerializeToXmlString_WhenApplicationHostValues_HasNoUserFields_SerializedXmlOutput_ShouldNotHaveUserFieldNodes()
        {
            // ARRANGE
            var app = GetApplication();
            app.HostValues.RemoveAll(hv => hv.Field1.Contains("AddAccount.Message.DataUpdate.Account.UserFields.UserField"));

            var applicant = GetApplicant();
            var userToken = "aBc123";

            var dto = GetAddAccount(app, applicant, userToken);

            var xml = dto.Message.SerializeToXmlString();

            xml = HostValueTranslator.UpdateRequestWithHostValues(
                xml,
                app.HostValues,
                dto.Message?.HostValueParentNode
            );

            xml = HostValueTranslator.UpdateRequestWithHostValues(
                xml,
                applicant.HostValues,
                dto.Message?.HostValueParentNode
            );

            var xDoc = XDocument.Parse(xml);

            // ACT
            var hasUserFieldNodes = xDoc.Descendants().Where(n => n.Name.LocalName == "UserField").Any();

            // ASSERT
            Assert.IsFalse(hasUserFieldNodes);
        }

        public AddAccount GetAddAccount(Application app, Applicant applicant, string userToken)
        {
            var stubServiceRepo = Substitute.For<ISoapRepository>();
            var stubLmsRepo = Substitute.For<ILmsRepository>();
            var fakeBehavior = new AddAccountBehavior(app, userToken, stubServiceRepo, stubLmsRepo);

            var account = new AddAccount()
            {
                Message = new Dto.Soap.Message()
                {
                    DataUpdate = new DataUpdate()
                    {
                        TraceNumber = "668254",
                        ProcessingCode = "ExternalUpdateRequest",
                        Source = "LoanOrigination",
                        UpdateAction = "Add",
                        UpdateTarget = "Account",
                        Account = new Account()
                        {
                            LoanOfficerName = "Admin Admin",
                            AccountOpenDate = "2019-07-25",
                            ProductName = app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.ProductName")) ? string.Empty : null,
                            RateClass = app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.RateClass")) ? string.Empty : null,
                            PromotionalRateClass = app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.PromotionalRateClass")) ? string.Empty : null,
                            PromotionalRateClassStartDate = app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.PromotionalRateClassStartDate")) ? string.Empty : null,
                            PromotionalRateClassEndDate = app.HostValues.Any(h => h.Field1.Equals("AddAccount.Message.DataUpdate.Account.PromotionalRateClassEndDate")) ? string.Empty : null,
                            CreditLimit = 10000.00m,
                            TaxOwnerPartyId = "79396",
                            TaxOwnerPartyType = "Person",
                            UserFields = fakeBehavior.GetUserFields(applicant)
                        }
                    }
                }
            };

            return account;
        }

        public UpdateAccount GetUpdateAccount(Application app, Applicant applicant)
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
                            },
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

        public Application GetApplication()
        {
            var app = new Application()
            {
                ApplicationId = 123,
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddAccount.Message.DataUpdate.Account.ProductName", "Visa Platinum Credit"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.RateClass", "A Credit Tier"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "CompetitorPayoffAmt", "Competitor Payoff Amt"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Value","CompetitorPayoffAmt", "LEVEL1-2000-9999"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Name", "CompetitorPayoffAmt2", "Competitor Payoff Amt 2"),
                    new HostValue("AddAccount.Message.DataUpdate.Account.UserFields.UserField.Value", "CompetitorPayoffAmt2", "LEVEL2-2000-9999"),
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
                ApplicantId = 1,
                ApplicationId = 123,
                HostValues = new List<HostValue>()
                {
                    new HostValue("AddAccount.Message.DataUpdate.Account.TaxOwnerPartyType", "Organization")
                }
            };

            return applicant;
        }
    }
}
