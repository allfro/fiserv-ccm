using System;
using System.Collections.Generic;
using System.Linq;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Soap;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors.Soap
{
    public class TestConnectionBehavior : Behavior, ITestConnection, ISoapTestConnectionBehavior
    {
        private const string TRACE_NUMBER = "123456";
        private const string PARTY_NUMBER = "99999999";
        private const string TAX_ID_NUMBER = "999999999";
        private const string ADDRESS_LINE_1 = "123 Oak Court";
        private const string ADDRESS_FIELD = "AddressLine1";

        private Application _app;
        private string _userToken;
        private UpdatePerson _person;
        private MessageResponse _messageResponse;
        private bool _connectionEstablished;

        public UpdatePerson Person
        {
            get
            {
                return _person;
            }
            set
            {
                _person = value;
            }
        }

        public TestConnectionBehavior(Application app, string userToken)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = new SoapRepository(userToken);
        }

        public TestConnectionBehavior(Application app, string userToken, ISoapRepository serviceRepository)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
        }

        public TestConnectionBehavior(Application app, string userToken, ISoapRepository serviceRepository, Credentials credentials)
        {
            _app = app;
            _userToken = userToken;
            _soapRepository = serviceRepository;
            _soapRepository.Credentials = credentials;
        }

        #region "Interface implementations"

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

        public bool ConnectionEstablished
        {
            get
            {
                return _connectionEstablished;
            }
            set
            {
                _connectionEstablished = value;
            }
        }

        public BaseResult TestConnection(string serviceUrl, string userName, string password, string facility)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Soap.TestConnectionBehavior.TestConnection"))
            {

                _soapRepository.Credentials = new Credentials()
                {
                    BaseUrl = serviceUrl,
                    Username = userName,
                    Password = password,
                    Facility = facility
                };

                tr.LogObject(_soapRepository.Credentials);

                // Can be any primary Applicant -- doesn't matter since we are only testing a connection.
                var lmsPerson = new LmsPerson()
                {
                    Applicant = _app.Applicants.SingleOrDefault(a => a.ApplicantTypeId == (int)Akcelerant.Lending.Lookups.Constants.Values.ApplicantType.Primary)
                };

                tr.Log($"UpdatePerson _person null? => {_person == null}");
                if (_person == null)
                {
                    tr.Log("Call GetDto() to get new _person");
                    _person = GetDto(lmsPerson);
                }
                tr.LogObject(_person);

                try
                {
                    tr.Log("Calling ISoapRespository.UpdatePerson");
                    _messageResponse = _soapRepository.UpdatePerson(_person, _app);

                    tr.Log($"_messageResponse.ResponseCode = {_messageResponse?.ResponseCode}");
                    tr.Log($"_messageResponse.ErrorMessage = {_messageResponse?.ErrorMessage}");

                    var isSystemMalfunction = (_messageResponse?.ResponseCode.Equals("SystemMalfunction", StringComparison.InvariantCulture) == true) ? true : false;
                    tr.Log($"isSystemMalfunction = {isSystemMalfunction}");

                    var isErrorMessageModifyPartyRequestFailed = (_messageResponse?.ErrorMessage.Contains($"Modify Party request failed. Party {_person.Message?.DataUpdate?.Person?.PartyNumber} not found.") == true) ? true : false;
                    tr.Log($"isErrorMessageModifyPartyRequestFailed = {isErrorMessageModifyPartyRequestFailed}");

                    //Use reponseCode and errorMessage to derive connectionEstablished according to business rules
                    _connectionEstablished = (isSystemMalfunction && isErrorMessageModifyPartyRequestFailed) ? true : false;
                    tr.Log($"_connectionEstablished = {_connectionEstablished}");
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Soap.TestConnectionBehavior.TestConnection");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to get a MessageResponse from SOAP Repository UpdateAccount(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTO
                    _person = null;
                }

                if (_connectionEstablished)
                {
                    result.Result = true;
                }
                else
                {
                    result.Result = false;
                    result.AddMessage(MessageType.Error, "Connection to CCM SOAP service was not established");
                }
            }

            return result;
        }

        public UpdatePerson GetDto(LmsPerson lmsPerson)
        {
            var person = new UpdatePerson()
            {
                Message = GetMessage(lmsPerson)
            };

            return person;
        }

        #endregion

        #region "DTO Builder Methods"

        private Dto.Soap.Message GetMessage(LmsPerson lmsPerson)
        {
            var message = new Dto.Soap.Message()
            {
                DataUpdate = GetDataUpdate(lmsPerson)
            };

            return message;
        }

        private DataUpdate GetDataUpdate(LmsPerson lmsPerson)
        {
            var dataUpdate = new DataUpdate()
            {
                TraceNumber = TRACE_NUMBER,
                ProcessingCode = "ExternalUpdateRequest",
                Source = "LoanOrigination",
                UpdateAction = "Modify",
                Person = GetPerson(lmsPerson),
                ModifiedFields = GetModifiedFields(lmsPerson)
            };

            return dataUpdate;
        }

        private Person GetPerson(LmsPerson lmsPerson)
        {
            var person = new Person()
            {
                PrimaryAddress = GetPrimaryAddress(lmsPerson)
            };

            person.PartyNumber = PARTY_NUMBER;
            person.TaxIdNumber = TAX_ID_NUMBER;

            return person;
        }

        private PrimaryAddress GetPrimaryAddress(LmsPerson lmsPerson)
        {

            var primaryAddress = new PrimaryAddress()
            {
                AddressLine1 = ADDRESS_LINE_1
            };

            return primaryAddress;
        }

        /// <summary>
        /// Manually creates a collection of ModifieldFields.
        /// </summary>
        /// <param name="lmsPerson"></param>
        /// <remarks>
        /// Optionally giving it the ability to access applicant-level host values.
        /// </remarks>
        /// <returns></returns>
        public List<ModifiedFields> GetModifiedFields(LmsPerson lmsPerson)
        {
            List<ModifiedFields> modifiedFieldsList = new List<ModifiedFields>();

            ModifiedFields modifiedFields = new ModifiedFields()
            {
                AddressField = new List<string>()
            };
            modifiedFields = GetAddressField(modifiedFields, lmsPerson);

            modifiedFieldsList.Add(modifiedFields);

            return modifiedFieldsList;
        }

        /// <summary>
        /// Manually creates a collection of AddressFields that will be added as a property of ModifiedFields.
        /// </summary>
        /// <param name="modifiedFields"></param>
        /// <param name="lmsPerson"></param>
        /// <remarks>
        /// This method already has access to application-level host values.
        /// Optionally giving it the ability to access applicant-level or authorized user-level host values.
        /// </remarks>
        /// <returns></returns>
        public static ModifiedFields GetAddressField(ModifiedFields modifiedFields, LmsPerson lmsPerson)
        {
            modifiedFields.AddressField.Add(ADDRESS_FIELD);

            return modifiedFields;
        }

        #endregion
    }

    public interface ISoapTestConnectionBehavior
    {
        /// <summary>
        /// Factory method that creates an UpdatePerson DTO.
        /// </summary>
        /// <param name="lmsPerson"></param>
        /// <remarks>Used only for SOAP implementation of TestConnectionBehavior</remarks>
        /// <returns></returns>
        UpdatePerson GetDto(LmsPerson lmsPerson);
    }
}
