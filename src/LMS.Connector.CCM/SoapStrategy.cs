using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM
{
    public class SoapStrategy : ClientStrategy
    {
        private ISoapRepository _repository;
        private Credentials _credentials;

        public ISoapRepository Repository
        {
            get
            {
                return _repository;
            }
            set
            {
                _repository = value;
            }
        }

        public Credentials Credentials
        {
            get
            {
                return _credentials;
            }
            set
            {
                _credentials = value;
            }
        }

        public SoapStrategy()
        {

        }

        public SoapStrategy(Application app, string userToken) : base(app, userToken)
        {
            _repository = new SoapRepository(userToken);

            _addPersonBehavior = new Behaviors.Soap.AddPersonBehavior(_app, _userToken, _repository);
            _addOrganizationBehavior = new Behaviors.Soap.AddOrganizationBehavior(_app, _userToken, _repository);
            _addAccountBehavior = new Behaviors.Soap.AddAccountBehavior(_app, _userToken, _repository);
            _addAccountPartyRelationshipBehavior = new Behaviors.Soap.AddAccountPartyRelationshipBehavior(_app, _userToken, _repository);
            _addCardBehavior = new Behaviors.Soap.AddCardBehavior(_app, _userToken, _repository);
            _updateAccountBehavior = new Behaviors.Soap.UpdateAccountBehavior(_app, _userToken, _repository);
        }

        public SoapStrategy(Application app, string userToken, Credentials credentials) : base(app, userToken)
        {
            _repository = new SoapRepository(userToken, credentials);
            _credentials = credentials;

            _addPersonBehavior = new Behaviors.Soap.AddPersonBehavior(_app, _userToken, _repository);
            _addOrganizationBehavior = new Behaviors.Soap.AddOrganizationBehavior(_app, _userToken, _repository);
            _addAccountBehavior = new Behaviors.Soap.AddAccountBehavior(_app, _userToken, _repository);
            _addAccountPartyRelationshipBehavior = new Behaviors.Soap.AddAccountPartyRelationshipBehavior(_app, _userToken, _repository);
            _addCardBehavior = new Behaviors.Soap.AddCardBehavior(_app, _userToken, _repository);
            _updateAccountBehavior = new Behaviors.Soap.UpdateAccountBehavior(_app, _userToken, _repository);
        }

        public SoapStrategy(Application app, string userToken, ISoapRepository repository) : base(app, userToken)
        {
            _repository = repository;

            _addPersonBehavior = new Behaviors.Soap.AddPersonBehavior(_app, _userToken, _repository);
            _addOrganizationBehavior = new Behaviors.Soap.AddOrganizationBehavior(_app, _userToken, _repository);
            _addAccountBehavior = new Behaviors.Soap.AddAccountBehavior(_app, _userToken, _repository);
            _addAccountPartyRelationshipBehavior = new Behaviors.Soap.AddAccountPartyRelationshipBehavior(_app, _userToken, _repository);
            _addCardBehavior = new Behaviors.Soap.AddCardBehavior(_app, _userToken, _repository);
            _updateAccountBehavior = new Behaviors.Soap.UpdateAccountBehavior(_app, _userToken, _repository);
        }

        public override BaseResult TestConnection(string serviceUrl, string userName, string password, string facility)
        {
            TestConnectionBehavior = new Behaviors.Soap.TestConnectionBehavior(_app, _userToken, _repository);

            var result = TestConnectionBehavior.TestConnection(serviceUrl, userName, password, facility);

            return result;
        }

        public override BaseResult AddAccount(Applicant primaryApplicant, out string accountNumber)
        {
            var result = AddAccountBehavior.AddAccount(primaryApplicant);
            accountNumber = AddAccountBehavior.MessageResponse?.AccountNumber;

            return result;
        }

        public override BaseResult AddCard(LmsPerson lmsPerson, out string cardNumber)
        {
            var result = AddCardBehavior.AddCard(lmsPerson);
            cardNumber = AddCardBehavior.MessageResponse?.CardNumber;

            return result;
        }
    }
}
