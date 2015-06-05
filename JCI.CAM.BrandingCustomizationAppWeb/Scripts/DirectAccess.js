/* namespace declaration */
var JCIDirectAccessClient = JCIDirectAccessClient || {};

/* Variables and Constants */
// Url of the Web App on which the current page is loaded 
JCIDirectAccessClient.sharepointSiteUrl = null;

// These are read from Configuration list. Set some default values for understanding the expected format
// format: '.jci.com', '.johnsoncontrols.com'
JCIDirectAccessClient.inclusionDomains = null;
// format: 'my.jci.com', 'mysite.jci.com', 'apps.jci.com', 'partners.jci.com', 'ag.johnsoncontrols.com', 'webmail.jci.com', 'peoplepicker.jci.com'
JCIDirectAccessClient.exclusionSubDomains = null;
// format: '116.195.164.77;117.195.164.72', '111.195.164.73;112.195.164.74', '114.195.164.71,115.195.164.77'
JCIDirectAccessClient.IPRange = null;
// This is the Security Token against which the service authenticates
JCIDirectAccessClient.daSecurityToken = "DASecurityToken";
// Url of custom web service 
JCIDirectAccessClient.ipWebSvcUrl = "https://getclientipaddress.azurewebsites.net/service.svc/getClientIPJSON/" + JCIDirectAccessClient.daSecurityToken + "?callback=?"

// All of these are constants
JCIDirectAccessClient.cookieName = "JCIDirectAccessIsNetworkExternal";
JCIDirectAccessClient.cookieValueExtNetwork = "true";
JCIDirectAccessClient.cookieValueIntNetwork = "false";
JCIDirectAccessClient.eventToBindDelegatesForHref = "click";
JCIDirectAccessClient.sharepointListName = "DirectAccessConfigurationList";
JCIDirectAccessClient.sharepointFilter = "DirectAccessKey";
JCIDirectAccessClient.inclusionDomainsCookieName = "JCIDirectAccessIncludedDomains";
JCIDirectAccessClient.exclusionSubDomainsCookieName = "JCIDirectAccessExcludedSubDomains";

/* Methods */
// Checks if the cookies are enabled.
JCIDirectAccessClient.AreCookiesEnabled = function () {
    var retVal = true;
    try {
        retVal = navigator.cookieEnabled;
    } catch (e) {
        JCIDirectAccessClient.LogToConsole("Error occured while checking if cookies are enabled. Assuming these are enabled.");
    }
    return retVal;
}

// Gets the url of the web application (Root Site Collection).
// The list containing the DA token and web service url would be located here.
JCIDirectAccessClient.GetRootSiteUrl = function () {
    var retVal = null;

    if (typeof (_spPageContextInfo) == undefined || _spPageContextInfo == null) {
        JCIDirectAccessClient.LogToConsole("ERROR: _spPageContextInfo is null. Cannot setup Direct Access Token");
    } else {

        var absoluteUrl = _spPageContextInfo.webAbsoluteUrl;
        var relativeUrl = _spPageContextInfo.webServerRelativeUrl;
        if (relativeUrl == '/') {
            retVal = absoluteUrl;
        }
        else {
            retVal = absoluteUrl.substring(0, absoluteUrl.indexOf(relativeUrl));
        }
        JCIDirectAccessClient.LogToConsole("SharePointSiteUrl is: " + retVal);
    }

    return retVal;
}

// Returns true if any of the items in array is a substring of the item
JCIDirectAccessClient.IsIteminArray = function (item, arr) {
    if (arr == null || item == null) {
        return false;
    }

    var lowerItem = item.toLowerCase();
    var arrValLower = null;

    var isInArr = false;
    jQuery.each(arr, function (i, val) {
        arrValLower = jQuery.trim(val).toLowerCase();
        if (lowerItem.indexOf(arrValLower) >= 0) {
            isInArr = true;
            return false;
        }
    });
    return isInArr;
}

// Calls SharePoint to retrieve Configuration and then calls the Web Service to retrieve the IP
JCIDirectAccessClient.MakeNetworkCallsAndSetupEventHandlers = function () {

    JCIDirectAccessClient.sharepointSiteUrl = JCIDirectAccessClient.GetRootSiteUrl();
    if (JCIDirectAccessClient.sharepointSiteUrl == null) {
        return;
    }

    jQuery.ajax({
        async: true,
        url: JCIDirectAccessClient.sharepointSiteUrl + "/_api/web/lists/getbytitle('" + JCIDirectAccessClient.sharepointListName + "')/Items?$filter=Title eq '" + JCIDirectAccessClient.sharepointFilter + "'",
        type: "GET",
        headers: { "accept": "application/json;odata=verbose" },
        success: function (data) {
            try {
                if (data.d.results) {
                    var items = data.d.results;
                    jQuery.each(items, function (i, item) {
                        if (item.Title == JCIDirectAccessClient.sharepointFilter) {
                            JCIDirectAccessClient.LogToConsole("Configuration list item found in the list: " + JCIDirectAccessClient.sharepointListName);
                            // The token value would be in the column by name "Value"
                            JCIDirectAccessClient.daSecurityToken = jQuery.trim(item.Value);
                            // The Service Url would be in the column by name "ServiceUrl". 
                            // Include the trailing '/' in the service url
                            JCIDirectAccessClient.ipWebSvcUrl = jQuery.trim(item.ServiceUrl);
                            //?callback=? is provided to execute the JSONP request.
                            JCIDirectAccessClient.ipWebSvcUrl = JCIDirectAccessClient.ipWebSvcUrl + JCIDirectAccessClient.daSecurityToken + "?callback=?";
                            JCIDirectAccessClient.LogToConsole("Direct Access IP Web Service Url is: " + JCIDirectAccessClient.ipWebSvcUrl);

                            var stringValue = item.IPRange;
                            JCIDirectAccessClient.IPRange = stringValue.split(',');
                            JCIDirectAccessClient.LogToConsole("IPRange is: " + JCIDirectAccessClient.IPRange);

                            stringValue = item.IncludedDomains;
                            JCIDirectAccessClient.inclusionDomains = stringValue.split(',');
                            JCIDirectAccessClient.LogToConsole("InclusionDomains is: " + JCIDirectAccessClient.inclusionDomains);
                            // Write the value to cookie for persisting, in case user refreshes the current page/ moves to a different page 
                            JCIDirectAccessClient.CreateCookie(JCIDirectAccessClient.inclusionDomainsCookieName, JCIDirectAccessClient.inclusionDomains);

                            stringValue = item.ExcludedSubDomains;
                            JCIDirectAccessClient.exclusionSubDomains = stringValue.split(',');
                            JCIDirectAccessClient.LogToConsole("ExclusionDomains is: " + JCIDirectAccessClient.exclusionSubDomains);
                            JCIDirectAccessClient.CreateCookie(JCIDirectAccessClient.exclusionSubDomainsCookieName, JCIDirectAccessClient.exclusionSubDomains);

                            // Make the Async Web Service call to retrieve IP
                            JCIDirectAccessClient.RetrieveIPFromWebService();
                            return false;
                        }
                    })
                }
            }
            catch (err) {
                JCIDirectAccessClient.LogToConsole('Error in reading Direct Access configuration from SharePoint: ' + err.message);
            }
        },
        error: function (err) {
            JCIDirectAccessClient.LogToConsole("Error occured in Web Service call to SharePoint to read Direct Access configuration data: " + err.statusText);
        }
    });
}

// Test method for testing exception handling
function UserException(message) {
    this.message = message;
    this.name = "UserException";
}

// Returns 0 if same. -ve if left is less than right, and +ve left is greater.
JCIDirectAccessClient.CompareIP = function (left, right) {
    left = jQuery.trim(left);
    right = jQuery.trim(right);

    var leftArr = left.split('.');
    var rightArr = right.split('.');
    var leftInt = 0;
    var rightInt = 0;
    var difference = 0;

    // loop through all the 4 bytes and compare each of them
    for (var i = 0; i < 4; i++) {
        leftInt = parseInt(leftArr[i]);
        rightInt = parseInt(rightArr[i]);
        difference = leftInt - rightInt;
        if (difference == 0) {
            continue;
        } else {
            break;
        }
    }

    return difference;
}

JCIDirectAccessClient.IsIPInternal = function (clientIP) {
    var isValid = false;

    // If clientIP is empty, then there's something wrong with either the input or the web service.
    // In this case, as per requirement, assume user is on internal n/w
    if (clientIP == null || clientIP.length == 0)
        return true;

    var arr = null;
    var leftDifference = 0;
    var rightDifference = 0;

    jQuery.each(JCIDirectAccessClient.IPRange, function (i, val) {
        arr = val.split(";");
        leftDifference = JCIDirectAccessClient.CompareIP(arr[0], clientIP);

        if (leftDifference <= 0) {
            // Compare with right only if the left is less than or equal to the client
            rightDifference = JCIDirectAccessClient.CompareIP(clientIP, arr[1]);
            if (rightDifference <= 0) {
                isValid = true;
                return false; // break the loop
            }
        } else {
            return; // continue
        }
    });

    return isValid;
}

JCIDirectAccessClient.SetCookieAndUpdateDom = function (isIpInternal) {

    if (isIpInternal) // on Internal network
    {
        JCIDirectAccessClient.LogToConsole("User is on Internal network");
        JCIDirectAccessClient.CreateCookie(JCIDirectAccessClient.cookieName, JCIDirectAccessClient.cookieValueIntNetwork);
    }
    else {
        JCIDirectAccessClient.LogToConsole("User is on External network");
        JCIDirectAccessClient.CreateCookie(JCIDirectAccessClient.cookieName, JCIDirectAccessClient.cookieValueExtNetwork);

        JCIDirectAccessClient.UpdateDom();
    }
}

JCIDirectAccessClient.RetrieveIPFromWebService = function () {

    jQuery.getJSON(JCIDirectAccessClient.ipWebSvcUrl, function (data) {
        try {
            // Since the call is async, all the below logic has to execute when the call returns, in the success call back
            var hostIPValue = data.GetClientIPAddressResult.Host;
            JCIDirectAccessClient.LogToConsole("Client IP is: " + hostIPValue);
            var isIpOfInternalNetwork = JCIDirectAccessClient.IsIPInternal(hostIPValue);
            // Wire up the handlers for anchor and iframe
            JCIDirectAccessClient.SetCookieAndUpdateDom(isIpOfInternalNetwork);
        } catch (err) {
            JCIDirectAccessClient.LogToConsole("Error occured in Web Service success event handler: " + err.message);
        }
    })
    .fail(function () { JCIDirectAccessClient.LogToConsole("Error occurred calling the GetIP Web Service"); }) // getJson catch
    .always(function () { JCIDirectAccessClient.LogToConsole("Completed calling the GetIP Web Service"); }) // getJson finally block

}

// Contains the logic to check the Url, if needed Transform, and return the new Url
// Returns null if no change in Url
JCIDirectAccessClient.CheckAndTransformUrl = function (transformUrl) {

    if (transformUrl != undefined) {

        // Empty anchor element, as the Href object has properties to provide the parts of a Url
        // Example: hrefObj.hostname, hrefObj.protocol, hrefObj.port
        var newAnchorObj = document.createElement('a');
        newAnchorObj.href = transformUrl;

        jQuery(newAnchorObj).attr('href', function (i, currentUrl) {

            // Make sure it is not a relative Url
            if (this.hostname.length > 0) {

                // JS method IndexOf is case-Sensitive
                var hostDomain = this.hostname;
                var protocol = this.protocol;
                var port = this.port;
                var pathname = this.pathname;
                var hash = this.hash;
                var querystring = this.search;

                var portString = "";
                var sslString = "";
                var finalPartString = "";
                var pathString = "";
                var convertToJuniperFormat = false;

                // IsIteminArry does a case-insensitive comparision
                var isUrlInIncludedDomains = JCIDirectAccessClient.IsIteminArray(hostDomain, JCIDirectAccessClient.inclusionDomains);
                if (isUrlInIncludedDomains) {
                    convertToJuniperFormat = !JCIDirectAccessClient.IsIteminArray(hostDomain, JCIDirectAccessClient.exclusionSubDomains);
                }

                if (convertToJuniperFormat) {
                    // IE returns 80/443 for http/https. But chrome return empty string
                    if (port != "80" && port != '443' && port != '') {
                        portString = ",Port=" + port;
                    }

                    if (protocol.toLowerCase().indexOf("https") == 0) {
                        sslString = ",SSL";
                    }

                    // In case of http://abc.jci.com chrome returns pathname as '/'. IE returns empty string ''
                    // for http://abc.jci.com/page.aspx chrome returns '/page.aspx' and IE returns 'page.aspx'
                    // We would want it to be without the '/' at the start. So remove it.
                    if (pathname.indexOf('/') == 0) {
                        pathname = pathname.substring(1, pathname.length);
                    }

                    // only try to get the finalPart and Path string if path is not empty
                    if (pathname.length > 0) {
                        finalPartString = JCIDirectAccessClient.GetFinalPart(pathname);
                        var finalPartIndex = pathname.lastIndexOf(finalPartString);
                        pathString = pathname.substring(0, finalPartIndex);
                    }

                    // querystring and hash are part of the final string
                    finalPartString = finalPartString + querystring + hash;

                    // add the '+' only if the finalPartString is not emtpy
                    if (finalPartString.length > 0) {
                        finalPartString = '+' + finalPartString;
                    }

                    // No O365 domains are present
                    var changedUrl = 'https://myaccess.ga.johnsoncontrols.com/' + pathString + ',DanaInfo=' + hostDomain
                        + portString + sslString + finalPartString;

                    return changedUrl;
                }
            }
        });

        // newUrl and transformUrl are same if nothing has changed
        var newUrl = jQuery(newAnchorObj).attr('href');

        if (newUrl != transformUrl) {
            JCIDirectAccessClient.LogToConsole("Changing the Url: " + transformUrl + " to: " + newUrl);
        } else {
            JCIDirectAccessClient.LogToConsole("No change in Url: " + transformUrl);
        }

        return newUrl;
    }
}

JCIDirectAccessClient.UpdateHref = function () {

    JCIDirectAccessClient.LogToConsole("Binding Click event to links");

    // Binding using delegate binds not only to the elements in the current DOM, but any elements added to it dynamically (Ajax)
    // A selector could have been passed as the first arg to delegate to get only the anchor tags with hrefs of *.jci.com or *.johnsoncontrols.com,
    // But by default selectors are case-sensitive on the values
    jQuery(document).delegate('a', JCIDirectAccessClient.eventToBindDelegatesForHref, function () {
        try {
            // Check for nulls
            if (jQuery(this) != undefined && jQuery(this).attr('href') != undefined) {

                // get the Host
                jQuery(this).attr('href', function (i, origValue) {

                    var newUrl = JCIDirectAccessClient.CheckAndTransformUrl(origValue);

                    // Only update, if the value has changed
                    if (newUrl != origValue) {
                        return newUrl;
                    }
                });
            }
        } catch (err) {
            JCIDirectAccessClient.LogToConsole("Error occurred in Click Event Handler: " + err.message);
        }
    });
}

JCIDirectAccessClient.CreateCookie = function (name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
    JCIDirectAccessClient.LogToConsole("Cookie created with value: " + value);
    JCIDirectAccessClient.LogToConsole("document.cookie value is: " + document.cookie);
}

JCIDirectAccessClient.ReadCookie = function (name) {
    // format of document.cookie is - "cookie1=value; cookie2=value";
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) {
            var cValue = c.substring(nameEQ.length, c.length);
            return cValue;
        }
    }
    return null;
}

// Transforms the Urls of iFrame Src to Juniper format
// Note: Dynamically inserted iFrames are not yet handled
JCIDirectAccessClient.UpdateIframeSrc = function () {

    JCIDirectAccessClient.LogToConsole("Analyzing iFrame src urls for transformation");

    jQuery('iframe').each(function (index) {

        var srcUrl = jQuery(this).attr('src');

        if (srcUrl != undefined) {
            var changedUrl = JCIDirectAccessClient.CheckAndTransformUrl(srcUrl);

            if (changedUrl != srcUrl) {
                jQuery(this).attr('src', changedUrl);
            }
        }
    });
}

JCIDirectAccessClient.UpdateDom = function () {

    // Update a href
    JCIDirectAccessClient.UpdateHref();

    // update iframe src
    JCIDirectAccessClient.UpdateIframeSrc();
}

// Check if cookie exists, if not find the network type and set it
JCIDirectAccessClient.SetupDirectAccessClient = function () {

    if (!JCIDirectAccessClient.AreCookiesEnabled) {
        // Cookies are disabled. As per requirement, code should assume user is connected to internal n/w.
        JCIDirectAccessClient.LogToConsole("Cookies are disabled. Assuming internal network");
        return;
    } else {
        JCIDirectAccessClient.LogToConsole("Cookies are enabled");
    }

    try {
        var cookieValue = JCIDirectAccessClient.ReadCookie(JCIDirectAccessClient.cookieName);
        JCIDirectAccessClient.LogToConsole("Value of Cookie is: " + cookieValue);

        if (!cookieValue) // No Cookie
        {
            JCIDirectAccessClient.LogToConsole("Cookie not found");

            JCIDirectAccessClient.MakeNetworkCallsAndSetupEventHandlers();
        }
        else //Cookie Exists
        {
            JCIDirectAccessClient.LogToConsole("Cookie found");
            // If Network cookie is found then, domain list cookies would also exist. Read them
            JCIDirectAccessClient.inclusionDomains = JCIDirectAccessClient.ReadCookie(JCIDirectAccessClient.inclusionDomainsCookieName).split(',');
            JCIDirectAccessClient.exclusionSubDomains = JCIDirectAccessClient.ReadCookie(JCIDirectAccessClient.exclusionSubDomainsCookieName).split(',');
            if (cookieValue == JCIDirectAccessClient.cookieValueExtNetwork) {
                JCIDirectAccessClient.LogToConsole("Network is External");
                JCIDirectAccessClient.UpdateDom();
            } else {
                JCIDirectAccessClient.LogToConsole("Network is Internal");
            }
        }
    } catch (err) {
        JCIDirectAccessClient.LogToConsole("Error occured in VerifyCookies: " + err.message);
    }
}

JCIDirectAccessClient.GetFinalPart = function (path) {
    var retVal = "";
    try {
        if (path == "" || path.indexOf('/') == -1) {
            // string is empty ex: http://www.microsoft.com/ - return ""
            // OR http://www.microsoft.com/default.aspx - return default.aspx
            return path;
        } else {
            var lastIndex = path.lastIndexOf('/');
            var length = path.length;

            if (lastIndex != length - 1) {
                // http://www.microsoft.com/corp/en-us/default.aspx - return default.aspx
                retVal = path.substring(lastIndex + 1, length)
                return retVal;
            } else {
                // http://www.microsoft.com/en-us/corp/lt/ - return lt
                retVal = path.substring(0, lastIndex);
                lastIndex = retVal.lastIndexOf('/');
                retVal = retVal.substring(lastIndex, retVal.length);
                return retVal;
            }
        }
    } catch (e) {
        JCIDirectAccessClient.LogToConsole("Error in GetFinalPart: " + e.message);
    }
    finally {
        return retVal;
    }
}

JCIDirectAccessClient.LogToConsole = function (strMessage) {
    // "console" works only on IE9 and above
    if (window.console) {
        window.console.log(strMessage);
    }
}


/* Entry Point */
// Check if the network location (internal/ external) is determined by calling SetupDirectAccessClient.
// If not the method will make the determination and set the cookie
JCIDirectAccessClient.ready = function () {
    jQuery(document).ready(JCIDirectAccessClient.SetupDirectAccessClient);
};
