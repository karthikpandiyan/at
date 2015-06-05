/*
Copyright (c) 2013 Webtrends, Inc.
SharePoint 2013 Loader v3.0.23
*/

try
{
    window.wt_sp_globals.loadCount = 0;
}
catch(ex)
{
    console.log(ex.message);
}
window.webtrendsAsyncInit = function () {

    if (window.wt_sp_globals == undefined) {
        window.wt_sp_globals = {
            title: window.location.protocol + "//" + window.location.host + _spPageContextInfo.siteServerRelativeUrl
        };
    }

    var dcs = new Webtrends.dcs().init({
        dcsid: "dcskz4se500000omx3bygmjfm_8r7h",
        timezone: -5,
        plugins: {
            sp: { src: JCI_CAM_BrandingCustomizationApp_AppWeb + "/Scripts/webtrends.sp.js" },
            hm: { src: "//s.webtrends.com/js/webtrends.hm.js" }
        }
    });

    try {
        window.wt_sp_globals.loadCount++;
    }
    catch (ex) {
        console.log(ex.message);
    }

    sendWTPageEvent();

};

function loadScript() {
    
    if (window.wt_sp_globals == undefined) {
        window.wt_sp_globals = {
            title: window.location.protocol + "//" + window.location.host + _spPageContextInfo.siteServerRelativeUrl
        };
    }

    var s = document.createElement("script");
    s.async = true;
    s.src = JCI_CAM_BrandingCustomizationApp_AppWeb + "/Scripts/webtrends.js";
    var s2 = document.getElementsByTagName("script")[0];
    s2.parentNode.insertBefore(s, s2);

}

/* Two different navigation techniques have been observed:
	1 - Clicking a link to a new page does not result in a page transition
	
		In this case, the URL takes the form:
		<site collection url>/_layouts/15/start.aspx#//<actual page url>
	
		The page is ready when the DOMContentLoaded type of event is Raised.
	
	2 - Clicking a link does result in a page transition.
	
		In this case, the URL takes the form:
		<site collection url>/<actual page url>

	It appears that technique 2 only occurs if the User Info Collector is installed but
	that has not been confirmed.		
*/
//Handle navigation technique 2
if (window.location.href.indexOf("start.aspx") < 0) {
    loadScript();
    try{
        window.wt_sp_globals.loadCount++;
    }
    catch (ex) {
        console.log(ex.message);
    }

    sendWTPageEvent();
}
else {
    //Handle navigation technique 1
    ExecuteOrDelayUntilScriptLoaded(
		function () {
		    //load webtrends.js
		    loadScript();
		    //Hook into event to determine when page is ready
		    var origRaiseFakeEvent = RaiseFakeEvent;
		    window.RaiseFakeEvent = function (e, i, j) {
		        if (e == "DOMContentLoaded") {
		            try {
		                window.wt_sp_globals.loadCount++;
		            }
		            catch (ex) {
		                console.log(ex.message);
		            }

		            sendWTPageEvent();
		        }

		        origRaiseFakeEvent(e, i, j);
		    }
		},
		"start.js"
	);
}

/*
* Send the page view event
* Manually set up the page specific parameters 
*/
function sendWTPageEvent() {

    //only send an event after webtrendsAsyncInit has been called
    //and the page is ready. Each of these events will increment loadcount 
    try {
        if (window.wt_sp_globals.loadCount >= 2) {
            Webtrends.multiTrack({
                transform: function (dcs, options) {
                    var newPageInfo = parsePageInfo();

                    //Need to modify DCS directly so the values stick
                    dcs["WT"]["ti"] = newPageInfo["WT.ti"];
                    dcs["DCS"]["dcsuri"] = newPageInfo["dcsuri"];
                    dcs["DCS"]["dcsqry"] = newPageInfo["dcsqry"];
                    dcs["DCS"]["dcssip"] = newPageInfo["dcssip"];
                    dcs["WT"]["es"] = newPageInfo["dcssip"] + newPageInfo["dcsuri"];
                    if (Webtrends.prevPageInfo)
                        dcs["DCS"]["dcsref"] = Webtrends.prevPageInfo["window.location.href"];

                    Webtrends.prevPageInfo = newPageInfo;
                }
            });
        }
    }
    catch (ex) {
        console.log(ex.message);
    }
}

function parsePageInfo() {
    var hash = window.location.href.indexOf("#");
    var dcsuri = window.location.pathname;
    if (hash != -1) {
        //Path will look something like this: http://engsp2k13std01/sites/dev/_layouts/15/start.aspx#/Shared%20Documents/Forms/AllItems.aspx
        //We went dcsuri to be /sites/dev/_layouts/15/start.aspx#/Shared%20Documents/Forms/AllItems.aspx
        dcsuri = dcsuri + window.location.href.substring(hash);
    }
    return {
        "dcssip": location.hostname,
        "dcsuri": dcsuri,
        "dcsqry": ((location.search) ? location.search : ""),
        "WT.ti": document.title,
        "window.location.href": window.location.href
    };
    return locationInfo;
}