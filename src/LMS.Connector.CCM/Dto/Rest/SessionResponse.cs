namespace LMS.Connector.CCM.Dto.Rest
{
    public class SessionResponse : DtoBase
    {
        public string AuthToken { get; set; }

        public override string HostValueParentNode
        {
            get { return "SessionResponse"; }
        }
    }
}
