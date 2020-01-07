using System.Collections.Generic;

namespace LMS.Connector.CCM.Dto.Rest
{
    public class PartyRelationshipsInquiryResponse : DtoBase
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public int TotalItemCount { get; set; }

        public IList<RelationshipInfo> RelationshipInfos { get; set; }
        public IList<Link> Links { get; set; }

        public override string HostValueParentNode
        {
            get { return "PartyRelationshipResponse"; }
        }
    }
}
