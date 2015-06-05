"use strict";

webshim.polyfill('forms forms-ext');

// variable used for cross site CSOM calls
var context;
// peoplePicker variable needs to be globally scoped as the generated html contains JS that will call into functions of this class
var priOwners;
var secOwners;

var queryStringList = {};
var JCI = {};
JCI.CAM = {} || JCI.CAM;

JCI.CAM.SiteRequest = {
    apiUrl: {
        'save': 'Home/SaveRequest',
        'getdata': 'Home/GetRequestData',
        'create': 'Home/Create',
        'checkIfExists': 'Home/CheckIfExists',
        'getFormData': 'Home/GetFormData',
        'edit': 'Home/EditRequest',
        'delete': 'Home/Delete'
    },

    show: ko.observable(false),
    siteTypeDescription: ko.observable(),
    siteTypeName: ko.observable(),
    siteTypeImg: ko.observable(),
    siteUrlValid: ko.observable('na'),
    errorMsg: ko.observable(""),
    submitStatus: ko.observable('no'),  // no - Not submitted, wait - processing
    siteRoot: ko.observable(),
    loading: ko.observable('wait'),
    mode: ko.observable('new'),
    baseType: ko.observable('PROJECTSITE'),
    datePrimaryCheck: ko.observable('ok'),

    // Vaidates whether the site Url is available or not
    validateSiteURL: function () {

        console.log("URL check!");
        var re = /^[A-Z0-9a-z-_]+$/;
        if (!re.test(request.siteURL())) {
            request.siteURL.setError("URL can only contain alphanumeric characters.");
            JCI.CAM.SiteRequest.siteUrlValid('na2');
            return;
        }
        if (request.siteURL.isValid()) {
            JCI.CAM.SiteRequest.siteUrlValid('wait');
            $.ajax({
                type: "POST",
                url: JCI.CAM.SiteRequest.apiUrl.checkIfExists + "?" + JCI.CAM.SiteRequest.getStandardTokens(),
                data: "{'name':'" + (request.webAppUrl() + request.managedPath() + "/" + request.siteURL()) + "'}",
                contentType: 'application/json; charset=utf-8',
                success: function (resultdata) {
                    JCI.CAM.SiteRequest.siteUrlValid('na');
                    if (resultdata.exists === false) {
                        JCI.CAM.SiteRequest.siteUrlValid('ok');
                    }
                    else {
                        JCI.CAM.SiteRequest.siteUrlValid('no');
                    }
                    console.log(resultdata);
                    //alert(ko.toJSON(resultdata));
                },
                error: function (error) {
                    if (error.status !== 302 && error.status !== 409 && error.status !== 200) {
                        ///ToDo: need to process this and show appropriate message
                        /// alert(error.responseText + " " + error.statusText);
                        JCI.CAM.SiteRequest.siteUrlValid('na');
                    }
                }
            });
        }
    },

    // Validate the input on the site request page and allows/blocks the request
    validate: function () {
        var el = document.getElementById("marker");
        el.scrollIntoView(true);

        var valid = true;
        JCI.CAM.SiteRequest.errorMsg("");
        var error = "<div data-bind='visible: JCI.CAM.SiteRequest.errorMsg() !== '', html:JCI.CAM.SiteRequest.errorMsg' role='alert' class='alert alert-danger alert-dismissable error'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>";
        if (JCI.CAM.SiteRequest.siteUrlValid() === 'na' || JCI.CAM.SiteRequest.siteUrlValid() === 'no') {
            valid = false;
            request.siteURL.setError("URL is not available");
        }
        for (var prop in request) {
            if (request[prop] !== undefined && request[prop] !== null && typeof (request[prop]) == 'function') {
                if (request[prop].isValid !== null && request[prop].isValid !== undefined && typeof (request[prop].isValid) === 'function' && !request[prop].isValid()) {
                    valid = false;
                    request[prop].setError(request[prop].error());
                    error += VarDisplayNameMap[prop] + ": " + request[prop].error() + "<br />";
                }
            }
        }
        if (request.isValid() && valid === true) {
            if (JCI.CAM.SiteRequest.siteUrlValid() === 'na' || JCI.CAM.SiteRequest.siteUrlValid() === 'no') {
                valid = false;
                request.siteURL.setError("URL is not available");
            }

            var users = JSON.parse($('#hdnAdministrators').val());
            if (users.length === 0) {
                request.primaryOwner.setError("At least one user is required");
                valid = false;
            }
            if (users.length > 1) {
                request.primaryOwner.setError("Only one user can be the primary owner");
                valid = false;
            }
            // check each element
            if (valid) {
                JCI.CAM.SiteRequest.submit();
            }
        }
        if (!valid) {
            error += "</div>";
            JCI.CAM.SiteRequest.errorMsg(error);
            $('.error').html(error);
        }
    },

    submit: function () {
        if (JCI.CAM.SiteRequest.submitStatus() == 'wait') {
            return;
        }
        if (JCI.CAM.SiteRequest.submitStatus() == 'yes') {
            $('.message').text("Request has already been submitted, please close your browser").show();
            return;
        }
        JCI.CAM.SiteRequest.submitStatus('wait');

        var el = document.getElementById("marker");
        el.scrollIntoView(true);

        var msg = "";
        $('.message').text("").hide();
        // check if the request is being edited
        if (request.requestId() > 0 && queryStringList['mode'] !== undefined && queryStringList['mode'] == 'edit') {
            $.ajax({
                type: "POST",
                url: JCI.CAM.SiteRequest.apiUrl.save + "?" + JCI.CAM.SiteRequest.getStandardTokens(),
                data: ko.toJSON(request),
                contentType: 'application/json; charset=utf-8',
                success: function (resultdata) {
                    // ToDo: Show user that request has been successfuly saved - Done
                    if (resultdata.success) {
                        JCI.CAM.SiteRequest.submitStatus('yes');
                    }
                    else {
                        JCI.CAM.SiteRequest.submitStatus('no');
                    }
                    console.log(resultdata);
                },
                error: function (error) {
                    debugger;
                    if (error.status !== 302 && error.status !== 409 && error.status !== 200) {
                        JCI.CAM.SiteRequest.submitStatus('no');
                        // ToDo: need to process this and show appropriate message
                        console.log(error.responseText + " " + error.statusText);
                    }
                }
            });
        }
        else {  // or its a new request
            $.ajax({
                type: "POST",
                url: JCI.CAM.SiteRequest.apiUrl.create + "?" + JCI.CAM.SiteRequest.getStandardTokens(),
                data: ko.toJSON(request),
                contentType: 'application/json; charset=utf-8',
                success: function (resultdata) {
                    console.log(resultdata);
                    // ToDo: Show user that request has been successfuly saved
                    if (resultdata.result == true) {
                        $('.message').text(resultdata.message).removeClass('alert-danger').addClass('alert alert-success').show();
                        JCI.CAM.SiteRequest.submitStatus('yes');
                    }
                    else {
                        $('.message').text(resultdata.message).removeClass('alert-success').addClass('alert alert-danger').show();
                        JCI.CAM.SiteRequest.submitStatus('no');
                    }

                },
                error: function (error) {
                    debugger;
                    if (error.status !== 302 && error.status !== 409 && error.status !== 200) {
                        JCI.CAM.SiteRequest.submitStatus('no');
                        ///ToDo: need to process this and show appropriate message
                        /// alert(error.responseText + " " + error.statusText);
                        //alert("error");
                    }
                }
            });
        }
    },

    submitInfo: function () {
        if (JCI.CAM.SiteRequest.submitStatus() == 'wait') {
            return;
        }
        if (JCI.CAM.SiteRequest.submitStatus() == 'yes') {
            $('.message').text("Request has already been submitted, please close your browser").show();
            return;
        }
        JCI.CAM.SiteRequest.submitStatus('wait');
        var msg = "";
        $('.message').text("").hide();
        // check if the request is being edited
        if (request.requestId() > 0 && JCI.CAM.SiteRequest.mode != undefined && JCI.CAM.SiteRequest.mode() == 'edit') {
            $.ajax({
                type: "POST",
                url: JCI.CAM.SiteRequest.apiUrl.edit + "?" + JCI.CAM.SiteRequest.getStandardTokens(),
                data: ko.toJSON({
                    request: parseInt(request.requestId()),
                    info: $('#AddInfo').val()
                }),
                contentType: 'application/json; charset=utf-8',
                success: function (resultdata) {
                    // ToDo: Show user that request has been successfuly saved - Done
                    if (resultdata.success) {
                        JCI.CAM.SiteRequest.submitStatus('yes');
                        $('.message').text(resultdata.message).removeClass('alert-danger').addClass('alert alert-success').show();
                    }
                    else {
                        JCI.CAM.SiteRequest.submitStatus('no');
                        $('.message').text(resultdata.message).removeClass('alert-success').addClass('alert alert-danger').show();
                    }
                    console.log(resultdata);
                },
                error: function (error) {
                    debugger;
                    if (error.status !== 302 && error.status !== 409 && error.status !== 200) {
                        JCI.CAM.SiteRequest.submitStatus('no');
                        // ToDo: need to process this and show appropriate message
                        console.log(error.responseText + " " + error.statusText);
                    }
                }
            });
        }
    },

    deleteRequest: function () {
        if (JCI.CAM.SiteRequest.submitStatus() == 'wait') {
            return;
        }
        if (JCI.CAM.SiteRequest.submitStatus() == 'yes') {
            $('.message').text("Request has already been deleted, please close your browser").show();
            return;
        }
        JCI.CAM.SiteRequest.submitStatus('wait');
        var msg = "";
        $('.message').text("").hide();
        // check if the request is being edited
        if (request.requestId() > 0 && JCI.CAM.SiteRequest.mode != undefined && JCI.CAM.SiteRequest.mode() == 'delete') {
            $.ajax({
                type: "POST",
                url: JCI.CAM.SiteRequest.apiUrl.delete + "?" + JCI.CAM.SiteRequest.getStandardTokens(),
                data: ko.toJSON({
                    requestId: parseInt(request.requestId())
                }),
                contentType: 'application/json; charset=utf-8',
                success: function (resultdata) {
                    // ToDo: Show user that request has been successfuly saved - Done
                    if (resultdata.success) {
                        JCI.CAM.SiteRequest.submitStatus('yes');
                        $('.message').text(resultdata.message).removeClass('alert-danger').addClass('alert alert-success').show();
                    }
                    else {
                        JCI.CAM.SiteRequest.submitStatus('no');
                        $('.message').text(resultdata.message).removeClass('alert-success').addClass('alert alert-danger').show();
                    }
                    console.log(resultdata);
                },
                error: function (error) {
                    debugger;
                    if (error.status !== 302 && error.status !== 409 && error.status !== 200) {
                        JCI.CAM.SiteRequest.submitStatus('no');
                        // ToDo: need to process this and show appropriate message
                        console.log(error.responseText + " " + error.statusText);
                    }
                }
            });
        }
    },
    /// This method is used to get standard token for sharepoint app
    getStandardTokens: function (ensureUniqueness, spSiteUrl) {
        var strValue = "";
        var spHostUrl = queryStringList["SPHostUrl"];
        // override sphosturl with specified siteurl
        if (spSiteUrl != undefined && spSiteUrl != '') {
            spHostUrl = spSiteUrl;
        }

        // read sharepoint tokens from query string
        var spLanguage = queryStringList["SPLanguage"];
        var spClientTag = queryStringList["SPClientTag"];
        var spProductNumber = queryStringList["SPProductNumber"];
        var spAppWebUrl = queryStringList["SPAppWebUrl"];
        var spHostTitle = queryStringList["SPHostTitle"];
        var spHostLogoUrl = queryStringList["SPHostLogoUrl"];
        var displayType = queryStringList["DisplayType"];

        // concat into single string
        if (null != spHostUrl && "" != spHostUrl) {
            strValue = "SPHostUrl=" + encodeURIComponent(spHostUrl) + "&";
        }
        if (null != spLanguage && "" != spLanguage) {
            strValue += "SPLanguage=" + encodeURIComponent(spLanguage) + "&";
        }
        if (null != spClientTag && "" != spClientTag) {
            strValue += "SPClientTag=" + encodeURIComponent(spClientTag) + "&";
        }
        if (null != spProductNumber && "" != spProductNumber) {
            strValue += "SPProductNumber=" + encodeURIComponent(spProductNumber) + "&";
        }
        if (null != spAppWebUrl && "" != spAppWebUrl) {
            strValue += "SPAppWebUrl=" + encodeURIComponent(spAppWebUrl) + "&";
        }
        if (null != spHostTitle && "" != spHostTitle) {
            strValue += "SPHostTitle=" + encodeURIComponent(spHostTitle) + "&";
        }
        if (null != spHostLogoUrl && "" != spHostLogoUrl) {
            strValue += "SPHostLogoUrl=" + encodeURIComponent(spHostLogoUrl) + "&";
        }
        if (null != displayType && "" != displayType) {
            strValue += "DisplayType=" + encodeURIComponent(displayType);
        }

        if (strValue.length > 0 && strValue.lastIndexOf("&") == (strValue.length - 1)) {
            strValue = strValue.substr(0, strValue.length - 1);
        }

        if (ensureUniqueness == true) {
            strValue += "&_cache=" + new Date().getTime();
        }
        return strValue;
    },

    populateQueryString: function (fullUrl) {
        if (fullUrl === undefined || fullUrl === "" || fullUrl === null) {
            return;
        }
        //get the parameters
        fullUrl.match(/\?(.+)$/);
        var params = RegExp.$1;
        // split up the query string and store in an
        // associative array
        var params = params.split("&");
        for (var i = 0; i < params.length; i++) {
            var tmp = params[i].split("=");
            queryStringList[tmp[0]] = $.trim(decodeURIComponent(tmp[1]));
        }
    },

    showOptions: function () {
        JCI.CAM.SiteRequest.show(!JCI.CAM.SiteRequest.show());
    },

    setSiteType: function (e) {
        try {
            var id = "";
            if (typeof (e) !== "string") {
                if (e == null) {
                    var e = event;
                }
                if (e == null) {
                    var e = Event;
                }
                if (e.currentTarget === null || e.currentTarget === undefined) {
                    if (e.srcElement !== null) {
                        var curEl = e.srcElement;
                        while (curEl.attributes['alt'] === undefined) {
                            var curEl = curEl.parentElement;
                        }
                        id = curEl.attributes['alt'].value;
                    }
                }
                else {
                    id = e.currentTarget.attributes['alt'].value;
                }
            }
            else {
                id = e;
            }

            // Get Site Type from id
            var sites = request.formData.templates.sites();
            var siteType = "";
            for (var site in sites) {
                if (sites[site].Id === id) {
                    // Highlight the selected option
                    $('.thumb').removeClass('SiteTypeSelected');
                    $('.thumb[alt=' + id + ']').addClass('SiteTypeSelected');
                    
                    JCI.CAM.SiteRequest.siteTypeDescription(sites[site].Description);
                    JCI.CAM.SiteRequest.siteTypeName(sites[site].Name);
                    JCI.CAM.SiteRequest.siteTypeImg(sites[site].TypeIcon);
                    JCI.CAM.SiteRequest.baseType(sites[site].BaseId);

                    if (sites[site].BaseId != "PROJECTSITE")
                    {
                        request.endDate.clearError();
                    }

                    request.siteTemplate(sites[site].Id);
                    request.webAppUrl(sites[site].WebappUrl);
                    request.managedPath(sites[site].ManagedPath);
                    request.siteType(sites[site].Id);
                    request.siteType.notifySubscribers();

                    var newValues = defaultValues[id];
                    if (newValues !== null || newValues !== 'undefined') {
                        for (var prop in newValues) {
                            if (newValues[prop] !== undefined && newValues[prop] !== null && typeof (request[prop]) == 'function') {
                                request[prop](newValues[prop]);
                                (request[prop]).clearError();
                            }
                        }
                        //for (var prop in request) {
                        //    if (newValues[prop] !== undefined && newValues[prop] !== null && typeof (request[prop]) == 'function') {
                        //        request[prop](newValues[prop]);
                        //        // (request[prop]).clearError();
                        //    }
                        //}
                    }
                }
            }
        }
        catch (e) {
            // console.log(e);
        }
    },

    primaryOwnerChanged: function () {
        var users = JSON.parse($('#hdnAdministrators').val());
        if (users.length > 1) {
            request.primaryOwner.setError("Only one primary owner can be specified");
        }
        else {
            $('#Administrators').val($('#hdnAdministrators').val());
        }
    },

    secondaryOwnerChanged: function () {
        Request.secondaryOwner($('#hdnsowner').val());
    },

    setRequestValues: function (newValues) {
        for (var prop in request) {
            if (newValues[prop] === undefined || newValues[prop] === null) {
                continue;
            }
            // Parse and convert dates from JSON to Date string
            if (newValues[prop].toString().lastIndexOf('/Date(') == 0) {
                newValues[prop] = new Date(parseInt(value.replace(/\/Date\((.*?)\)\//gi, "$1")));
            }
            if (newValues[prop] !== undefined && newValues[prop] !== null) {
                request[prop](newValues[prop]);
            }
        }

        // Get Site Type Id from Site Type Name
        var sites = request.formData.templates.sites();
        var siteType = "";
        for (var site in sites) {
            if (sites[site].Title === newValues.siteType) {
                JCI.CAM.SiteRequest.siteTypeName(sites[site].Title);
                JCI.CAM.SiteRequest.siteTypeDescription(sites[site].Description);
                JCI.CAM.SiteRequest.siteTypeImg(sites[site].TypeIcon);
                JCI.CAM.SiteRequest.setSiteType(sites[site].Id);
            }
        }
    },

    initFormData: function (newValues) {
        var vm = new viewModel(newValues);
        request.formData = vm;

        var levels = request.formData.confidentiallityLevels();   // Create a copy
        var arr = [];
        for (var index in levels) {
            arr.push({ text: levels[index], value: levels[index] });
        }
        request.formData.confidentiallityLevels.removeAll(); // Empty the array
        request.formData.confidentiallityLevels.push({ text: 'Select', value: null });

        for (var index in arr) {
            request.formData.confidentiallityLevels.push(arr[index]);
        }

        request.primaryOwner(ko.toJSON(newValues.user));

        request.siteBU(newValues.currentBu);
        request.siteRegion(newValues.currentRegion);
        request.siteCountry(newValues.currentCountry);

        if (queryStringList['edit'] !== undefined && queryStringList['edit'] !== null) {
            $('#action').text('Edit site request')
            queryStringList.mode = 'edit';  // Set the mode to edit
            JCI.CAM.SiteRequest.mode('edit');
            request.requestId(parseInt(queryStringList['edit']));  // Assing value to property

            $.ajax({
                type: "GET",
                url: JCI.CAM.SiteRequest.apiUrl.getdata + "?requestId=" + request.requestId() + "&" + JCI.CAM.SiteRequest.getStandardTokens(),
                success: function (resultdata) {
                    JCI.CAM.SiteRequest.setRequestValues(resultdata);
                },
                error: function (error) {
                    if (error.status !== 302 && error.status !== 409 && error.status !== 200) {
                        // ToDo: need to process this and show appropriate message
                        // alert(error.responseText + " " + error.statusText);
                        // alert("error");
                    }
                }
            });
        }
        if (queryStringList['requestId'] !== undefined && queryStringList['requestId'] !== null) {
            queryStringList.mode = 'delete';  // Set the mode to edit
            JCI.CAM.SiteRequest.mode('delete');
            request.requestId(parseInt(queryStringList['requestId']));  // Assing value to property

            $.ajax({
                type: "GET",
                url: JCI.CAM.SiteRequest.apiUrl.getdata + "?requestId=" + request.requestId() + "&" + JCI.CAM.SiteRequest.getStandardTokens(),
                success: function (resultdata) {
                    JCI.CAM.SiteRequest.setRequestValues(resultdata);
                },
                error: function (error) {
                    if (error.status !== 302 && error.status !== 409 && error.status !== 200) {
                        // ToDo: need to process this and show appropriate message
                        // alert(error.responseText + " " + error.statusText);
                        // alert("error");
                    }
                }
            });
        }

        ko.applyBindings(request);  // Apply View model bindings
        JCI.CAM.SiteRequest.setSiteType('PROJECTSITE0');
    },
    
    checkDate: function (value) {
            var allowBlank = true;
            var minYear = 1980;
            var maxYear = 2100;

            var errorMsg = "";
            var re = /(\d{1,2})\/(\d{1,2})\/(\d{4})/;
            var m;
            var day = 0;
            var month = 0;
            var year = 0;
            if ((m = re.exec(value)) !== null) {
                if (m.index === re.lastIndex) {
                    re.lastIndex++;
                }
                // View your result using the m-variable.
                day = m[2];
                month = m[1];
                year = m[3];
                
            }
            else
            {
                errorMsg = "Invalid date!";
                return false;
            }

            // regular expression to match required date format
            if (!allowBlank && value == "") {
                errorMsg = "Empty date not allowed!";
                return false;
            }

            if (day > 28)
            {
                if (month == 2) {
                    if (day > 29)
                    {
                        errorMsg = "Invalid date";
                    }
                    if(!((year % 400 == 0) || ((year % 4 == 0) && (year % 100 != 0)))) // is not a leap year
                    {
                        if (day != 29) {
                            errorMsg = "Invalid date"
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            if (errorMsg != "") {            
                return false;
            }

            return true;
        },
}

var defaultValues = {   // default values that are pre-selected if the site type changes
    'COMMUNITY0': { communitySiteType: 'Public' },
    'STS0': {},
    'PROJECTSITE0': {},
    'COLLABORATION0': {},
    'PARTNER0': {}
}

var siteTemplates = function (data) {
    var self = this;
    self.sites = ko.observableArray(data);
}

var VarDisplayNameMap = {
    requestId: "Invalid site request, please refresh your browser",
    siteType: "Site Type",
    siteName: "Site Name",
    description: "Site Description",
    siteURL: "Site URL",
    primaryOwner: "Primary owner",
    secondaryOwner: "Secondary Owner",
    siteBU: "Site Business Unit",
    siteRegion: "Site Region",
    siteCountry: "Site Country",
    confidentiality: "Confidentiality and Security Level",
    isTestSite: "Test Site checkbox",
    isAgreed: "Terms & Service agreement",
    managedPath: "The site templates were not loaded correctly, refresh the page and try again",
    webAppUrl: "The site templates were not loaded correctly, refresh the page and try again",
    siteTemplate: "The site templates were not loaded correctly, refresh the page and try again",
    jciCustomers: "Customer Name",
    startDate: "Start Date",
    endDate: "End Date"
}

var viewModel = function (data) {
    var self = this;
    self.businessUnit = ko.observableArray(data.businessUnits);
    self.regions = ko.observableArray(data.regions);
    self.countries = ko.observableArray(data.countries);
    self.confidentiallityLevels = ko.observableArray(data.classificationLevels);
    self.templates = new siteTemplates(data.siteTemplates);
}

var request = { // request view model
    requestId: ko.observable(0),
    siteType: ko.observable("").extend({ required: true }),
    siteTemplate: ko.observable("not-required").extend({ required: true }),
    siteName: ko.observable("").extend({ required: true }).extend({ pattern: /^[A-Z0-9a-z-_\s]+$/ }).extend({ maxLength: 100 }),
    description: ko.observable().extend({ required: true }),
    webAppUrl: ko.observable().extend({ required: true }),
    managedPath: ko.observable().extend({ required: true }),
    siteURL: ko.observable("").extend({ maxLength: 100 }).extend({ required: true }).extend({ pattern: /^[A-Z0-9a-z-_]+$/ }),
    confidentiality: ko.observable(null).extend({ required: { onlyIf: function () { return JCI.CAM.SiteRequest.baseType() !== "COMMUNITYSITE"; } } }),
    primaryOwner: ko.observable([]).extend({ required: { params: true, message: 'At least one primary owner is required' }, minLength: 3 }).extend({ notify: 'always' }),
    secondaryOwner: ko.observable([]),
    siteBU: ko.observable().extend({ required: true }),
    siteRegion: ko.observable().extend({ required: true }),
    siteCountry: ko.observable().extend({ required: true }),
    isTestSite: ko.observable(false),
    isAgreed: ko.observable().extend({ equal: { params: true, message: 'This must be checked' } }),   // by default it should be null (not false) to indicate that user has not checked/unchecked this
    jciCustomers: ko.observable().extend({ required: { onlyIf: function () { return JCI.CAM.SiteRequest.baseType() == "PARTNERSITE"; } } }).extend({ pattern: /^[A-Z0-9a-z-_\s]+$/ }),
    // Extended properties - These properties are only applicable for Collaboration and Project site
    startDate: ko.observable().extend({ required: { onlyIf: (function () { return JCI.CAM.SiteRequest.baseType() == "PROJECTSITE"; }) } }), // initialize with Today's date
    endDate: ko.observable().extend({ date: true }),
    projectNumber: ko.observable(),
    communitySiteType: ko.observable().extend({ required: { onlyIf: (function () { return JCI.CAM.SiteRequest.baseType() == "COMMUNITYSITE"; }) } }), // initialize with Today's date,

    // FormData
    formData: null
};

JCI.CAM.SiteRequest.loadHandler = function () { // On Load event handler

    JCI.CAM.SiteRequest.populateQueryString(document.referrer.toString()); // Populate QueryStrings
    JCI.CAM.SiteRequest.populateQueryString(window.location.toString()); // Populate QueryStrings

    ko.validation.configure({           // Setup and configure validation
        registerExtenders: true,
        messagesOnModified: true,
        insertMessages: true,
        parseInputAttributes: true,
        messageTemplate: null,
        grouping: {
            deep: true,        //by default grouping is shallow
            observable: true,   //and using observables
            live: true		    //react to changes to observableArrays if observable === true
        }
    });

    request.errors = ko.validation.group(request); // Register View Model for Validation
    JCI.CAM.SiteRequest.ApplySubscribers(); // Add subscriptions to view model members
    $('#wizard').easyWizard({ showButtons: false, showSteps: false });  // Initialize Azure wizard control

    // People Picker init code
    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    var spHostUrl = decodeURIComponent(getQueryStringParameter('SPHostUrl'));
    var appWebUrl = decodeURIComponent(getQueryStringParameter('SPAppWebUrl'));
    var spLanguage = decodeURIComponent(getQueryStringParameter('SPLanguage'));

    //Build absolute path to the layouts root with the spHostUrl
    var layoutsRoot = spHostUrl + '/_layouts/15/';

    // Remove this once issue is resolved
    layoutsRoot = "/Scripts/";

    //load all appropriate scripts for the page to function
    $.getScript(layoutsRoot + "MicrosoftAjax.js", function (s, txt, xhr) {
        $.getScript(layoutsRoot + 'SP.Runtime.js',
            function (s, txt, xhr) {
                $.getScript(layoutsRoot + 'SP.js',
                    function (s, txt, xhr) {
                        //Load the SP.UI.Controls.js file to render the App Chrome
                        $.getScript(layoutsRoot + 'SP.UI.Controls.js',
                            function (s, txt, xhr) {
                                renderSPChrome();
                                //load scripts for cross site calls (needed to use the people picker control in an IFrame)
                                $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function (s, txt, xhr) {
                                    context = new SP.ClientContext(appWebUrl);
                                    var factory = new SP.ProxyWebRequestExecutorFactory(appWebUrl);
                                    context.set_webRequestExecutorFactory(factory);

                                    //Make a people picker control
                                    //1. context = SharePoint Client Context object
                                    //2. $('#spanAdministrators') = SPAN that will 'host' the people picker control
                                    //3. $('#inputAdministrators') = INPUT that will be used to capture user input
                                    //4. $('#divAdministratorsSearch') = DIV that will show the 'dropdown' of the people picker
                                    //5. $('#hdnAdministrators') = INPUT hidden control that will host a JSON string of the resolved users
                                    priOwners = new CAMControl.PeoplePicker(context, $('#spanAdministrators'), $('#inputAdministrators'), $('#divAdministratorsSearch'), $('#hdnAdministrators'));
                                    // required to pass the variable name here!
                                    priOwners.InstanceName = "priOwners";
                                    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                                    // Do not set the Language property if you do not have foreseen javascript resource file for your language
                                    priOwners.Language = spLanguage;
                                    // optionally show more/less entries in the people picker dropdown, 4 is the default
                                    priOwners.MaxEntriesShown = 5;
                                    // Can duplicate entries be selected (default = false)
                                    priOwners.AllowDuplicates = false;
                                    // Show the user loginname
                                    priOwners.ShowLoginName = true;
                                    // Show the user title
                                    priOwners.ShowTitle = true;
                                    // Set principal type to determine what is shown (default = 1, only users are resolved). 
                                    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                                    // Set ShowLoginName and ShowTitle to false if you're resolving groups
                                    priOwners.PrincipalType = 1;
                                    // start user resolving as of 2 entered characters (= default)
                                    priOwners.MinimalCharactersBeforeSearching = 3;
                                    // Hookup everything
                                    // priOwners.Initialize();

                                    secOwners = new CAMControl.PeoplePicker(context, $('#spansowner'), $('#inputsowner'), $('#divsownerSearch'), $('#hdnsowner'));
                                    secOwners.InstanceName = "secOwners";
                                    // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
                                    // Do not set the Language property if you do not have foreseen javascript resource file for your language
                                    secOwners.Language = spLanguage;
                                    // optionally show more/less entries in the people picker dropdown, 4 is the default
                                    secOwners.MaxEntriesShown = 5;
                                    // Can duplicate entries be selected (default = false)
                                    secOwners.AllowDuplicates = false;
                                    // Show the user loginname
                                    secOwners.ShowLoginName = true;
                                    // Show the user title
                                    secOwners.ShowTitle = true;
                                    // Set principal type to determine what is shown (default = 1, only users are resolved). 
                                    // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
                                    // Set ShowLoginName and ShowTitle to false if you're resolving groups
                                    secOwners.PrincipalType = 1;
                                    // start user resolving as of 2 entered characters (= default)
                                    secOwners.MinimalCharactersBeforeSearching = 3;
                                    // Hookup everything
                                    // secOwners.Initialize();

                                    $.ajax({
                                        type: "GET",
                                        url: JCI.CAM.SiteRequest.apiUrl.getFormData + "?" + JCI.CAM.SiteRequest.getStandardTokens(),
                                        success: function (resultdata) {
                                            JCI.CAM.SiteRequest.initFormData(resultdata);
                                            priOwners.Initialize();
                                            secOwners.Initialize();
                                            request.startDate(new Date().format('yyyy-MM-dd'));
                                            JCI.CAM.SiteRequest.loading('done');
                                        },
                                        error: function (error) {
                                            if (error.status !== 302 && error.status !== 409 && error.status !== 200) {
                                                // ToDo: need to process Form data request failure
                                                // alert(error.responseText + " " + error.statusText);
                                                // alert("error");
                                            }
                                        }
                                    });
                                });
                            });

                    });
            });
    });

    $('input').on('blur', function (e) {
        if (e == null) {
            e = event;
        }
        if (e.currentTarget.id == "endDate") {
            if (request.siteType() != "PROJECTSITE0")
            {
                request.endDate.clearError();
                return;
            }
            var pat = /[\d]+/;
            var dtPat = /[\d]{1,2}\/[\d]{1,2}\/[\d]{1,2}/;

            var endDate = $('#endDate ~ input').val();

            if (endDate != undefined && endDate != null && endDate != "") {
                if (!JCI.CAM.SiteRequest.checkDate(endDate)) {
                    request.endDate.setError("Please enter valid end date");
                    JCI.CAM.SiteRequest.datePrimaryCheck("NO");
                    JCI.CAM.SiteRequest.datePrimaryCheck.notifySubscribers("NO", 'change')
                    $('#endDateValid').show();
                    return;
                }
            }
            else
            {
                return;
            }

            var chunks = endDate.split('/');
            chunks[0] = parseInt(chunks[0]);
            chunks[1] = parseInt(chunks[1]);
            if (chunks[0] == 2 && chunks[1] > 29) {
                //request.endDate.setError("Please enter valid end date");
                JCI.CAM.SiteRequest.datePrimaryCheck("NO");
                JCI.CAM.SiteRequest.datePrimaryCheck.notifySubscribers("NO", 'change')
                $('#endDateValid').show();
                return;
            }
            if (endDate != "" && !pat.test(endDate)) {
                request.endDate.setError("Please enter valid end date");
                JCI.CAM.SiteRequest.datePrimaryCheck("NO");
                JCI.CAM.SiteRequest.datePrimaryCheck.notifySubscribers("NO", 'change')
                $('#endDateValid').show();
                return;
            }
            else if (endDate != "") {
                if (!dtPat.test(endDate)) {
                    request.endDate.setError("Please enter valid end date");
                    JCI.CAM.SiteRequest.datePrimaryCheck("NO");
                    JCI.CAM.SiteRequest.datePrimaryCheck.notifySubscribers("NO", 'change')
                    $('#endDateValid').show();
                    return;
                }
                endDate = new Date(endDate);
                if (endDate.toString() == "Invalid Date") {
                    request.endDate.setError("Please enter valid end date");
                    JCI.CAM.SiteRequest.datePrimaryCheck("NO");
                    JCI.CAM.SiteRequest.datePrimaryCheck.notifySubscribers("NO", 'change')
                    $('#endDateValid').show();
                    return;
                }
                else {
                    JCI.CAM.SiteRequest.datePrimaryCheck("OK");
                    JCI.CAM.SiteRequest.datePrimaryCheck.notifySubscribers("OK", 'change')
                    $('#endDateValid').hide();
                    return;
                }
            }
        }
    });

    $('input[type="date"]').on('change', function () {
        var val = $.prop(this, 'value');
        request.startDate($('#startDate').val());
        request.endDate($('#endDate').val());
        var startDate = $('#startDate').val();

        var re = /^([\d]{4})-([\d]{1,2})-([\d]{1,2})$/;
        var m = null;
        if ((re.exec(startDate)) !== null) {
            m = re.exec(startDate);
            var day = m[3];
            var year = m[1];
            var month = m[2]
            var startDate = month + "/" + day + "/" + year;
            if (!JCI.CAM.SiteRequest.checkDate(startDate))
            {
                request.startDate.setError("Please check start date value");
                return;
            }
        }

        startDate = new Date($('#startDate').val());
        var endDate = new Date($('#endDate').val());

        if (startDate.toString() == "Invalid Date") {
            request.startDate(null);
            request.startDate.setError("Please select project start date");
            return;
        }

        if (request.siteType() == "PROJECTSITE0") {
            if (endDate.toString() != "Invalid Date") {
                if (startDate.toString() == "Invalid Date") {
                    request.startDate.setError("Please select project start date");
                    return;
                }
                if (endDate < startDate) {
                    request.endDate.setError("End date cannot be earlier than Start Date");
                    return;
                }
                else {
                    request.endDate.clearError();
                }
            }
            else {
                if ($('#endDate').val() != "") {
                    request.endDate.setError("Invalid value for End date");
                    return;
                }
            }
        }
    });
};

JCI.CAM.SiteRequest.ApplySubscribers = function () {

    request.siteName.subscribe(function (change) {  // subscribe to site name to update site url
        if (request.siteURL() === "" || request.siteURL() == "") {
            var url = request.siteName();
            var pattern = /^[A-Z0-9a-z-_\s]+$/
            if (!pattern.test(url)) {
                return;
            }

            while (url.indexOf(" ", 0) > 0) {
                url = url.replace(" ", "");
            }
            request.siteURL(url);
            JCI.CAM.SiteRequest.validateSiteURL();
        }
    });

    request.primaryOwner.subscribe(function (change) {
        if (change != undefined) {
            var users = JSON.parse(change);
            if (users.length === 0) {
                request.primaryOwner.setError("At least one primary owner is required");
            }
            if (users.length > 1) {
                request.primaryOwner.setError("Only one primary owner can be specified");
            }
            else {
                $('#Administrators').val($('#hdnAdministrators').val());
            }

            if ($('#hdnAdministrators').val() == "") {
                $('#hdnAdministrators').val(request.primaryOwner());
            }
        }
    });

    request.startDate.subscribe(function (change) {
        if (change != undefined) {
            $('#startDate').val(change);
        }
    });
}

// function to get a parameter value by a specific key
function getQueryStringParameter(urlParameterKey) {
    var params = document.URL.split('?')[1].split('&');
    var strParams = '';
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split('=');
        if (singleParam[0] == urlParameterKey)
            return singleParam[1];
    }
}

// Function requiered by the People picker controls
function chromeLoaded() {
    $('body').show();
}

// Function callback to render chrome after SP.UI.Controls.js loads
function renderSPChrome() {
    // Set the chrome options for launching Help, Account, and Contact pages
    var options = {
        'appTitle': document.title,
        'onCssLoaded': 'chromeLoaded()'
    };

    // Load the Chrome Control in the divSPChrome element of the page
    // var chromeNavigation = new SP.UI.Controls.Navigation('divSPChrome', options);
    // chromeNavigation.setVisible(true);
}

$(document).ready(JCI.CAM.SiteRequest.loadHandler); // Register Page on load event handler
