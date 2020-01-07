using System;
using System.Linq;
using System.Xml;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.CCMSoapWebService;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Helpers;
using LMS.Connector.CCM.Models;
using LMS.Core.HostValues.Utility.Translator.Xml;

namespace LMS.Connector.CCM.Repositories
{
    public class SoapRepository : ISoapRepository
    {
        private const string SOLUTION_CODE = "CCM_ORIGINATION";
        private const string PARAMETER_CODE = "CCM_SOAP_SERVICE_URL";

        private string _userToken;
        private ILmsRepository _lmsRepository;
        private Credentials _credentials;
        private CredentialsHeader _credentialsHeader;
        private CcmWebServiceSoap _soapClient;
        private ProcessMessageNodeRequest _messageNodeRequest;
        private ProcessMessageNodeResponse _messageNodeResponse;
        private MessageResponse _messageResponse;
        private XmlDocument _xmlDoc;

        public SoapRepository()
        {

        }

        public SoapRepository(string userToken)
        {
            _userToken = userToken;
            _lmsRepository = new LmsRepository(userToken);
            _credentials = GetServiceCredentials();
            _soapClient = new CcmWebServiceSoapClient("CcmWebServiceSoap", _credentials.BaseUrl);
        }

        public SoapRepository(string userToken, Credentials credentials)
        {
            _userToken = userToken;
            _lmsRepository = new LmsRepository(userToken);
            _credentials = credentials;
            _soapClient = new CcmWebServiceSoapClient("CcmWebServiceSoap", _credentials.BaseUrl);
        }

        public SoapRepository(string userToken, Credentials credentials, CcmWebServiceSoap soapClient)
        {
            _userToken = userToken;
            _lmsRepository = new LmsRepository(userToken);
            _credentials = credentials;
            _soapClient = soapClient;
        }

        public SoapRepository(string userToken, Credentials credentials, CcmWebServiceSoap soapClient, ILmsRepository lmsRepository)
        {
            _userToken = userToken;
            _lmsRepository = lmsRepository;
            _credentials = credentials;
            _soapClient = soapClient;
        }

        #region "Interface implementations"

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

        public ILmsRepository LmsRepository
        {
            get
            {
                return _lmsRepository;
            }
            set
            {
                _lmsRepository = value;
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

        public CredentialsHeader CredentialsHeader
        {
            get
            {
                return GetCredentialsHeader();
            }
            set
            {
                _credentialsHeader = value;
            }
        }

        public CcmWebServiceSoap SoapClient
        {
            get
            {
                return _soapClient;
            }
            set
            {
                _soapClient = value;
            }
        }

        public ProcessMessageNodeRequest ProcessMessageNodeRequest
        {
            get
            {
                return _messageNodeRequest;
            }
            set
            {
                _messageNodeRequest = value;
            }
        }

        public ProcessMessageNodeResponse ProcessMessageNodeResponse
        {
            get
            {
                return _messageNodeResponse;
            }
            set
            {
                _messageNodeResponse = value;
            }
        }

        public MessageResponse MessageResponse
        {
            get
            {
                return _messageResponse;
            }
            set
            {
                _messageResponse = value;
            }
        }

        public ProcessMessageNodeRequest GetProcessMessageNodeRequest(CredentialsHeader credentialsHeader, string request)
        {
            ProcessMessageNodeRequest processMessageNodeRequest = null;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.GetProcessMessageNodeRequest"))
            {
                var newNode = GetXmlNodeRequest(request);

                processMessageNodeRequest = new ProcessMessageNodeRequest()
                {
                    CredentialsHeader = credentialsHeader,
                    request = newNode
                };
            }

            return processMessageNodeRequest;
        }

        public MessageResponse ProcessMessage(string message)
        {
            ClearMessageResponse();

            MessageResponse messageResponse = null;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.ProcessMessage(string)"))
            {
                if (_credentialsHeader == null)
                {
                    _credentialsHeader = GetCredentialsHeader();
                }
                tr.LogObject(_credentialsHeader);

                var processMessageNodeRequest = GetProcessMessageNodeRequest(_credentialsHeader, message);
                tr.Log("CCM Message Request:");
                tr.LogObject(processMessageNodeRequest);

                try
                {
                    var processMessageNodeResponse = _soapClient.ProcessMessageNode(processMessageNodeRequest);
                    tr.Log("CCM Message Response:");
                    tr.LogObject(processMessageNodeResponse);

                    messageResponse = processMessageNodeResponse?.ProcessMessageNodeResult.Deserialize<MessageResponse>();
                    tr.LogObject(messageResponse);

                    _messageResponse = messageResponse;
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Repositories.SoapRepository.ProcessMessage(string)");
                    throw;
                }

                return messageResponse;
            }
        }

        public MessageResponse AddAccount(AddAccount account, Application app, Applicant applicant)
        {
            MessageResponse messageResponse = null;
            string messageXml = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.AddAccount"))
            {
                try
                {
                    messageXml = account.Message?.SerializeToXmlString();
                    tr.Log($"AddAccount: BEFORE setting host values => {messageXml}");

                    tr.Log("AddAccount: Set Application-level host values ");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        app.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
                        account.Message?.HostValueParentNode
                    );
                    tr.Log($"AddAccount: AFTER Application-level host values => {messageXml}");

                    tr.Log("AddAccount: Set Applicant-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        applicant.HostValues.Where(hv => hv.Field1.StartsWith("AddAccount.")).ToList(),
                        account.Message?.HostValueParentNode
                    );
                    tr.Log($"AddAccount: AFTER Applicant-level host values => {messageXml}");

                    tr.LogObject(CredentialsHeader);

                    tr.Log("AddAccount: Calling ISoapRepository.ProcessMessage");
                    messageResponse = ProcessMessage(messageXml);
                }
                catch (Exception ex)
                {
                    // Handle serialization and host value translation exceptions here

                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Repositories.SoapRepository.AddAccount");
                    throw;
                }
            }

            return messageResponse;
        }

        public MessageResponse AddAccountPartyRelationship(AddAccountPartyRelationship accountPartyRelationship, Application app, Applicant applicant)
        {
            MessageResponse messageResponse = null;
            string messageXml = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.AddAccountPartyRelationship"))
            {
                try
                {
                    messageXml = accountPartyRelationship.Message?.SerializeToXmlString();
                    tr.Log($"AddAccountPartyRelationship: BEFORE setting host values => {messageXml}");

                    tr.Log("AddAccountPartyRelationship: Set Application-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        app.HostValues.Where(hv => hv.Field1.StartsWith("AddAccountPartyRelationship.")).ToList(),
                        accountPartyRelationship.Message?.HostValueParentNode
                    );
                    tr.Log($"AddAccountPartyRelationship: AFTER Application-level host values => {messageXml}");

                    tr.Log("AddAccountPartyRelationship: Set Applicant-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        applicant.HostValues.Where(hv => hv.Field1.StartsWith("AddAccountPartyRelationship.")).ToList(),
                        accountPartyRelationship.Message?.HostValueParentNode
                    );
                    tr.Log($"AddAccountPartyRelationship: AFTER Applicant-level host values => {messageXml}");

                    tr.LogObject(CredentialsHeader);

                    tr.Log("AddAccountPartyRelationship: Calling ISoapRepository.ProcessMessage");
                    messageResponse = ProcessMessage(messageXml);
                }
                catch (Exception ex)
                {
                    // Handle serialization and host value translation exceptions here

                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Repositories.SoapRepository.AddAccountPartyRelationship");
                    throw;
                }
            }

            return messageResponse;
        }

        public MessageResponse AddCard(AddCard card, Application app, LmsPerson lmsPerson)
        {
            MessageResponse messageResponse = null;
            string messageXml = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.AddCard"))
            {
                try
                {
                    messageXml = card.Message?.SerializeToXmlString();
                    tr.Log($"AddCard: BEFORE setting host values => {messageXml}");

                    tr.Log("AddCard: Set Application-level host values ");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        app.HostValues.Where(hv => hv.Field1.StartsWith("AddCard.")).ToList(),
                        card.Message?.HostValueParentNode
                    );
                    tr.Log($"AddCard: AFTER Application-level host values => {messageXml}");

                    if (lmsPerson.Applicant != null)
                    {
                        tr.Log("AddCard: Set Applicant-level host values");
                        messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                            messageXml,
                            lmsPerson.Applicant.HostValues.Where(hv => hv.Field1.StartsWith("AddCard.")).ToList(),
                            card.Message?.HostValueParentNode
                        );
                        tr.Log($"AddCard: AFTER Applicant-level host values => {messageXml}");
                    }
                    else if (lmsPerson.Applicant == null && lmsPerson.AuthorizedUser != null)
                    {
                        tr.Log("AddCard: Set Authorized User-level host values");
                        messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                            messageXml,
                            lmsPerson.AuthorizedUser.HostValues.Where(hv => hv.Field1.StartsWith("AddCard.")).ToList(),
                            card.Message?.HostValueParentNode
                        );
                        tr.Log($"AddCard: AFTER Authorized User-level host values => {messageXml}");
                    }

                    tr.LogObject(CredentialsHeader);

                    tr.Log("AddCard: Calling ISoapRepository.ProcessMessage");
                    messageResponse = ProcessMessage(messageXml);
                }
                catch (Exception ex)
                {
                    // Handle serialization and host value translation exceptions here

                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Repositories.SoapRepository.AddCard");
                    throw;
                }
            }

            return messageResponse;
        }

        public MessageResponse AddOrganization(AddOrganization organization, Application app, Applicant applicant, Address address, Phone phone)
        {
            MessageResponse messageResponse = null;
            string messageXml = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.AddOrganization"))
            {
                try
                {
                    messageXml = organization.Message?.SerializeToXmlString();
                    tr.Log($"AddOrganization: BEFORE setting host values => {messageXml}");

                    tr.Log("AddOrganization: Set Application-level host values ");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        app.HostValues.Where(hv => hv.Field1.StartsWith("AddOrganization.")).ToList(),
                        organization.Message?.HostValueParentNode
                    );
                    tr.Log($"AddOrganization: AFTER Application-level host values => {messageXml}");

                    tr.Log("AddOrganization: Set Applicant-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        applicant.HostValues.Where(hv => hv.Field1.StartsWith("AddOrganization.")).ToList(),
                        organization.Message?.HostValueParentNode
                    );
                    tr.Log($"AddOrganization: AFTER Applicant-level host values => {messageXml}");

                    tr.Log("AddOrganization: Set Address-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        address.HostValues.Where(hv => hv.Field1.StartsWith("AddOrganization.")).ToList(),
                        organization.Message?.HostValueParentNode
                    );
                    tr.Log($"AddOrganization: AFTER Address-level host values => {messageXml}");

                    tr.Log("AddOrganization: Set Phone-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        phone.HostValues.Where(hv => hv.Field1.StartsWith("AddOrganization.")).ToList(),
                        organization.Message?.HostValueParentNode
                    );
                    tr.Log($"AddOrganization: AFTER Phone-level host values => {messageXml}");

                    tr.LogObject(CredentialsHeader);

                    tr.Log("AddOrganization: Calling ISoapRepository.ProcessMessage");
                    messageResponse = ProcessMessage(messageXml);
                }
                catch (Exception ex)
                {
                    // Handle serialization and host value translation exceptions here

                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Repositories.SoapRepository.AddOrganization");
                    throw;
                }
            }

            return messageResponse;
        }

        public MessageResponse AddPerson(AddPerson person, Application app, LmsPerson lmsPerson, Address address, Phone phone)
        {
            MessageResponse messageResponse = null;
            string messageXml = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.AddPerson"))
            {
                try
                {
                    messageXml = person.Message?.SerializeToXmlString();
                    tr.Log($"AddPerson: BEFORE setting host values => {messageXml}");

                    tr.Log("AddPerson: Set Application-level host values ");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        app.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                        person.Message?.HostValueParentNode
                    );
                    tr.Log($"AddPerson: AFTER Application-level host values => {messageXml}");

                    if (lmsPerson.Applicant != null)
                    {
                        tr.Log("AddPerson: Set Applicant-level host values");
                        messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                            messageXml,
                            lmsPerson.Applicant.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                            person.Message?.HostValueParentNode
                        );
                        tr.Log($"AddPerson: AFTER Applicant-level host values => {messageXml}");
                    }
                    else if (lmsPerson.Applicant == null && lmsPerson.AuthorizedUser != null)
                    {
                        tr.Log("AddPerson: Set Authorized User-level host values");
                        messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                            messageXml,
                            lmsPerson.AuthorizedUser.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                            person.Message?.HostValueParentNode
                        );
                        tr.Log($"AddPerson: AFTER Authorized User-level host values => {messageXml}");
                    }

                    tr.Log("AddPerson: Set Address-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        address.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                        person.Message?.HostValueParentNode
                    );
                    tr.Log($"AddPerson: AFTER Address-level host values => {messageXml}");

                    tr.Log("AddPerson: Set Phone-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        phone.HostValues.Where(hv => hv.Field1.StartsWith("AddPerson.")).ToList(),
                        person.Message?.HostValueParentNode
                    );
                    tr.Log($"AddPerson: AFTER Phone-level host values => {messageXml}");

                    tr.Log("AddPerson: Calling ISoapRepository.ProcessMessage");
                    messageResponse = ProcessMessage(messageXml);
                }
                catch (Exception ex)
                {
                    // Handle serialization and host value translation exceptions here

                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Repositories.SoapRepository.AddPerson");
                    throw;
                }
            }

            return messageResponse;
        }

        public MessageResponse UpdateAccount(UpdateAccount account, Application app, Applicant applicant)
        {
            MessageResponse messageResponse = null;
            string messageXml = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.UpdateAccount"))
            {
                try
                {
                    messageXml = account.Message?.SerializeToXmlString();
                    tr.Log($"UpdateAccount: BEFORE setting host values => {messageXml}");

                    tr.Log("UpdateAccount: Set Application-level host values ");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        app.HostValues.Where(hv => hv.Field1.StartsWith("UpdateAccount.")).ToList(),
                        account.Message?.HostValueParentNode
                    );
                    tr.Log($"UpdateAccount: AFTER Application-level host values => {messageXml}");

                    tr.Log("UpdateAccount: Set Applicant-level host values");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        applicant.HostValues.Where(hv => hv.Field1.StartsWith("UpdateAccount.")).ToList(),
                        account.Message?.HostValueParentNode
                    );
                    tr.Log($"UpdateAccount: AFTER Applicant-level host values => {messageXml}");

                    tr.LogObject(CredentialsHeader);

                    tr.Log("UpdateAccount: Calling ISoapRepository.ProcessMessage");
                    messageResponse = ProcessMessage(messageXml);
                }
                catch (Exception ex)
                {
                    // Handle serialization and host value translation exceptions here

                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Repositories.SoapRepository.UpdateAccount");
                    throw;
                }
            }

            return messageResponse;
        }

        public MessageResponse UpdatePerson(UpdatePerson person, Application app)
        {
            MessageResponse messageResponse = null;
            string messageXml = string.Empty;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.UpdateAccount"))
            {
                try
                {
                    messageXml = person.Message?.SerializeToXmlString();
                    tr.Log($"UpdatePerson: BEFORE setting host values => {messageXml}");

                    tr.Log("UpdatePerson: Set Application-level host values ");
                    messageXml = HostValueTranslator.UpdateRequestWithHostValues(
                        messageXml,
                        app.HostValues.Where(hv => hv.Field1.StartsWith("UpdatePerson.")).ToList(),
                        person.Message?.HostValueParentNode
                    );
                    tr.Log($"UpdatePerson: AFTER Application-level host values => {messageXml}");

                    tr.Log("UpdatePerson: Calling ISoapRepository.ProcessMessage");
                    messageResponse = ProcessMessage(messageXml);
                }
                catch (Exception ex)
                {
                    // Handle serialization and host value translation exceptions here

                    tr.LogException(ex);
                    Utility.LogError(ex, "LMS.Connector.CCM.Repositories.SoapRepository.UpdatePerson");
                    throw;
                }
            }

            return messageResponse;
        }

        #endregion

        #region "Helpers"

        public CredentialsHeader GetCredentialsHeader()
        {
            CredentialsHeader credentialsHeader = null;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.GetCredentialsHeader"))
            {
                credentialsHeader = new CredentialsHeader()
                {
                    Username = _credentials.Username,
                    Password = _credentials.Password,
                    Facility = _credentials.Facility,
                    CultureId = "en"
                };
            }

            return credentialsHeader;
        }

        public Credentials GetServiceCredentials()
        {
            Credentials credentials = null;

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.SoapRepository.GetServiceCredentials"))
            {
                tr.Log($"SOLUTION_CODE = {SOLUTION_CODE}, PARAMETER_CODE = {PARAMETER_CODE}");
                tr.Log("Calling ILmsRepository.GetServiceCredentials");
                credentials = _lmsRepository.GetServiceCredentials(SOLUTION_CODE, PARAMETER_CODE);
                tr.LogObject(credentials);
            }

            return credentials;
        }

        public XmlNode GetXmlNodeRequest(string request)
        {
            _xmlDoc = new XmlDocument();

            _xmlDoc.LoadXml(request);

            var newNode = _xmlDoc.DocumentElement;

            return newNode;
        }

        #endregion

        #region "Private methods"

        private void ClearMessageResponse()
        {
            _messageResponse = null;
        }

        #endregion
    }
}
