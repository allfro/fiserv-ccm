﻿using System;
using System.Xml.Serialization;

namespace LMS.Connector.CCM.Dto.Soap
{
    [Serializable]
    public class UpdatePerson : DtoBase
    {
        public Message Message { get; set; }

        [XmlIgnore]
        public override string HostValueParentNode
        {
            get { return "UpdatePerson"; }
        }
    }
}
