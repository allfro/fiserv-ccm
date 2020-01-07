using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class Person : DtoBase
    {
        /// <summary>
        /// The PartyId to be assigned to the person.
        /// </summary>
        /// <example>1234</example>
        [XmlElement(Order = 1)]
        public string PartyNumber { get; set; }

        /// <summary>
        /// Social Security Number of person being added. SSN is a required field in the CCM database.
        /// </summary>
        /// <remarks>
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
        /// Indicates person's type of relationship to institution.
        /// </summary>
        /// <remarks>
        /// Possible values are Customer and Other.
        /// If tag is empty or missing, CCM will default to Customer when creating a new person record.
        /// </remarks>
        /// <example>Customer</example>
        [XmlElement(Order = 6)]
        public string InstitutionRelationShipType { get; set; }

        /// <summary>
        /// Last name of person to add.
        /// </summary>
        /// <example>Smith</example>
        [XmlElement(Order = 7)]
        public string LastName { get; set; }

        /// <summary>
        /// First name of person to add.
        /// </summary>
        /// <example>John</example>
        [XmlElement(Order = 8)]
        public string FirstName { get; set; }

        /// <summary>
        /// Middle name of person to add.
        /// </summary>
        /// <example>Alexander</example>
        [XmlElement(Order = 9)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Date of birth of person to add in yyyy-MM-dd format. This field is required in the CCM database.
        /// </summary>
        /// <remarks>
        /// If the tag is empty or missing, CCM will default to current date.
        /// </remarks>
        /// <example>1976-01-09</example>
        [XmlElement(Order = 10)]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Date of death of person in yyyy-MM-dd format.
        /// </summary>
        /// <example>2007-05-18</example>
        [XmlElement(Order = 11)]
        public string DateOfDeath { get; set; }

        /// <summary>
        ///  Title for person to add.
        /// </summary>
        /// <remarks>
        /// Possible values are Mr., Mrs., Dr., Ms., Miss., Other.
        /// </remarks>
        /// <example>Miss.</example>
        [XmlElement(Order = 12)]
        public string Title { get; set; }

        /// <summary>
        ///  Suffix for name of person to add.
        /// </summary>
        /// <remarks>
        /// Possible values are Jr., Sr., and Other.
        /// </remarks>
        /// <example>Jr.</example>
        [XmlElement(Order = 13), DefaultValue("")]
        public string NameSuffix { get; set; }

        /// <summary>
        ///  Maiden name of the mother of the person to add.
        /// </summary>
        /// <example>Brown</example>
        [XmlElement(Order = 14)]
        public string MothersMaidenName { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "Person"; }
        }
    }
}
