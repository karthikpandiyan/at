// Register script for MDS if possible
RegisterModuleInit("TestSiteBannerInjection.js", RemoteManager_Inject); //MDS registration
RemoteManager_Inject(); //non MDS run

var cacheTimeout = 1800;
var currentTime;
var timeStamp;
var buildTestSiteBanner;

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("TestSiteBannerInjection.js");
}

function RemoteManager_Inject() {    

    var jQuery = JCI_CAM_TestSiteBannerApp_AppWeb +  "/scripts/jquery-1.9.1.min.js";

    //load jQuery plugin
    loadScript(jQuery, function () {
        injectTestSiteBanner();
    });
}

function injectTestSiteBanner() {
    var $s = jQuery.noConflict();
    $s(document).ready(function () {

        // Get localstorage last updated timestamp values if they exist             
        timeStamp = localStorage.getItem("testSiteBannerTimeStamp");

        // If nothing in localstorage
        if (timeStamp == "" || timeStamp == null) {

            buildTestSiteBanner = true;

            // Set timestamp for expiration
            currentTime = Math.floor((new Date().getTime()) / 1000);
            localStorage.setItem("testSiteBannerTimeStamp", currentTime);
        }
        else {
            // Check for expiration. If expired, rebuild test site banner            
            if (isKeyExpired("testSiteBannerTimeStamp")) {

                // Set to true and replace static menu code below with results from a data source
                buildTestSiteBanner = true

                // Set timestamp for expiration
                currentTime = Math.floor((new Date().getTime()) / 1000);
                localStorage.setItem("testSiteBannerTimeStamp", currentTime);
            }
            else {

                // First time load
                buildTestSiteBanner = true
            }
        }

        if (buildTestSiteBanner) {
            // Inject banner HTML for test site 
            var testBannerHTML = '<div style="height:35px;">' +
                                       '<div style="position:absolute; z-index:0; width:100%; min-width:600px;">' +
                                       '<div style="width: 33%; background-color:#08338f; height:35px; float:left;"></div>' +
                                       '<div style="width: 34%; background-color:#00c8ff; height:35px; float:left;"></div>' +
                                       '<div style="width: 33%; background-color:#5fc12f; height:35px; float:left;"></div>' +
                                       '</div>' +
                                       '<div id="messageText"  style="position:absolute; z-index:1; width:100%; height:35px; text-align:center; min-width:600px;"></div>' +
                                       '</div>';

            if (!$s("#messageText").length) {
                if ($s('#suiteBarDelta').length) {
                    $s('#suiteBarDelta').after(testBannerHTML);
                    banner();
                }
                else {
                    if ($s('#suiteBar').length) {
                        $s('#suiteBar').after(testBannerHTML);
                        banner();
                    }
                }
            }
            else {
                console.log("Test site banner HTML already exists.");
            }            
        }
    });
}

function banner()
{
    var url = document.URL;
    if (url.toLowerCase().indexOf("sdlg=1") > 0) {
        document.getElementById("messageText").innerHTML = "<span style=\"font-size:17pt;  color:#fff;\">This is a test site. Please use it for test purposes only.</span>";
    } else {
        document.getElementById("messageText").innerHTML = "<span style=\"font-size:20pt;  color:#fff;\">This is a test site. Please use it for test purposes only.</span>";
    }
}

// Check to see if the key has expired
function isKeyExpired(timeStampKey) {

    // Retrieve the example setting for expiration in seconds
    var expiryStamp = localStorage.getItem(timeStampKey);

    if (expiryStamp != null && cacheTimeout != null) {

        // Retrieve the timestamp and compare against specified cache timeout settings to see if it is expired
        var currentTime = Math.floor((new Date().getTime()) / 1000);

        if (currentTime - parseInt(expiryStamp) > parseInt(cacheTimeout)) {
            return true; //Expired
        }
        else {
            return false;
        }
    }
    else {
        //default 
        return true;
    }
}

function loadScript(url, callback) {
    if (typeof jQuery == "undefined") {
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
    else {
        injectTestSiteBanner();
    }
}
