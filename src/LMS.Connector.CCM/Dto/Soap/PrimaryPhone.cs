using System;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class PrimaryPhone : DtoBase
    {
        /// <summary>
        /// Country calling code for phone record.  A value must be sent for phone records outside of the United States.
        /// </summary>
        /// <example>954</example>
        [XmlElement(Order = 1)]
        public string CountryCallingCode { get; set; }

        /// <summary>
        /// City area code for phone record.
        /// </summary>
        /// <example>317</example>
        [XmlElement(Order = 2)]
        public string CityAreaCode { get; set; }

        /// <summary>
        /// Seven digit phone number.
        /// </summary>
        /// <example>2452456</example>
        [XmlElement(Order = 3)]
        public string LocalPhoneNumber { get; set; }

        /// <summary>
        /// Extension for phone record.
        /// </summary>
        /// <example>3300</example>
        [XmlElement(Order = 4)]
        public string Extension { get; set; }

        /// <summary>
        /// Description for phone record.
        /// </summary>
        /// <example>Primary contact number</example>
        [XmlElement(Order = 5)]
        public string Description { get; set; }

        /// <summary>
        /// Indicates the type of phone record to be added.
        /// </summary>
        /// <remarks>
        /// If tag is empty or missing CCM will default to Home.
        /// Possible values are the following:
        /// Business – indicates business number
        /// Fax – indicates fax number
        /// Home – indicates home number
        /// Mobile – indicates mobile number
        /// </remarks>
        /// <example>Home</example>
        [XmlElement(Order = 6)]
        public string PhoneType { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "PrimaryPhone"; }
        }
    }
}
