using System.Collections.Generic;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM
{
    public class RestStrategy : ClientStrategy
    {
        private IRestRepository _repository;
        private Credentials _credentials;

        public IRestRepository Repository
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

        public RestStrategy()
        {

        }

        public RestStrategy(Application app, string userToken) : base(app, userToken)
        {
            _repository = new RestRepository(userToken);

            _testConnectionBehavior = new Behaviors.Rest.TestConnectionBehavior(_userToken, _repository);
            _inquryBehavior = new Behaviors.Rest.InquiryBehavior(_app, _userToken);
        }

        public RestStrategy(Application app, string userToken, Credentials credentials) : base(app, userToken)
        {
            _repository = new RestRepository(userToken, credentials);
            _credentials = credentials;

            _testConnectionBehavior = new Behaviors.Rest.TestConnectionBehavior(_userToken, _repository);
            _inquryBehavior = new Behaviors.Rest.InquiryBehavior(_app, _userToken, _repository);
        }

        public RestStrategy(Application app, string userToken, IRestRepository repository) : base(app, userToken)
        {
            _repository = repository;

            _testConnectionBehavior = new Behaviors.Rest.TestConnectionBehavior(_userToken, _repository);
            _inquryBehavior = new Behaviors.Rest.InquiryBehavior(_app, _userToken, _repository);
        }

        public override BaseResult Inquiry(string personNumber, out string errorMessage, out IList<RelationshipInfo> relationshipInfos)
        {
            var result = InquiryBehavior.Inquiry(personNumber);
            errorMessage = InquiryBehavior.ErrorMessage;
            relationshipInfos = InquiryBehavior.RelationshipInfos;

            return result;
        }
    }
}
