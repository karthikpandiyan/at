// Register script for MDS if possible
// RegisterModuleInit("settings.js", RemoteManager_Inject); //MDS registration

if (document.readyState === "complete") {
    //Already loaded!
    RemoteManager_Inject(); //non MDS run
}
else {
    //Add onload or DOMContentLoaded event listeners 
    // Mozilla, Opera and webkit nightlies currently support this event
    if (document.addEventListener) {
        // Use the handy event callback
        document.addEventListener("DOMContentLoaded", function () { RemoteManager_Inject(); }, false);

        // If IE event model is used
    } else if (document.attachEvent) {
        // ensure firing before onload,
        // maybe late but safe also for iframes
        document.attachEvent("onreadystatechange", function () { RemoteManager_Inject(); });
    }
}

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("settings.js");
}

function RemoteManager_Inject() {

    var jQuery = JCI_CAM_SPDSettingsApp_AppWeb + "/Scripts/jquery-1.9.1.min.js";

    // load jQuery 
    loadScript(jQuery, function () {

        // Customize the viewlsts.aspx page
        if (IsOnPage("settings.aspx")) {
            //hide the subsites link on the viewlsts.aspx page
            $("#ctl00_PlaceHolderMain_SiteCollectionAdmin_RptControls_SharePointDesignerSettings").parent().hide();
        }
    });
}

function IsOnPage(pageName) {
    if (window.location.href.toLowerCase().indexOf(pageName.toLowerCase()) > -1) {
        return true;
    } else {
        return false;
    }
}

function loadScript(url, callback) {
    var head = document.getElementsByTagName("head")[0];
    var script = document.createElement("script");
    script.src = url;

    // Attach handlers for all browsers
    var done = false;
    script.onload = script.onreadystatechange = function () {
        if (!done && (!this.readyState
					|| this.readyState == "loaded"
					|| this.readyState == "complete")) {
            done = true;

            // Continue your code
            callback();

            // Handle memory leak in IE
            script.onload = script.onreadystatechange = null;
            head.removeChild(script);
        }
    };

    head.appendChild(script);
}
