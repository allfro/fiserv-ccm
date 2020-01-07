using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class Organization : DtoBase
    {
        /// <summary>
        /// The PartyId to be assigned to the organization.
        /// </summary>
        /// <example>1234</example>
        [XmlElement(Order = 1)]
        public string PartyNumber { get; set; }

        /// <summary>
        /// Tax identification number of organization being added. The tax id is a required field in the CCM database.
        /// </summary>
        /// <remarks>
        /// Possible values are Member and Non-Member.
        /// If this tag is empty or missing, CCM will default to 999999999.
        /// </remarks>
        /// <example>111223333</example>
        [XmlElement(Order = 2)]
        public string TaxIdNumber { get; set; }

        [XmlElement(Order = 3)]
        public PrimaryAddress PrimaryAddress { get; set; }

        [XmlElement(Order = 4)]
        public PrimaryEmail PrimaryEmail { get; set; }

        [XmlElement(Order = 5)]
        public PrimaryPhone PrimaryPhone { get; set; }

        /// <summary>
        /// Indicates organization's type of relationship to institution.
        /// </summary>
        /// <remarks>
        /// If tag is empty or missing, CCM will default to Customer when creating a new person record.
        /// </remarks>
        /// <example>Customer</example>
        [XmlElement(Order = 6)]
        public string InstitutionRelationShipType { get; set; }

        /// <summary>
        /// Name of organization to add.
        /// </summary>
        /// <example>Smith's Bait and Tackle</example>
        [XmlElement(Order = 7)]
        public string Name { get; set; }

        /// <summary>
        /// Type of business.
        /// </summary>
        /// <remarks>
        /// Possible values are Corporation, Partnership, and SoleProprietorship.
        /// If the tag is empty or missing, CCM will default to Corporation.
        /// </remarks>
        /// <example>Corporation</example>
        [XmlElement(Order = 8), DefaultValue("")]
        public string BusinessOwnershipType { get; set; }

        /// <summary>
        /// Date organization was created in yyyy-MM-dd format. This field is required in the CCM database.
        /// </summary>
        /// <remarks>
        /// If the tag is empty or missing, CCM will default to current date.
        /// If tag does not have a value, do not send tag at all; do not leave it empty.
        /// </remarks>
        /// <example>1976-01-09</example>
        [XmlElement(Order = 9)]
        public string CreationDate { get; set; }

        /// <summary>
        /// Standard Industrial Classification code for organization.
        /// </summary>
        /// <example>1415</example>
        [XmlElement(Order = 10)]
        public string SicCode { get; set; }

        /// <summary>
        /// North American Industry Classification System code for organization.
        /// </summary>
        /// <example>1415</example>
        [XmlElement(Order = 11)]
        public string NaicsCode { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "Organization"; }
        }
    }
}
