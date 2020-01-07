using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class PrimaryAddress : DtoBase
    {
        /// <summary>
        /// First line of address record.
        /// </summary>
        /// <example>123 Oak Court</example>
        [XmlElement(Order = 1)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Second line of address record.
        /// </summary>
        /// <example>Apt. 2</example>
        [XmlElement(Order = 2)]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// City of address record
        /// </summary>
        /// <example>Indianapolis</example>
        [XmlElement(Order = 3)]
        public string City { get; set; }

        /// <summary>
        /// State or Province abbreviation for address record.
        /// </summary>
        /// <example>IN</example>
        [XmlElement(Order = 4)]
        public string StateProvince { get; set; }

        /// <summary>
        /// Postal (ZIP) code for address record.
        /// </summary>
        /// <example>46521</example>
        [XmlElement(Order = 5)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Country code abbreviation for address record.  This tag is not required, even if PrimaryAddress node is sent.
        /// </summary>
        /// <remarks>
        /// If this tag is empty or missing CCM will default to US.
        /// </remarks>
        /// <example>US</example>
        [XmlElement(Order = 6)]
        public string CountryCode { get; set; }

        /// <summary>
        /// Indicates the type of address.
        /// </summary>
        /// <remarks>
        /// If tag is empty or missing CCM will default to Home.
        /// Possible values are the following:
        /// Business – indicates business address
        /// Home – indicates home address
        /// International – indicates international address
        /// Military – indicates military address
        /// POBox – indicates post office box
        /// </remarks>
        /// <example>Home</example>
        [XmlElement(Order = 7), DefaultValue("")]
        public string AddressType { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "PrimaryAddress"; }
        }
    }
}
