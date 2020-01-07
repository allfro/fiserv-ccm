namespace LMS.Connector.CCM.Dto.Rest
{
    public class PartyRelationshipsInquiry : DtoBase
    {
        private string _apiVersion = "v1";

        public string ApiVersion
        {
            get
            {
                return _apiVersion;
            }
            set
            {
                _apiVersion = value;
            }
        }

        /// <summary>
        /// The party id.
        /// </summary>
        public int PartyId { get; set; }

        public override string HostValueParentNode
        {
            get { return "Inquiry"; }
        }
    }
}
