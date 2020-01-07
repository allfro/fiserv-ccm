using System;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class AddAccount : DtoBase
    {
        public Message Message { get; set; }

        public override string HostValueParentNode
        {
            get { return "AddAccount"; }
        }
    }
}
