using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class Account : DtoBase
    {
        /// <summary>
        /// The number of the CCM account to modify.
        /// </summary>
        /// <remarks>
        /// This tag is required and the account number must exist in the CCM database in order for the message to process successfully.
        /// Note, the account number itself cannot be modified.
        /// </remarks>
        /// <example>12345689</example>
        [XmlElement(Order = 1)]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Name of loan officer to associate with account opening.
        /// </summary>
        /// <remarks>
        /// Any value accepted.
        /// If empty or missing CCM will default to user account running process.
        /// System will default to user account running process if existing information is deleted.
        /// </remarks>
        /// <example>John Smith</example>
        [XmlElement(Order = 2)]
        public string LoanOfficerName { get; set; }

        /// <summary>
        /// Date account opened in yyyy-MM-dd format.
        /// </summary>
        /// <remarks>
        /// Typically current date for AddAccount.
        /// System will default to current date if existing information is deleted.
        /// If empty or missing CCM will default to current date.
        /// When you do not want to include a date for an optional date field, you cannot send the tag at all.
        /// If tag does not have a value, do not send tag at all; do not leave it empty.
        /// </remarks>
        /// <example>2007-05-15</example>
        [XmlElement(Order = 3)]
        public string AccountOpenDate { get; set; }

        /// <summary>
        /// Must be valid product name in CCM database. And the product must be configured to allow new account openings.
        /// </summary>
        /// <example>Visa Gold Plus</example>
        [XmlElement(Order = 4)]
        public string ProductName { get; set; }

        /// <summary>
        /// Indicates lockout status of account.
        /// </summary>
        /// <remarks>
        /// This field is required if the tag is specified by an AccountField tag.
        /// The possible values are as follows:
        /// Active – indicates the account is not locked out and is able to conduct normal transactions.
        /// PaymentsOnly – indicates account is locked out and only Payment transactions should be allowed to post to account.
        /// Complete – indicates account is locked out and no transactions should be allowed to post to account.
        /// </remarks>
        /// <example>PaymentsOnly</example>
        [XmlElement(Order = 5)]
        public string LockoutStatus { get; set; }

        /// <summary>
        /// Reason associated with LockoutStatus.
        /// </summary>
        /// <remarks>
        /// Possible values are the following:
        /// None – indicates account is not locked out or that no reason has been designated for lockout.
        /// DelinquentLoan – indicates account is locked out due to delinquency.
        /// DelinquentLoanCat1 – indicates account is locked out because the account's delinquency has reached the number of days specified in the institution's LoanDelinquentCat1 parameter.
        /// DelinquentLoanCat2 – indicates account is locked out because the account's delinquency has reached the number of days specified in the institution's LoanDelinquentCat2 parameter.
        /// System will default to None if existing information is deleted.
        /// </remarks>
        /// <example>DelinquentLoan</example>
        [XmlElement(Order = 6)]
        public string LockoutReason { get; set; }

        /// <summary>
        /// Name of rate class for account.
        /// </summary>
        /// <remarks>
        /// If empty, CCM will assign default rate class for new accounts configured for the Product.
        /// If a value is sent, it must be a valid rate class for this product in the CCM database.
        /// (Ex. Introductory Rate)
        /// System will default to product level default if existing information is deleted.
        /// </remarks>
        /// <example>Visa Classic Standard Rate</example>
        [XmlElement(Order = 7)]
        public string RateClass { get; set; }

        /// <summary>
        /// Name of promotional rate class for account.
        /// </summary>
        /// <remarks>
        /// This should only be sent to override the promotional rate class that may be configured for the product.
        /// If a value is sent, it must be a valid rate class for this product in the CCM database.
        /// </remarks>
        /// <example>Soldiers and Sailors Rate Class</example>
        [XmlElement(Order = 8)]
        public string PromotionalRateClass { get; set; }

        /// <summary>
        /// Date to begin using promotional rate in yyyy-MM-dd format.
        /// </summary>
        /// <remarks>
        /// If a value is sent, it must be greater than or equal to the account open date.
        /// When you do not want to include a date for an optional date field, you cannot send the tag at all.
        /// If tag does not have a value, do not send tag at all; do not leave it empty.
        /// </remarks>
        /// <example>2007-05-20</example>
        [XmlElement(Order = 9)]
        public string PromotionalRateClassStartDate { get; set; }

        /// <summary>
        /// Date to stop using promotional rate in yyyy-MM-dd format.
        /// </summary>
        /// <remarks>
        /// If a value is sent, it must be greater than the PromotionalRateClassStartDate.
        /// When you do not want to include a date for an optional date field, you cannot send the tag at all.
        /// If tag does not have a value, do not send tag at all; do not leave it empty.
        /// </remarks>
        /// <example>2007-05-20</example>
        [XmlElement(Order = 10)]
        public string PromotionalRateClassEndDate { get; set; }

        /// <summary>
        /// Credit limit for account.
        /// </summary>
        /// <remarks>
        /// This field is required if the tag is specified by an AccountField tag.
        /// </remarks>
        /// <example>5000.00</example>
        [XmlElement(Order = 11)]
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// The party id of the tax owner on the account.
        /// </summary>
        /// <remarks>
        /// This tag is used in conjunction with the TaxOwnerPartyType to identify the party.
        /// This party must exist in the CCM database prior to receiving AddAccount message.
        /// This party must exist in the CCM database prior to receiving UpdateAccount message.
        /// This field is required if the tag is specified by an AccountField tag
        /// TCCUS users: This corresponds to the Pers.PersNbr or Org.OrgNbr from Core of the party.
        /// </remarks>
        /// <example>1234</example>
        [XmlElement(Order = 12)]
        public string TaxOwnerPartyId { get; set; }

        /// <summary>
        /// The party type of the tax owner on the account.
        /// </summary>
        /// <remarks>
        /// Possible values are Person or Organization.
        /// This tag is used in conjunction with the TaxOwnerPartyId to identify the party.
        /// This party must exist in the CCM database prior to receiving AddAccount message.
        /// This party must exist in the CCM database prior to receiving UpdateAccount message.
        /// This field is required if the tag is specified by an AccountField tag.
        /// </remarks>
        /// <example>Organization</example>
        [XmlElement(Order = 13)]
        public string TaxOwnerPartyType { get; set; }

        [XmlArray(ElementName = "UserFields", Order = 14)]
        [XmlArrayItem(Type = typeof(UserField), ElementName = "UserField")]
        public List<UserField> UserFields { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "Account"; }
        }
    }
}
