using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class ModifiedFields : DtoBase
    {
        /// <summary>
        /// Indicates field should be modified.
        /// </summary>
        /// <remarks>
        /// One AccountField tag should be present for every element in the Account node to be modified.
        /// For example, if the intent of the Update Account message was to modify the credit limit and lockout status of an account, then there should be an AccountField tag with a value of CreditLimit and another AccountField tag with a value of LockoutStatus.
        /// Only tags referenced by AccountField tags will be processed; extraneous tags in the Account node will be ignored.
        /// If an AccountField tag for a non-required tag is included and then the corresponding tag is empty or missing, CCM will delete any existing information or assign system default values.
        /// If an AccountField tag for a conditionally required tag is included and then the corresponding tag is empty or missing, CCM will not process the message and will return an error.
        /// </remarks>
        [XmlElement(Order = 1)]
        public List<string> AccountField { get; set; }

        /// <summary>
        /// Indicates field should be modified.
        /// </summary>
        /// <remarks>
        /// One CardField tag should be present for every element in the Card node to be modified.
        /// For example, if the intent of the UpdateCard message was to modify the party assigned to the card, then there should be a CardField tag with a value of CardholderPartyNumber and another CardField tag with a value of CardholderPartyType.
        /// Only tags referenced by CardField tags will be processed; extraneous tags in the Card node will be ignored.
        /// If a CardField tag for a non-required tag is included and then the corresponding tag is empty or missing, CCM will delete any existing information or assign system default values.
        /// If a CardField tag for a conditionally required tag is included and then the corresponding tag is empty or missing, CCM will not process the message and will return an error.
        /// </remarks>
        [XmlElement(Order = 2)]
        public List<string> CardField { get; set; }

        /// <summary>
        /// Indicates field should be modified.
        /// </summary>
        /// <remarks>
        /// One AccountPartyRelationshipField tag should be present for every element in the AccountPartyRelationship node to be modified.
        /// Only tags referenced by AccountPartyRelationshipField tags will be processed; extraneous tags in the AccountPartyRelationship node will be ignored.
        /// If an AccountPartyRelationshipField tag for a non-required tag is included and then the corresponding tag is empty or missing, CCM will delete any existing information or assign system default values.
        /// If an AccountPartyRelationshipField tag for a conditionally required tag is included and then the corresponding tag is empty or missing, CCM will not process the message and will return an error.
        /// </remarks>
        [XmlElement(Order = 3)]
        public List<string> AccountPartyRelationshipField { get; set; }

        /// <summary>
        /// Indicates field should be modified.
        /// </summary>
        /// <remarks>
        /// One PersonField tag should be present for every element in the Person node to be modified.
        /// For example, if the intent of the UpdatePerson message was to modify the tax id number, then there should be a PersonField tag with a value of TaxIdNumber.
        /// Only tags in the Person node referenced by PersonField tags will be processed; extraneous tags in the Person node will be ignored.
        /// If a PersonField tag for a non-required tag is included and then the corresponding tag is empty or missing, CCM will delete any existing information or assign system default values.
        /// If a PersonField tag for a conditionally required tag is included and then the corresponding tag is empty or missing, CCM will not process the message and will return an error.
        /// </remarks>
        [XmlElement(Order = 4)]
        public List<string> PersonField { get; set; }

        /// <summary>
        /// Indicates field should be modified.
        /// </summary>
        /// <remarks>
        /// One OrganizationField tag should be present for every element in the Organization node to be modified.
        /// For example, if the intent of the UpdateOrganization message was to modify the tax id number, then there should be an Organization Field tag with a value of TaxIdNumber.
        /// Only tags in the Organization node referenced by OrganizationField tags will be processed; extraneous tags in the Organization node will be ignored.
        /// If an OrganizationField tag for a non-required tag is included and then the corresponding tag is empty or missing, CCM will delete any existing information or assign system default values.
        /// If an OrganizationField tag for a conditionally required tag is included and then the corresponding tag is empty or missing, CCM will not process the message and will return an error.
        /// </remarks>
        [XmlElement(Order = 5)]
        public List<string> OrganizationField { get; set; }

        /// <summary>
        /// Indicates field should be modified.
        /// </summary>
        /// <remarks>
        /// One AddressField tag should be present for every element in the PrimaryAddress node to be modified.
        /// Only tags in the PrimaryAddress node referenced by AddressField tags will be processed; extraneous tags in the PrimaryAddress node will be ignored.
        /// If an AddressField tag for a non-required tag is included and then the corresponding tag is empty or missing, CCM will delete any existing information or assign system default values.
        /// If an AddressField tag for a conditionally required tag is included and then the corresponding tag is empty or missing, CCM will not process the message and will return an error.
        /// </remarks>
        [XmlElement(Order = 6)]
        public List<string> AddressField { get; set; }

        /// <summary>
        /// Indicates field should be modified.
        /// </summary>
        /// <remarks>
        /// One EmailField tag should be present for every element in the PrimaryEmail node to be modified.
        /// Only tags in the PrimaryEmail node referenced by EmailField tags will be processed; extraneous tags in the PrimaryEmail node will be ignored.
        /// If an EmailField tag for a non-required tag is included and then the corresponding tag is empty or missing, CCM will delete any existing information or assign system default values.
        /// If an EmailField tag for a conditionally required tag is included and then the corresponding tag is empty or missing, CCM will not process the message and will return an error.
        /// </remarks>
        [XmlElement(Order = 7)]
        public List<string> EmailField { get; set; }

        /// <summary>
        /// Indicates field should be modified.
        /// </summary>
        /// <remarks>
        /// One PhoneField tag should be present for every element in the PrimaryPhone node to be modified.
        /// Only tags in the PrimaryPhone node referenced by PhoneField tags will be processed; extraneous tags in the PrimaryPhone node will be ignored.
        /// If a PhoneField tag for a non-required tag is included and then the corresponding tag is empty or missing, CCM will delete any existing information or assign system default values.
        /// If a PhoneField tag for a conditionally required tag is included and then the corresponding tag is empty or missing, CCM will not process the message and will return an error.
        /// </remarks>
        [XmlElement(Order = 8)]
        public List<string> PhoneField { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "ModifiedFields"; }
        }
    }
}
