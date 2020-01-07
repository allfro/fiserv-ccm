using System;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class Card : DtoBase
    {
        /// <summary>
        /// CCM account number to associate with new card.
        /// </summary>
        /// <remarks>
        /// This account number must exist in the CCM database prior to sending the AddCard message.
        /// This account must have an account status of Active.
        /// </remarks>
        /// <example>12345689</example>
        [XmlElement(Order = 1)]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Card number associated with card record to modify.
        /// </summary>
        /// <remarks>
        /// The CardNumber, CardExpirationDate, and SequenceNumber will be used to identify unique
        /// card records in the scenario where multiple card records exist with the same card
        /// number and expiration date.
        /// </remarks>
        /// <example>4444555566667777</example>
        [XmlElement(Order = 2)]
        public string CardNumber { get; set; }

        /// <summary>
        /// Indicates sequence number (also known as plastic digit) or card record.
        /// </summary>
        /// <remarks>
        /// This tag is used in conjunction with CardNumber and CardExpirationDate to identify
        /// unique card records in cases where multiple card records exist with the same card
        /// number and expiration date.
        /// </remarks>
        /// <example>2</example>
        [XmlElement(Order = 3)]
        public string SequenceNumber { get; set; }

        /// <summary>
        /// Indicates expiration date for card record, in yyyy-MM-dd format.
        /// </summary>
        /// <remarks>
        /// This tag is used in conjunction with CardNumber and SequenceNumber to identify unique
        /// card records in cases where multiple card records exist with the same card number
        /// and expiration date.
        /// </remarks>
        /// <example>2009-12-31</example>
        [XmlElement(Order = 4)]
        public string CardExpirationDate { get; set; }

        /// <summary>
        /// Embossing line for new card.
        /// </summary>
        /// <remarks>
        /// The embossing line is limited to 19 characters.
        /// </remarks>
        /// <example>John M. Smith</example>
        [XmlElement(Order = 5)]
        public string EmbossingLine1 { get; set; }

        /// <summary>
        /// Second embossing line for new card.
        /// </summary>
        /// <remarks>
        /// The second embossing line is optional, and is also limited to 19 characters.
        /// </remarks>
        /// <example>Smith Company Name</example>
        [XmlElement(Order = 6)]
        public string EmbossingLine2 { get; set; }

        /// <summary>
        /// The party id of the cardholder.
        /// </summary>
        /// <remarks>
        /// This tag is used in conjunction with the CardholderPartyType to identify the party.
        /// This party must exist in the CCM database prior to receiving AddCard message.
        /// TCCUS users: This corresponds to the Pers.PersNbr or Org.OrgNbr from Core of the party.
        /// </remarks>
        /// <example>1234</example>
        [XmlElement(Order = 7)]
        public string CardholderPartyNumber { get; set; }

        /// <summary>
        /// The party type of the cardholder.
        /// </summary>
        /// <remarks>
        /// Possible values are Person or Organization.
        /// This tag is used in conjunction with the CardholderPartyNumber to identify the party.
        /// This party must exist in the CCM database prior to receiving AddCard message.
        /// </remarks>
        /// <example>Person</example>
        [XmlElement(Order = 8)]
        public string CardholderPartyType { get; set; }

        /// <summary>
        /// Offset of the PIN for the new card.
        /// </summary>
        /// <remarks>
        /// The PIN offset is a 4 digit number.
        /// CCM does not store PIN values.
        /// PIN offsets can only be stored if the institution configuration has this feature enabled.
        /// </remarks>
        /// <example>1122</example>
        [XmlElement(Order = 9)]
        public string PinOffset { get; set; }

        /// <summary>
        /// Indicates whether or not to reissue a new card when this card expires.
        /// </summary>
        /// <remarks>
        /// Possible values are True and False.
        /// False prevents the card from being reissued upon expiration.
        /// True configures the card so that it will be reissued upon expiration.
        /// If empty or missing, CCM will default to True.
        /// </remarks>
        /// <example>False</example>
        [XmlElement(Order = 10)]
        public string Reissue { get; set; }

        /// <summary>
        /// Indicates the method by which the new card will be ordered and delivered to the party.
        /// </summary>
        /// <remarks>
        /// Possible values are Batch or InstantIssue.
        /// Cards designated as Batch are typically ordered in the next card order job.
        /// Cards designated as InstantIssue are ignored and not included in a card order job, as the InstantIssue designation indicates that the party has already received the card.
        /// Use of the InstantIssue feature is an institution level setting in CCM.
        /// If this value is empty or missing, CCM will default to Batch.
        /// If InstantIssue is sent and the institution is not configured for InstantIssue an error will be returned.
        /// </remarks>
        /// <example>Batch</example>
        [XmlElement(Order = 11)]
        public string CardOrderType { get; set; }

        /// <summary>
        /// Indicates the name of design for the new card.
        /// </summary>
        /// <remarks>
        /// Products can be configured in CCM to allow multiple design types.
        /// The value sent for the DesignType must be a valid value for the product associated with the account number.
        /// If empty or missing, CCM will default to the product default design type.
        /// </remarks>
        /// <example>Indianapolis Colts Design</example>
        [XmlElement(Order = 12)]
        public string CardDesignName { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "Card"; }
        }
    }
}
