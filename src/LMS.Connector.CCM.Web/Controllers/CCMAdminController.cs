using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Web.Mvc;
using Akcelerant.Core;
using Akcelerant.Core.Client.Tracing;
using Akcelerant.Core.Data.DAL;
using Akcelerant.Core.Data.DTO.Host;
using Akcelerant.Core.Data.DTO.Result;
using Akcelerant.Core.Web;
using LMS.Connector.CCM.Web.Models;

namespace LMS.Connector.CCM.Web.Controllers
{
    public class CCMAdminController : BaseController
    {
        private static string _solutionCode = "CCM_ORIGINATION";
        private static string _paramEnumName = "SOLUTION";
        private const int DISBURSEMENT_PROVIDER_ID = 17;

        public CCMAdminController()
        {
            SecurityFunctionCode = "CONNECTORS";
        }

        protected override void BindDataToGrid(ref Akcelerant.Common.WebControls.DataControls.Grids.Grid oGrid, string Filter = "")
        {

        }

        /// <summary>
        /// Provides a CCMAdminModel to the view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            CCMAdminModel model;

            using (var tr = new Tracer("LMS.Connector.CCM.Web.CCMAdminController.Index"))
            {
                model = new CCMAdminModel(_solutionCode, _paramEnumName);
                tr.LogObject(model);
            }

            return View(model);
        }

        /// <summary>
        /// Saves the form values from the view to the store.
        /// </summary>
        /// <param name="formValues"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Index(FormCollection formValues)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Web.CCMAdminController.Index(FormCollection)"))
            {
                tr.LogObject(formValues);

                var model = new CCMAdminModel(_solutionCode, _paramEnumName);

                TryUpdateModel(model);

                var validationResult = ValidateParameters(model);

                if (!string.IsNullOrWhiteSpace(validationResult))
                {
                    result.Result = false;
                    result.AddMessage(MessageType.Error, validationResult);

                    return result.ToJsonResult();
                }

                if (formValues["IsActive"] == "on")
                {
                    model.IsActive = true;
                }

                tr.LogObject(model);

                var parameters = new Parameters();

                try
                {
                    parameters.SetActiveFlag(Parameters.Parameter.Solution, _solutionCode, model.IsActive);
                    parameters.SetParameterValue(Parameters.Parameter.Solution, _solutionCode, "CCM_USERNAME", model.UserName.Trim());
                    parameters.SetParameterValue(Parameters.Parameter.Solution, _solutionCode, "CCM_PASSWORD", model.Password.Trim());
                    parameters.SetParameterValue(Parameters.Parameter.Solution, _solutionCode, "CCM_FACILITY", model.Facility.Trim());
                    parameters.SetParameterValue(Parameters.Parameter.Solution, _solutionCode, "CCM_SOAP_SERVICE_URL", model.SoapServiceUrl.Trim());
                    parameters.SetParameterValue(Parameters.Parameter.Solution, _solutionCode, "CCM_REST_SERVICE_URL", model.RestServiceUrl.Trim());
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Web.Controllers.CCMAdminController.Index(FormCollection)");
                    result.AddMessage(MessageType.Error, $"There was a problem saving the connection parameters: {ex.Message}");

                    return result.ToJsonResult();
                }

                result.Result = true;
                tr.LogObject(result);
            }

            return result.ToJsonResult();
        }

        /// <summary>
        /// Checks the connectivity to CCM SOAP and REST services.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult TestConnections(CCMAdminModel model)
        {
            var result = new BaseResult();

            using (var tr = new Tracer("LMS.Connector.CCM.Web.CCMAdminController.TestConnections(CCMAdminModel)"))
            {
                var validationResult = ValidateParameters(model);

                if (!string.IsNullOrWhiteSpace(validationResult))
                {
                    result.Result = false;
                    result.AddMessage(MessageType.Error, validationResult);

                    return result.ToJsonResult();
                }

                var connectorParams = new List<HostParameter>()
                {
                    new HostParameter("CCM_USERNAME", model.UserName.Trim()),
                    new HostParameter("CCM_PASSWORD", model.Password.Trim()),
                    new HostParameter("CCM_FACILITY", model.Facility.Trim()),
                    new HostParameter("CCM_SOAP_SERVICE_URL", model.SoapServiceUrl.Trim()),
                    new HostParameter("CCM_REST_SERVICE_URL", model.RestServiceUrl.Trim()),
                    new HostParameter("USERTOKEN", WebUtil.UserToken)
                };

                tr.LogObject(connectorParams);

                try
                {
                    using (var svc = new Akcelerant.Lending.Client.Services.DisburseApplication())
                    {
                        svc.SetEndpointAddress(GetDisbursementProviderServiceLocation(DISBURSEMENT_PROVIDER_ID));

                        result = svc.TestConnection(connectorParams);

                        if (result.Result)
                        {
                            result.Messages.Add(new Akcelerant.Core.Data.DTO.Result.Message(MessageType.Success, "Connection to CCM was successful."));
                            tr.LogObject(result);
                        }
                        else
                        {
                            result.Result = false;
                            result.Messages.Clear();
                            result.AddError("Connection to the Credit Card Management - Loan Origination service failed. Please validate your credentials and try again.");
                        }
                    }
                }
                catch (FaultException<ExceptionDetail> ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Web.Controllers.CCMAdminController.TestConnection");
                    result.AddError("Connection to the Credit Card Management - Loan Origination service failed. Please validate your credentials and try again.");
                }
                catch (Exception ex)
                {
                    tr.LogException(ex);
                    result.Result = false;
                    result.ExceptionId = Utility.LogError(ex, "LMS.Connector.CCM.Web.Controllers.CCMAdminController.TestConnection");
                    result.AddError("Connection to the Credit Card Management - Loan Origination service failed. Please validate your credentials and try again.");
                }
            }

            return result.ToJsonResult();
        }

        /// <summary>
        /// Server-side validation for test connection call
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ValidateParameters(CCMAdminModel model)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(model.UserName))
                errors.AppendLine("Username is a required field.");
            if (string.IsNullOrWhiteSpace(model.Password))
                errors.AppendLine("Password is a required field.");

            return errors.ToString();
        }

        /// <summary>
        /// Gets the service location (URI) of a given disbursement provider id.
        /// </summary>
        /// <param name="disbursementProviderId"></param>
        /// <returns></returns>
        private string GetDisbursementProviderServiceLocation(int disbursementProviderId)
        {
            string disbursementProviderServiceLocation = string.Empty;

            using (var dal = new SQL())
            {
                try
                {
                    dal.Parameters.Clear();
                    dal.Parameters.AddWithValue("@DisbursementProviderId", disbursementProviderId);
                    dal.Execute("SELECT ServiceLocation FROM Lending.DisbursementProvider WHERE DisbursementProviderId = @DisbursementProviderId", ref disbursementProviderServiceLocation);
                }
                catch (Exception ex)
                {
                    Utility.LogError(ex, "LMS.Connector.CCM.Web.CCMAdminController.GetDisbursementProviderServiceLocation");
                }
            }

            return disbursementProviderServiceLocation;
        }
    }
}