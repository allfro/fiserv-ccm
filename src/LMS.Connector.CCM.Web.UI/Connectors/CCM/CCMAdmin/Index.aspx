<%@ Page Language="VB" Inherits="Akcelerant.Core.Web.EditBaseView" %>

<%@ Import Namespace="Akcelerant.Common.Web.FileIncluder" %>
<script runat="server">
    Protected Overrides Sub InitView()
        MyBase.InitView()
        MyBase.Includer.IncludeJs("Resources/jQuery/jquery.min.js")
        MyBase.Includer.IncludeJs("Connectors/CCM/CCMAdmin/Index.js")
    End Sub
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="display: none">
            <%=Html.Hidden("testConnectionSuccessful", "false")%>
        </div>
        <div>
            <%
                Using CCMConnectorForm As New Akcelerant.Common.WebControls.Forms.Form
                    With CCMConnectorForm
                        .ID = "CCMConnectorForm"
                        .Title = "Fiserv Credit Card Management - Loan Orgination"
                        .ItemName = "connector"
                        .OnValidate = "validate"
                        With .Toolbar
                            .UseDefaultButtons = False
                            Dim permission As Akcelerant.Framework.Common.Enums.PermissionLevel
                            Using Sec As New Akcelerant.Core.FrameworkSecurity.Security()
                                permission = Sec.GetPermission(WebUtil.UserToken, "CONNECTORS")
                            End Using
                            With .AddButton("Save", "saveModel", "icon-button-save", "Save", String.Format("Save this {0}", {CCMConnectorForm.ItemName}), String.Format("{0}_SaveButton", {CCMConnectorForm.ID}))
                                If permission < Akcelerant.Framework.Common.Enums.PermissionLevel.Change Then
                                    .Disabled = True
                                End If
                            End With
                            .AddButton(Akcelerant.Common.WebControls.Forms.FormToolbar.ButtonType.Cancel)
                            .AddButton("Test Connection", "testConnection", "icon-button-testconnection", "", "Test connection to CCM")
                        End With
                        With .CenterRegion
                            .Ref = "CCMConnectorCenterPanel"
                            With .AddFieldSet("Activate Service")
                                .LabelWidth = 0
                                .Instructions = "Check the Activate Service box to activate the Credit Card Management – Loan Origination (CCM) connector. An agreement with Fiserv must be in place prior to activating this service. For more information, contact a Temenos Customer Care Specialist."
                                Dim cb As New Akcelerant.Common.WebControls.Forms.Control("IsActive", "../../cbActive", "", Model.IsActive, Forms.Control.ControlType.CheckBox)
                                cb.BoxLabel = "Activate Service"
                                .AddControl(cb)
                            End With
                            With .AddFieldSet("Connection Parameters")
                                .LabelWidth = 200

                                With .AddControl(New Akcelerant.Common.WebControls.Forms.Control(Akcelerant.Common.WebControls.Forms.Control.ControlType.TextField))
                                    .Label = "Username"
                                    .Ref = "../../username"
                                    .ItemId = "username"
                                    .Value = Model.UserName
                                    .Validate.AllowBlank = False
                                    .OnChange = "resetConnectionTest"
                                End With

                                With .AddControl(New Akcelerant.Common.WebControls.Forms.Control(Akcelerant.Common.WebControls.Forms.Control.ControlType.Password))
                                    .Label = "Password"
                                    .Ref = "../../password"
                                    .ItemId = "password"
                                    .Value = Model.Password
                                    .Validate.AllowBlank = False
                                    .OnChange = "resetConnectionTest"
                                End With

                                With .AddControl(New Akcelerant.Common.WebControls.Forms.Control(Akcelerant.Common.WebControls.Forms.Control.ControlType.TextField))
                                    .Label = "Facility"
                                    .Ref = "../../facility"
                                    .ItemId = "facility"
                                    .Value = Model.Facility
                                    .Validate.AllowBlank = True
                                    .OnChange = "resetConnectionTest"
                                End With

                                With .AddControl(New Akcelerant.Common.WebControls.Forms.Control(Akcelerant.Common.WebControls.Forms.Control.ControlType.TextField))
                                    .Label = "Service URL for SOAP"
                                    .Ref = "../../soapServiceUrl"
                                    .ItemId = "soapServiceUrl"
                                    .Value = Model.SOAPServiceURL
                                    .Validate.AllowBlank = True
                                    .OnChange = "resetConnectionTest"
                                End With

                                With .AddControl(New Akcelerant.Common.WebControls.Forms.Control(Akcelerant.Common.WebControls.Forms.Control.ControlType.TextField))
                                    .Label = "Service URL for REST"
                                    .Ref = "../../restServiceUrl"
                                    .ItemId = "restServiceUrl"
                                    .Value = Model.RESTServiceUrl
                                    .Validate.AllowBlank = True
                                    .OnChange = "resetConnectionTest"
                                End With

                                With .AddControl(New Akcelerant.Common.WebControls.Forms.Control(Akcelerant.Common.WebControls.Forms.Control.ControlType.TextField))
                                    .Hidden = True
                                    .Ref = "../../siteRoot"
                                    .ItemId = "siteRoot"
                                    .Value = Model.SiteRoot
                                End With
                            End With
                        End With
                        Response.Write(.ToString)
                    End With
                End Using
            %>
        </div>
    </form>
</body>
</html>
