namespace LMS.Connector.CCM.Dto.Rest
{
    public class RelationshipInfo : DtoBase
    {
        public string AccountId { get; set; }
        public string AccountNumber { get; set; }
        public bool IsMaster { get; set; }
        public string MasterAccountId { get; set; }
        public string MasterAccountNumber { get; set; }
        public string ProductName { get; set; }
        public string AccountStatus { get; set; }
        public string PartName { get; set; }
        public string AccountRelationshipType { get; set; }
        public bool IsTaxOwner { get; set; }
        public string CardId { get; set; }
        public string CardNumber { get; set; }
        public string CardExpirationDate { get; set; }
        public string CardStatus { get; set; }
        public string PartyId { get; set; }
        public string PartyType { get; set; }
        public string CardHolderName { get; set; }
        public string OtherName { get; set; }

        public override string HostValueParentNode
        {
            get { return "RelationshipInfo"; }
        }
    }
}
