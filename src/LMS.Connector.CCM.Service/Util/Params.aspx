<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Params.aspx.cs" Inherits="LMS.Connector.CCM.Service.Util.Params" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>CCM Service Parameters</title>
    <style type="text/css">
        .text { font-family:Tahoma; font-size:11pt }
        .param-label { padding-top:20px }
        .param-value { padding-top:20px }
        .param-desc { padding-left:30px; font-size:10pt }
    </style>
</head>
<body class="text">
    <form id="form1" runat="server">
        <fieldset>
            <legend>CCM Service Parameters</legend>
            <table cellpadding="5px" cellspacing="0px" border="0px">
                <tr>
                    <td class="param-label">Tracing Enabled:</td>
                    <td class="param-value">
                        Yes <asp:RadioButton ID="TracingEnabledYes" GroupName="TracingEnabled" runat="server" />
                        No <asp:RadioButton ID="TracingEnabledNo" GroupName="TracingEnabled" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="param-label">Logging Config File:</td>
                    <td class="param-value">
                        <asp:TextBox ID="TracingConfigFile" Width="400px" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="param-label">Default Formatter:</td>
                    <td class="param-value">
                        <asp:DropDownList ID="TracingFormatter" runat="server">
                            <asp:ListItem Value="PipeDelimited" Text="Pipe Delimited"></asp:ListItem>
                            <asp:ListItem Value="ServiceLog" Text="Service Log"></asp:ListItem>
                            <asp:ListItem Value="Xml" Text="Xml"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
            <br />
            <asp:Button runat="server" Text="Submit" ID="SubmitButton" />
        </fieldset>
    </form>
</body>
</html>