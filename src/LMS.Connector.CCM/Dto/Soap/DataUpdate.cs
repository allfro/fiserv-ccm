using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class DataUpdate : DtoBase
    {
        /// <summary>
        /// Should be unique number by source.
        /// </summary>
        /// <remarks>
        /// Facilitates searching/troubleshooting in CCM.
        /// </remarks>
        /// <example>1234568</example>
        [XmlElement(Order = 1)]
        public string TraceNumber { get; set; }


        /// <summary>
        /// Processing code for message.
        /// </summary>
        /// <remarks>Value should always be "ExternalUpdateRequest" for this message.</remarks>
        [XmlElement(Order = 2)]
        public string ProcessingCode { get; set; }

        /// <summary>
        /// The agreed upon source name for the message.
        /// </summary>
        /// <remarks>
        /// For example, TCCUS, LoanOrigination, MultiPoint, etc. Facilitates searching/troubleshooting.
        /// LoanOrigination is a system defined value in CCM that can be used. If the FI wants to send another value,
        /// it needs to be configured in CCM.
        /// </remarks>
        [XmlElement(Order = 3)]
        public string Source { get; set; }

        /// <summary>
        /// Indicates type of action.
        /// </summary>
        /// <remarks>
        /// Value should always be "Add" for AddPerson.
        /// Value should always be "Add" for AddOrganization.
        /// Value should always be "Add" for AddAccount.
        /// Value should always be "Indeterminate" for AccountPartyRelationship messages.
        /// Value should always be "Add" for AddCard.
        /// Value should always be "Modify" for UpdateAccount.
        /// </remarks>
        [XmlElement(Order = 4)]
        public string UpdateAction { get; set; }

        /// <summary>
        /// Indicates target of UpdateAction.
        /// </summary>
        /// <remarks>
        /// Value should always be "Account" for AddAccount.
        /// Value should always be "AccountPartyRelationships" for AddAccountPartyRelationship.
        /// Value should always be "Card" for AddCard.
        /// </remarks>
        [XmlElement(Order = 5)]
        public string UpdateTarget { get; set; }

        [XmlElement(Order = 6)]
        public Account Account { get; set; }

        [XmlElement(Order = 7)]
        public Card Card { get; set; }

        [XmlArray(ElementName = "AccountPartyRelationships", Order = 8)]
        [XmlArrayItem(Type = typeof(AccountPartyRelationship), ElementName = "AccountPartyRelationship")]
        public List<AccountPartyRelationship> AccountPartyRelationships { get; set; }

        [XmlElement(Order = 9)]
        public Person Person { get; set; }

        [XmlElement(Order = 10)]
        public Organization Organization { get; set; }

        [XmlElement(Order = 11)]
        public List<ModifiedFields> ModifiedFields { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "DataUpdate"; }
        }
    }
}
