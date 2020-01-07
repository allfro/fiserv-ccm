namespace LMS.Connector.CCM.Dto.Rest
{
    public class Header : DtoBase
    {
        public string ContentType { get; set; }
        public string Authorization { get; set; }

        public override string HostValueParentNode
        {
            get { return "Header"; }
        }
    }
}
