using System;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Lending.Data.DTO.Applications;
using LMS.Connector.CCM.Dto.Rest;
using LMS.Connector.CCM.Models;
using LMS.Connector.CCM.Repositories;

namespace LMS.Connector.CCM.Behaviors.Rest
{
    public class TestConnectionBehavior : Behavior, ITestConnection, IRestTestConnectionBehavior
    {
        private string _userToken;
        private Session _session;
        private bool _connectionEstablished;

        public TestConnectionBehavior(string userToken)
        {
            _userToken = userToken;
            _restRepository = new RestRepository(userToken);
        }

        public TestConnectionBehavior(string userToken, IRestRepository repository)
        {
            _userToken = userToken;
            _restRepository = repository;
        }

        public TestConnectionBehavior(string userToken, IRestRepository repository, Credentials credentials)
        {
            _userToken = userToken;
            _restRepository = repository;
            _restRepository.Credentials = credentials;
        }

        #region "Interface implementations"

        public Session Session
        {
            get
            {
                return _session;
            }
            set
            {
                _session = value;
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

        /// <summary>
        /// Not needed for REST implementation.
        /// </summary>
        public Dto.Soap.MessageResponse MessageResponse
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public BaseResult TestConnection(string serviceUrl, string userName, string password, string facility)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Behaviors.Rest.TestConnectionBehavior.TestConnection"))
            {
                if (_restRepository.Credentials == null)
                {
                    _restRepository.Credentials = new Credentials()
                    {
                        BaseUrl = serviceUrl,
                        Username = userName,
                        Password = password,
                        Facility = facility
                    };
                }
                tr.LogObject(_restRepository.Credentials);

                tr.Log($"Session _session null? => {_session == null}");
                if (_session == null)
                {
                    tr.Log("Call GetSession() to get new _session");
                    _session = GetSession();
                }
                tr.LogObject(_session);

                try
                {
                    tr.Log("Calling IRestRepository.TestConnection");
                    _connectionEstablished = _restRepository.TestConnection(_session);
                    tr.Log($"_connectionEstablished = {_connectionEstablished}");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Behaviors.Rest.TestConnectionBehavior.TestConnection");
                    result.AddMessage(MessageType.Error, $"Exception when attempting to call REST Repository TestConnection(): {ex.Message}");
                }
                finally
                {
                    // Deallocate DTO
                    _session = null;
                }

                if (_connectionEstablished)
                {
                    result.Result = true;
                }
                else
                {
                    result.Result = false;
                    result.AddMessage(MessageType.Error, "Connection to CCM REST service was not established");
                }
            }

            return result;
        }

        /// <summary>
        /// Not needed for REST implementation of TestConnectionBehavior.
        /// </summary>
        /// <param name="primaryApplicant"></param>
        /// <returns></returns>
        public Dto.Soap.UpdateAccount GetDto(Applicant primaryApplicant)
        {
            throw new NotImplementedException();
        }

        public Session GetSession()
        {
            var session = new Session()
            {
                UserName = _restRepository.Credentials.Username,
                Password = _restRepository.Credentials.Password,
                Authentication = "CCM"
            };

            return session;
        }

        #endregion
    }

    public interface IRestTestConnectionBehavior
    {
        Session Session { get; set; }

        /// <summary>
        /// Factory method that creates a Session DTO.
        /// </summary>
        /// <returns></returns>
        Session GetSession();
    }
}
