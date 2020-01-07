using System;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class AccountPartyRelationship : DtoBase
    {
        /// <summary>
        /// Indicates type of action.
        /// </summary>
        /// <remarks>Value should always be Add for adding an account party relationship.</remarks>
        [XmlElement(Order = 1)]
        public string UpdateAction { get; set; }

        /// <summary>
        /// ModifiedFields element must be present for UpdateAccountPartyRelationship.
        /// </summary>
        [XmlElement(Order = 2)]
        public ModifiedFields ModifiedFields { get; set; }

        /// <summary>
        /// CCM account number to associate with new account party relationship.
        /// </summary>
        /// <remarks>
        /// This account number must exist in the CCM database prior to sending the AddAccountPartyRelationship message.
        /// This account must have an account status of Active.
        /// </remarks>
        /// <example>12345689</example>
        [XmlElement(Order = 3)]
        public string AccountNumber { get; set; }

        /// <summary>
        /// The party id of the party associated with the account party relationship.
        /// </summary>
        /// <remarks>
        /// This tag is used in conjunction with the PartyType to identify the party.
        /// This party must exist in the CCM database prior to receiving AddAccountPartyRelationship message.
        /// TCCUS users: This corresponds to the Pers.PersNbr or Org.OrgNbr from Core of the party.
        /// </remarks>
        /// <example>1234</example>
        [XmlElement(Order = 4)]
        public string PartyNumber { get; set; }

        /// <summary>
        /// The party type of the party associated with the party account relationship.
        /// </summary>
        /// <remarks>
        /// Possible values are Person or Organization.
        /// This tag is used in conjunction with the PartyNumber to identify the party.
        /// This party must exist in the CCM database prior to receiving AddAccountPartyRelationship message.
        /// </remarks>
        /// <example>Organization</example>
        [XmlElement(Order = 5)]
        public string PartyType { get; set; }

        /// <summary>
        /// Indicates type of relationship party will have on account.
        /// </summary>
        /// <remarks>
        /// Possible values are Owner and CoMaker.
        /// If tag is empty or missing CCM will default to Owner.
        /// </remarks>
        /// <example>CoMaker</example>
        [XmlElement(Order = 6)]
        public string AccountRelationshipType { get; set; }

        /// <summary>
        /// Effective date for adding party to account in yyyy-MM-dd format.
        /// </summary>
        /// <remarks>
        /// If tag is empty or missing CCM will default to current date.
        /// If tag does not have a value, do not send tag at all; do not leave it empty
        /// </remarks>
        /// <example>2007-05-17</example>
        [XmlElement(Order = 7)]
        public string OriginationDate { get; set; }

        /// <summary>
        /// Indicates whether statements should be mailed to the party.
        /// </summary>
        /// <remarks>
        /// Possible values are True and False.
        /// True indicates that a statement should be mailed to the party as long as the party is not bankrupt.
        /// False indicates that statements should not be mailed to the party.
        /// </remarks>
        /// <example>True</example>
        [XmlElement(Order = 8)]
        public string SendStatement { get; set; }

        /// <summary>
        /// Indicates the manner by which statements should be delivered to the party.
        /// </summary>
        /// <remarks>
        /// The delivery channel is configurable per institution.
        /// If a value is sent, it must be a valid value in the CCM database.
        /// If tag is empty or missing, CCM will default to the institution default value.
        /// </remarks>
        /// <example>E-Statements</example>
        [XmlElement(Order = 9)]
        public string StatementDeliveryChannel { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "AccountPartyRelationship"; }
        }
    }
}
