using System;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class PrimaryEmail : DtoBase
    {
        /// <summary>
        /// Email address to be added.
        /// </summary>
        /// <example>John.smith@isp.org</example>
        [XmlElement(Order = 1)]
        public string EmailAddress { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "EmailAddress"; }
        }
    }
}
