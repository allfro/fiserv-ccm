namespace LMS.Connector.CCM.Dto.Rest
{
    public class Link : DtoBase
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Method { get; set; }

        public override string HostValueParentNode
        {
            get { return "Link"; }
        }
    }
}
