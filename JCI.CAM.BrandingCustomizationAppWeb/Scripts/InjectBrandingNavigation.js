//-----------------------------------------------------------------------
// <copyright file="InjectBrandingNavigation.js" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


// Register script for MDS if possible
RegisterModuleInit("InjectBrandingNavigation.js", RemoteManager_Inject); //MDS registration
RemoteManager_Inject(); //non MDS run

var cacheTimeout = 1800;
var currentTime;
var timeStamp;
var brandingNavInjected;
//var context = SP.ClientContext.get_current();
//var user = context.get_web().get_currentUser();
var buildBrandingNavigation;

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("InjectBrandingNavigation.js");
}

function RemoteManager_Inject() {    

    var jQuery = JCI_CAM_BrandingCustomizationApp_AppWeb + "/Scripts/jquery-1.9.1.min.js";

     //load jQuery plugin
    loadScript(jQuery, function () {
    injectLinks();
    });
}

function injectLinks() {        
    var $s = jQuery.noConflict();    
    $s(document).ready(function() {
        
        // Get localstorage last updated timestamp values if they exist             
        timeStamp = localStorage.getItem("navTimeStamp");

        // If nothing in localstorage
        if (timeStamp == "" || timeStamp == null) {

            // Key expired - Rebuild secondary navigation here and refresh key expiration
            buildNavigation();

            buildBrandingNavigation = true;

            // Set timestamp for expiration
            currentTime = Math.floor((new Date().getTime()) / 1000);
            localStorage.setItem("navTimeStamp", currentTime);
        }
        else {
            // Check for expiration. If expired, rebuild navigation            
            if (isKeyExpired("navTimeStamp")) {

                // Key expired - Rebuild secondary navigation here and refresh key expiration
                buildNavigation();

                // Set to true and replace static menu code below with results from a data source
                buildBrandingNavigation = true

                // Set timestamp for expiration
                currentTime = Math.floor((new Date().getTime()) / 1000);
                localStorage.setItem("navTimeStamp", currentTime);
            }
            else {

                // First time load
                buildBrandingNavigation = true
            }            
        }
       
        if (buildBrandingNavigation) {
            // Inject navigation bar for Header
            var JCILogoURL = JCI_CAM_BrandingCustomizationApp_AppWeb + "/Styles/Images/JohnsonControlsLogo.png";
            var JCISiteTitleLogoURL = JCI_CAM_BrandingCustomizationApp_AppWeb + "/Styles/Images/JohnsonControlsLogo103.png";
            var headerNavigationHTML = '<div class="OuterMainContainer BorderBottom SteelGray s4-notdlg noindex"><div class="JCIContainer s4-notdlg UtilityNavigation"><div id="TopNavigationCustom"><div id="SignIn"><div data-name="SignIn"></div></div></div><div class="clearfix"></div></div></div>';

            $s('#DeltaSiteLogo a img').attr("src", JCISiteTitleLogoURL);
            $s("div.ms-core-brandingText" ).replaceWith( "<span class='logoText'><span class='cyan'>my</span><span class='blue'>JCI</span></span>" );

            if ($s('#mysite-ribbonrow').length == 0) {
                if (!$s(".OuterMainContainer.BorderBottom.SteelGray.s4-notdlg.noindex").length)
                {
                    $s('#s4-ribbonrow').before(headerNavigationHTML);
                }
                else
                {
                    console.log("Header HTML already exists.");
                }
            }
            else {
                if (!$s('#s4-ribbonrow').hasClass(".OuterMainContainer.BorderBottom.SteelGray.s4-notdlg.noindex")) {
                    $s('#mysite-ribbonrow').before(headerNavigationHTML);
                }
                else {
                    console.log("Header HTML already exists.");
                }
            }

            // Inject navigation bar for footer
            if (!$s("#footer").length) {
                $s('#contentRow').after('<div id="footer" class="s4-notdlg ms-dialogHidden noindex"><div class="JCIContainer"></div></div>');
            }
            else
            {
                console.log("Footer HTML already exists.");
            }

            var layoutsRoot = window.location.protocol + "//" + window.location.host + '/_layouts/15/';
            
            // Executing the js files
            if ($s('#footer .JCIContainer').html() != undefined) {
                if ($s('#footer .JCIContainer').html().length == 0) {
                    $s.ajaxSetup({ cache: true });
                    SP.SOD.executeFunc('SP.js', 'SP.ClientContext', function () {
                        if (typeof SP.UserProfiles == "undefined") {
                            SP.SOD.registerSod('SP.UserProfiles.js', SP.Utilities.Utility.getLayoutsPageUrl('SP.UserProfiles.js'));
                        }
                        SP.SOD.executeFunc('SP.UserProfiles.js', 'SP.UserProfiles', function () {
                            SP.SOD.registerSod('sp.taxonomy.js', SP.Utilities.Utility.getLayoutsPageUrl('sp.taxonomy.js'));
                            SP.SOD.executeFunc('sp.taxonomy.js', false, Function.createDelegate(this, function () {
                                $s.getScript(JCI_CAM_BrandingCustomizationApp_AppWeb + '/scripts/JCIGlobalNavigation.js?rev=1',
                                            JCIGlobalNavigation.ready());
                                $s.getScript(JCI_CAM_BrandingCustomizationApp_AppWeb + '/scripts/JQuery.WaterMark.js?rev=2', function (jd) {
                                    JCIJQueryWaterMark.ready();
                                });

                                $s.getScript(JCI_CAM_BrandingCustomizationApp_AppWeb + '/scripts/JCIMaster.js?rev=3', function (jd) {
                                    JCIMasterJS.ready();
                                });

                                $s.getScript(JCI_CAM_BrandingCustomizationApp_AppWeb + '/scripts/DirectAccess.js?rev=4', function (jd) {
                                    JCIDirectAccessClient.ready();
                                });                                
                            }));
                        });
                    });
                }               
            }
            else
            {
                console.log("Header and footer html not exists.");
            }
        }
    });
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

function buildNavigation() {
    // Data source implementation details here

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
        injectLinks();
    }
}