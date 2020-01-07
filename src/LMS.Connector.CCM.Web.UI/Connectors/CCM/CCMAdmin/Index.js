function testConnection() {
    var form = Global.getComponent("CCMConnectorForm_FormPanel");

    Global.mask("Testing Connections...");

    var model = {};
    model.UserName = form.CCMConnectorCenterPanel.username.getValue();
    model.Password = form.CCMConnectorCenterPanel.password.getValue();
    model.Facility = form.CCMConnectorCenterPanel.facility.getValue();
    model.SOAPServiceUrl = form.CCMConnectorCenterPanel.soapServiceUrl.getValue();
    model.RESTServiceUrl = form.CCMConnectorCenterPanel.restServiceUrl.getValue();

    var siteRoot = form.CCMConnectorCenterPanel.siteRoot.getValue();

    var errorMessage = "";
    if (model.UserName == undefined || model.UserName == "")
        errorMessage = errorMessage + "Username is a required field.<br />";
    if (model.Password == undefined || model.Password == "")
        errorMessage = errorMessage + "Password is a required field.<br />";

    if (errorMessage.length > 0) {
        Global.alertError(errorMessage);
        Global.unmask();

        return;
    }

    /* Call controller to check connectivity and then handle callback */
    var postUrl = siteRoot + "Connectors/CCM/CCMAdmin.mvc/TestConnections/";
    $.ajax({
        url: postUrl,
        type: "POST",
        data: JSON.stringify(model),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            testConnectionCallback(response);
        },
        error: function () {
            document.getElementById("testConnectionSuccessful").value = "false";
            Global.alertError("The process timed out. Please try again.");
        }
    });
}

function testConnectionCallback(response, transaction) {
    if (!Global.parseAjaxResult(response, transaction)) {
        return;
    }

    if (response) {
        if (response.Result) {
            document.getElementById("testConnectionSuccessful").value = "true";
            Global.alertSuccess(response.Messages[0].Text);
        }
        else {
            document.getElementById("testConnectionSuccessful").value = "false";
            Global.alertError(response.Messages[0].Text, response.ExceptionId);
        }
    }
    else {
        document.getElementById("testConnectionSuccessful").value = "false";
        Global.alertError("The process timed out. Please try again.");
    }

    Global.unmask();
}

function saveModel() {
    var form = Global.getComponent("CCMConnectorForm_FormPanel");
    CCMConnectorForm.doAction("SAVE");
}

function validate() {
    var obj = new Validation();
    var form = Global.getComponent("CCMConnectorForm_FormPanel");
    var isActiveCheckbox = form.CCMConnectorCenterPanel.cbActive.getValue();

    if (isActiveCheckbox == true && document.getElementById("testConnectionSuccessful").value == "false") {
        obj.addMessage("The connection must be tested successfully before saving.");
    }

    var retval = (obj.messages.length > 0) ? obj : true;

    return retval;
}

function resetConnectionTest() {
    /* This function will set the testConnectionSuccessful value to false when a Username, Password,
     * Facility, SOAP Service URL, or REST Service Url value is changed. */
    document.getElementById("testConnectionSuccessful").value = "false";
}