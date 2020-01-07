using System.ComponentModel.DataAnnotations;
using System.Data;
using Akcelerant.Core;
using Akcelerant.Core.Web;

namespace LMS.Connector.CCM.Web.Models
{
    public class CCMAdminModel
    {
        private string _solutionCode;
        private Parameters _parameters;

        public bool IsActive { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string Facility { get; set; }

        public string SoapServiceUrl { get; set; }

        public string RestServiceUrl { get; set; }

        public string SiteRoot { get { return WebUtil.AppVars.SiteRoot; } }

        public CCMAdminModel()
        {
            _solutionCode = "CCM_ORIGINATION";
            _parameters = new Parameters();
            IsActive = _parameters.GetActiveFlag(Parameters.Parameter.Solution, _solutionCode);

            SetSolutionParameterValues();
        }

        public CCMAdminModel(string solutionCode)
        {
            _solutionCode = solutionCode;
            _parameters = new Parameters();
            IsActive = _parameters.GetActiveFlag(Parameters.Parameter.Solution, solutionCode);

            SetSolutionParameterValues();
        }

        public CCMAdminModel(string solutionCode, string parameterEnumName)
        {
            _solutionCode = solutionCode;
            _parameters = new Parameters();
            var parameterEnum = _parameters.GetParameterEnum(parameterEnumName);
            IsActive = _parameters.GetActiveFlag(parameterEnum, solutionCode);

            SetSolutionParameterValues();
        }

        /// <summary>
        /// Gets the CCM Origination solution parameters and sets the appropriate model properties.
        /// </summary>
        private void SetSolutionParameterValues()
        {
            using (var dt = _parameters.GetAllParametersValue(Parameters.Parameter.Solution, _solutionCode))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    switch (dr["ParameterCode"].ToString())
                    {
                        case "CCM_USERNAME":
                            UserName = dr["ParameterValue"].ToString();
                            break;
                        case "CCM_PASSWORD":
                            Password = dr["ParameterValue"].ToString();
                            break;
                        case "CCM_FACILITY":
                            Facility = dr["ParameterValue"].ToString();
                            break;
                        case "CCM_SOAP_SERVICE_URL":
                            SoapServiceUrl = dr["ParameterValue"].ToString();
                            break;
                        case "CCM_REST_SERVICE_URL":
                            RestServiceUrl = dr["ParameterValue"].ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
