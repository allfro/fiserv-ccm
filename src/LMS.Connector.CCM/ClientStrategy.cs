using System.Collections.Generic;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Behaviors;
using LMS.Connector.CCM.Models;

namespace LMS.Connector.CCM
{
    public abstract class ClientStrategy
    {
        protected ITestConnection _testConnectionBehavior;
        protected IAddAccount _addAccountBehavior;
        protected IAddCard _addCardBehavior;
        protected IAddPerson _addPersonBehavior;
        protected IAddOrganization _addOrganizationBehavior;
        protected IAddAccountPartyRelationship _addAccountPartyRelationshipBehavior;
        protected IInquiry _inquryBehavior;
        protected IUpdateAccount _updateAccountBehavior;

        protected Application _app;
        protected string _userToken;

        public ITestConnection TestConnectionBehavior
        {
            get
            {
                return _testConnectionBehavior;
            }
            set
            {
                _testConnectionBehavior = value;
            }
        }

        public IAddAccount AddAccountBehavior
        {
            get
            {
                return _addAccountBehavior;
            }
            set
            {
                _addAccountBehavior = value;
            }
        }

        public IAddCard AddCardBehavior
        {
            get
            {
                return _addCardBehavior;
            }
            set
            {
                _addCardBehavior = value;
            }
        }

        public IAddPerson AddPersonBehavior
        {
            get
            {
                return _addPersonBehavior;
            }
            set
            {
                _addPersonBehavior = value;
            }
        }

        public IAddOrganization AddOrganizationBehavior
        {
            get
            {
                return _addOrganizationBehavior;
            }
            set
            {
                _addOrganizationBehavior = value;
            }
        }

        public IAddAccountPartyRelationship AddAccountPartyRelationshipBehavior
        {
            get
            {
                return _addAccountPartyRelationshipBehavior;
            }
            set
            {
                _addAccountPartyRelationshipBehavior = value;
            }
        }

        public IInquiry InquiryBehavior
        {
            get
            {
                return _inquryBehavior;
            }
            set
            {
                _inquryBehavior = value;
            }
        }

        public IUpdateAccount UpdateAccountBehavior
        {
            get
            {
                return _updateAccountBehavior;
            }
            set
            {
                _updateAccountBehavior = value;
            }
        }

        public Application Application
        {
            get
            {
                return _app;
            }
            set
            {
                _app = value;
            }
        }

        public string UserToken
        {
            get
            {
                return _userToken;
            }
            set
            {
                _userToken = value;
            }
        }

        public ClientStrategy()
        {

        }

        public ClientStrategy(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
        }

        /// <summary>
        /// Tests the connection to a CCM service.
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="facility"></param>
        /// <returns></returns>
        public virtual BaseResult TestConnection(string serviceUrl, string userName, string password, string facility)
        {
            return TestConnectionBehavior.TestConnection(serviceUrl, userName, password, facility);
        }

        /// <summary>
        /// Creates a new CCM credit card account.
        /// </summary>
        /// <param name="primaryApplicant"></param>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public virtual BaseResult AddAccount(Applicant primaryApplicant, out string accountNumber)
        {
            accountNumber = string.Empty;

            return AddAccountBehavior.AddAccount(primaryApplicant);
        }

        /// <summary>
        /// Makes a call to create cards for the Primary
        /// </summary>
        /// <param name="person"></param>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public virtual BaseResult AddCard(LmsPerson lmsPerson, out string cardNumber)
        {
            cardNumber = string.Empty;

            return AddCardBehavior.AddCard(lmsPerson);
        }

        /// <summary>
        /// Creates a Person in CCM. This Person must already exist in DNA/OSI.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public virtual BaseResult AddPerson(LmsPerson lmsPerson)
        {
            return AddPersonBehavior.AddPerson(lmsPerson);
        }

        /// <summary>
        /// Creates an Organization in CCM. This Organization must already exist in DNA/OSI.
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public virtual BaseResult AddOrganization(Applicant organization)
        {
            return AddOrganizationBehavior.AddOrganization(organization);
        }

        /// <summary>
        /// Adds a Joint CCM credit card account.
        /// </summary>
        /// <param name="jointApplicant"></param>
        /// <returns></returns>
        public virtual BaseResult AddAccountPartyRelationship(Applicant jointApplicant)
        {
            return AddAccountPartyRelationshipBehavior.AddAccountPartyRelationship(jointApplicant);
        }

        /// <summary>
        /// Determines if each Applicant (Primary or Joint(s)) and Authorizaed User(s) exists in CCM.
        /// </summary>
        /// <remarks>Person/Organization number must exist on the Application</remarks>
        /// <param name="personNumber"></param>
        /// <param name="errorMessage"></param>
        /// <param name="relationshipInfos"></param>
        /// <returns></returns>
        public virtual BaseResult Inquiry(string personNumber, out string errorMessage, out IList<Dto.Rest.RelationshipInfo> relationshipInfos)
        {
            errorMessage = string.Empty;
            relationshipInfos = null;

            return InquiryBehavior.Inquiry(personNumber);
        }

        /// <summary>
        /// Updates the credit limit on the CCM credit card account.
        /// </summary>
        /// <param name="primaryApplicant"></param>
        /// <returns></returns>
        public virtual BaseResult UpdateAccount(Applicant primaryApplicant)
        {
            return UpdateAccountBehavior.UpdateAccount(primaryApplicant);
        }
    }
}
