using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    /// <summary>
    /// This DTO is only meant to be used with SOAP calls.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "MessageResponse", Namespace = "http://www.opensolutions.com/CCM/CcmMessageResponse.xsd")]
    public class MessageResponse : DtoBase
    {
        [XmlAttribute(AttributeName = "msg", Namespace = "http://www.opensolutions.com/CCM/CcmMessage.xsd", Form = XmlSchemaForm.Qualified)]
        public string Msg { get; set; }
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "msg", Namespace = "http://www.w3.org/2000/xmlns/")]

        public string TraceNumber { get; set; }
        public string ResponseCode { get; set; }
        public string ErrorMessage { get; set; }
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
        public string PersonPartyId { get; set; }
        public string OrganizationPartyId { get; set; }
        public string AddressVerificationResponseCode { get; set; }
        public decimal AvailableCredit { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "MessageResponse"; }
        }
    }
}
