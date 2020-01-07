using System;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class AddAccountPartyRelationship : DtoBase
    {
        public Message Message { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "AddAccountPartyRelationship"; }
        }
    }
}
