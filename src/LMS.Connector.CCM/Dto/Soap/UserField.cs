using System;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class UserField : DtoBase
    {
        /// <summary>
        /// Name of the user field to be added.
        /// </summary>
        /// <remarks>
        /// Value must be valid user field name for this product in the CCM database.
        /// </remarks>
        /// <example>Credit Score</example>
        [XmlElement(Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Value for the user field.
        /// </summary>
        /// <remarks>
        /// User fields indicated in Name tag can be configured in CCM to either accept any value or to accept only validated list.
        /// If the user field is configured to accept only certain values, this value must be a valid value.
        /// If the user field is configured to accept any value, then any value can be sent here.
        /// </remarks>
        /// <example>650</example>
        [XmlElement(Order = 2)]
        public string Value { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "UserField"; }
        }
    }
}
