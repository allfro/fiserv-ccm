namespace LMS.Connector.CCM.Dto.Rest
{
    public class Session : DtoBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Authentication { get; set; }
        public string IpAddress { get; set; }

        public override string HostValueParentNode
        {
            get { return "Session"; }
        }
    }
}
