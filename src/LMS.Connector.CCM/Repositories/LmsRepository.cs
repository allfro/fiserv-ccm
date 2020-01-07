using System;
using System.Data;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.FrameworkSecurity;
using Akcelerant.Core.Lookups;
using LMS.Connector.CCM.Models;

namespace LMS.Connector.CCM.Repositories
{
    public class LmsRepository : ILmsRepository
    {
        private string _userToken;
        private LookupComponent _lookup;
        private Parameters _parameters;

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

        public LookupComponent Lookup
        {
            get
            {
                return _lookup;
            }
            set
            {
                _lookup = value;
            }
        }

        public Parameters Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        public LmsRepository(string userToken)
        {
            _userToken = userToken;
            _lookup = new LookupComponent(userToken);
            _parameters = new Parameters();
        }

        public LmsRepository(string userToken, LookupComponent lookup)
        {
            _userToken = userToken;
            _lookup = lookup;
            _parameters = new Parameters();
        }

        public LmsRepository(string userToken, Parameters parameters)
        {
            _userToken = userToken;
            _lookup = new LookupComponent(userToken);
            _parameters = parameters;
        }

        public LmsRepository(string userToken, LookupComponent lookup, Parameters parameters)
        {
            _userToken = userToken;
            _lookup = lookup;
            _parameters = parameters;
        }

        public string GetUserFullNameById(int userId)
        {
            string loanOfficerName;

            using (var security = new Security())
            {
                loanOfficerName = security.GetUserFullNameById(userId);
            }

            return loanOfficerName;
        }

        public string GetLookupCodeById(int lookupId, LookupTypes lookupType)
        {
            var code = _lookup.GetLookupCodeById(lookupId, lookupType);

            return code;
        }

        public int GetLookupIdByTypeAndCode(LookupTypes lookupType, string lookupCode)
        {
            var lookupId = _lookup.GetLookupIdByTypeAndCode(lookupType, lookupCode);

            return lookupId;
        }

        public string GetLookupDescById(int lookupId, LookupTypes lookupType)
        {
            var lookupDesc = _lookup.GetLookupDescById(lookupId, lookupType);

            return lookupDesc;
        }

        public Credentials GetServiceCredentials(string solutionCode, string serviceUrlParameterCode)
        {
            var credentials = new Credentials();

            using (var tr = new Tracer("LMS.Connector.CCM.Repositories.LmsRepository.GetServiceCredentials"))
            {
                using (var dt = _parameters.GetAllParametersValue(Parameters.Parameter.Solution, solutionCode))
                {
                    tr.Log($"dt.Rows = {dt.Rows.Count}");
                    foreach (DataRow dr in dt.Rows)
                    {
                        var parameterCode = dr["ParameterCode"]?.ToString();

                        if (parameterCode == serviceUrlParameterCode)
                        {
                            credentials.BaseUrl = dr["ParameterValue"].ToString();

                            //Check If URL Last character does not have "/", Append "/"
                            if (!string.IsNullOrWhiteSpace(credentials.BaseUrl) && credentials.BaseUrl[credentials.BaseUrl.Length - 1] != Convert.ToChar("/"))
                                credentials.BaseUrl = credentials.BaseUrl + "/";
                        }
                        else if (parameterCode == "CCM_USERNAME")
                        {
                            credentials.Username = dr["ParameterValue"].ToString();
                        }
                        else if (parameterCode == "CCM_PASSWORD")
                        {
                            credentials.Password = dr["ParameterValue"].ToString();
                        }
                        else if (parameterCode == "CCM_FACILITY")
                        {
                            credentials.Facility = dr["ParameterValue"].ToString();
                        }
                    }
                }

                tr.LogObject(credentials);
            }

            return credentials;
        }

        public string GetPhoneType(int phoneTypeId)
        {
            var phoneTypeCode = GetLookupCodeById(phoneTypeId, LookupTypes.PhoneType);
            string phoneType = string.Empty;

            switch (phoneTypeCode)
            {
                case "HOME":
                    phoneType = "Home";
                    break;
                case "WORK":
                    phoneType = "Business";
                    break;
                case "MOBILE":
                    phoneType = "Mobile";
                    break;
                case "FAX":
                    phoneType = "Fax";
                    break;
                case "VACATION":
                    phoneType = "Other";
                    break;
                case "OTHER":
                    phoneType = "Other";
                    break;
                default:
                    break;
            }

            return phoneType;
        }
    }
}
