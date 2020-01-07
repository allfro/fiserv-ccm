using System.Collections.Generic;
using System.Xml.Serialization;
using Akcelerant.Core.Data.DTO.Host;
using Newtonsoft.Json;

namespace LMS.Connector.CCM.Dto
{
    public abstract class DtoBase
    {
        private List<HostValue> _hostValues = new List<HostValue>();

        /// <summary>
        /// Host values that will either be exported when sending a request to the core, or host values that were imported via a response from core.
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public List<HostValue> HostValues
        {
            get { return _hostValues; }
            set { _hostValues = value; }
        }

        /// <summary>
        /// Defines the parent node that will be prepended to imported host values
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public abstract string HostValueParentNode { get; }
    }
}
