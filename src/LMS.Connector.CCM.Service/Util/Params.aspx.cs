using System;
using System.Web;
using System.Web.UI;
using Akcelerant.Core.Client.Tracing;

namespace LMS.Connector.CCM.Service.Util
{
    public partial class Params : Page
    {
        protected void Page_PreInit(object sender, System.EventArgs e)
        {
            if (!HttpContext.Current.Request.IsLocal)
            {
                Response.Clear();
                Response.StatusCode = 401;
                Response.StatusDescription = "Sorry! You are not authorized to access this page.";
                Response.End();
                return;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                //update the common tracing params
                Config.IsEnabled = this.TracingEnabledYes.Checked;
                Config.ConfigFile = this.TracingConfigFile.Text;
                Config.SetDefaultFormatter(this.TracingFormatter.Text);
            }
            else
            {
                //load the form
                if (Config.IsEnabled)
                    this.TracingEnabledYes.Checked = true;
                else
                    this.TracingEnabledNo.Checked = true;

                this.TracingConfigFile.Text = Config.ConfigFile;
                this.TracingFormatter.Text = Config.DefaultFormatterName;
            }
        }
    }
}