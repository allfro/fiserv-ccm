using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    [XmlRoot(Namespace = "http://www.opensolutions.com/CCM/CcmMessage.xsd")]
    public class Message : DtoBase
    {
        public DataUpdate DataUpdate { get; set; }

        [XmlAttribute(AttributeName = "msg", Namespace = "http://www.opensolutions.com/CCM/CcmMessage.xsd", Form = XmlSchemaForm.Qualified)]
        public string Msg { get; set; }

        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation = "http://www.opensolutions.com/CCM/CcmMessage.xsd CcmMessage.xsd";

        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Xsi { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "Message"; }
        }
    }
}
