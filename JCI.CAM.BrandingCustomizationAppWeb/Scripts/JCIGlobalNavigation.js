//-----------------------------------------------------------------------
// <copyright file="JCIGlobalNavigation.js" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


var JCIGlobalNavigation = {};
JCIGlobalNavigation.Footer = {};
JCIGlobalNavigation.Header = {};
JCIGlobalNavigation.TopNavigation = {};

//Global variables
var context;
var session;
var storeId;
var navigationGroupId;
var termId;
var termStores;
var termStoresEnum;
var termSets;
var divFooterHeading;
var divFooterHeadingId;
var currentLcid;
var userDepartment;
var personAllProperties;
var configPopertyForAudienceTargetting = 'Department';
var configFooterMaxNumberOfLinksUnderOneHead = 5;
var configHeaderMaxNumberOfLinksUnderOneHead = 10;
var configTopNavigationMaxLinks = 6;
var configFooterMaxNumberOfColumns = 6;
var configHeaderMaxNumberOfColumns = 5;
var configTermStoreName = 'Managed Metadata Service';
var configNavigationGroupName = 'JCI.NavigationTest';
var configFooterTermSet = 'Footer';
var configHeaderTermSet = 'Header';
var configTopNavigationTermSet = 'TopNavigation';
var configFilterArray = [];
var configCorrespondigUserProfilePropertyArray = [];
var configLocalStorageExpirationInSeconds = 1800;
var configGlobalNavigationConfigurationListName = "GlobalNavigationConfiguration";
var configIsActiveTermFilterName = "IsActive";
var configOpenInNewWindowTermFilterName = "OpenInNewWindow";
var configUrlTermFilterName = "URL";
var configIsCopyrightTermFilterName = "IsCopyright";

// Footer specific variables
JCIGlobalNavigation.Footer.termSetId;
JCIGlobalNavigation.Footer.globalTermSetEnum;
JCIGlobalNavigation.Footer.footerNavigationItems;
JCIGlobalNavigation.Footer.userNavigationItems;
JCIGlobalNavigation.Footer.navigationTermObject;
JCIGlobalNavigation.Footer.navigationTermDataSource;
JCIGlobalNavigation.Footer.constantLocalStorageKeyForDataSource = 'JCINavigationFooterCachedTerms';
JCIGlobalNavigation.Footer.constantStorageKeyForDataSourceExpirationTime = 'JCINavigationFooterCachedTermsTime';

// Header specific variables
JCIGlobalNavigation.Header.termSetId;
JCIGlobalNavigation.Header.globalTermSetEnum;
JCIGlobalNavigation.Header.headerNavigationItems;
JCIGlobalNavigation.Header.userNavigationItems;
JCIGlobalNavigation.Header.navigationTermObject;
JCIGlobalNavigation.Header.navigationTermDataSource;
JCIGlobalNavigation.Header.constantLocalStorageKeyForDataSource = 'JCINavigationHeaderCachedTerms';
JCIGlobalNavigation.Header.constantStorageKeyForDataSourceExpirationTime = 'JCINavigationHeaderCachedTermsTime';

// TopNavigation specific variables
JCIGlobalNavigation.TopNavigation.termSetId;
JCIGlobalNavigation.TopNavigation.topNavigationItems;
JCIGlobalNavigation.TopNavigation.navigationTermDataSource;


var startTimeStamp;

/* 
    *On document ready  call showGlobalNavigation Method
*/
JCIGlobalNavigation.ready = function () {
    jQuery(document).ready(function () {
        startTimeStamp = new Date().getTime();
        JCIGlobalNavigation.logMessage("Start: :" + startTimeStamp);
        SP.SOD.executeOrDelayUntilScriptLoaded(function () { JCIGlobalNavigation.showGlobalNavigation(); }, 'SP.UserProfiles.js');

    });

    /*
    * Any click on the document should close the header fly out if any
    * @e - click event argument
*/
    jQuery(document).click(function (e) {
        try {
            if (e.target.id != 'mega-hold' && jQuery('.mega-hold').find(e.target).length == 0 && jQuery(e.target).attr('class') != 'JCIHeaderMainLinks') {
                jQuery(".secondLevelUL").css('display', 'none');
                jQuery('.has-sub').css('background-color', '#fff');
                jQuery('.has-sub1').css('background-color', '#fff');
                jQuery('#s4-bodyContainer').removeClass('OverflowXHidden');
            }
        }
        catch (err) {
            JCIGlobalNavigation.logMessage('Error in document.click event' + err.message);
        }

    });
};

/* 
   * Get rendering options and call method to fetch configuration data from list  
*/
JCIGlobalNavigation.showGlobalNavigation = function () {
    try {
        var rootSiteUrl = JCIGlobalNavigation.getRootSiteUrl();
        if (rootSiteUrl) {
            JCIGlobalNavigation.getGlobalNavigationConfiguration(rootSiteUrl, configGlobalNavigationConfigurationListName, _spPageContextInfo.siteAbsoluteUrl);
        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.showGlobalNavigation :' + err.message);
    }
}


/*  
   * Gets configuration from global configuration lists ( exists only at root site collection of the web application)
   * @siteUrl - url of the site collection where global configuration list is hosted
*/
JCIGlobalNavigation.getGlobalNavigationConfiguration = function (siteUrl, listName, filterUrl) {
    var globalNavigationDefaultConfigurationObject;
    var globalNavigationUserConfigurationObject;
    jQuery.ajax({
        url: siteUrl + "/_api/web/lists/getbytitle('" + listName + "')/Items",
        type: "GET",
        headers: { "accept": "application/json;odata=verbose" },
        success: function (data) {
            if (data.d.results) {
                var items = data.d.results;
                jQuery.each(items, function (i, item) {
                    if (item.Title.toString().toUpperCase() == filterUrl.toString().toUpperCase()) {
                        globalNavigationUserConfigurationObject = {
                            portalUrl: item.Title,
                            termStoreName: item.TermStoreName,
                            termGroupName: item.TermGroupName,
                            termHeaderName: item.TermHeaderName,
                            termFooterName: item.TermFooterName,
                            termTopNavigationName: item.TermTopNavigationName,
                            profileProperties: item.ProfileProperties,
                            termProperties: item.TermProperties,
                            maxLinksPerHeadingFooter: item.FooterMaxLinks,
                            maxLinksPerHeadingHeader: item.HeaderMaxLinks,
                            maxHeaderColumns: item.MaxHeaderColumns,
                            maxFooterColumns: item.MaxFooterColumns,
                            maxTopNavigationLinks: item.TopNavigationMaxLinks

                        };

                    } else if (item.DefaultNavigationSettings) {
                        globalNavigationDefaultConfigurationObject = {
                            portalUrl: item.Title,
                            termStoreName: item.TermStoreName,
                            termGroupName: item.TermGroupName,
                            termHeaderName: item.TermHeaderName,
                            termFooterName: item.TermFooterName,
                            termTopNavigationName: item.TermTopNavigationName,
                            profileProperties: item.ProfileProperties,
                            termProperties: item.TermProperties,
                            maxLinksPerHeadingFooter: item.FooterMaxLinks,
                            maxLinksPerHeadingHeader: item.HeaderMaxLinks,
                            maxHeaderColumns: item.MaxHeaderColumns,
                            maxFooterColumns: item.MaxFooterColumns,
                            maxTopNavigationLinks: item.TopNavigationMaxLinks

                        };
                    }
                });
            }
            if (globalNavigationUserConfigurationObject) {
                JCIGlobalNavigation.setConfigurationsFromList(globalNavigationUserConfigurationObject);
            }
            else {
                JCIGlobalNavigation.setConfigurationsFromList(globalNavigationDefaultConfigurationObject);
            }



        },
        error: function (err) {
            JCIGlobalNavigation.logMessage('Error while fetching configuration information from list' + err.statusText);
        }
    });
}


/*
    * Assigns list configuration to js variables
    * @listConfiguration - Configuration object which holds all the configuration values for global navigation
*/
JCIGlobalNavigation.setConfigurationsFromList = function (listConfiguration) {
    try {
        configTermStoreName = listConfiguration.termStoreName;
        configNavigationGroupName = listConfiguration.termGroupName;
        configHeaderTermSet = JCIGlobalNavigation.extractTermSetId(listConfiguration.termHeaderName, "Header");
        configFooterTermSet = JCIGlobalNavigation.extractTermSetId(listConfiguration.termFooterName, "Footer");
        configTopNavigationTermSet = JCIGlobalNavigation.extractTermSetId(listConfiguration.termTopNavigationName, "TopNavigation");

        if (listConfiguration.profileProperties) {
            configCorrespondigUserProfilePropertyArray = listConfiguration.profileProperties.split(',');
        }


        if (listConfiguration.termProperties) {
            configFilterArray = listConfiguration.termProperties.split(',');
        }

        configFooterMaxNumberOfLinksUnderOneHead = listConfiguration.maxLinksPerHeadingFooter;
        configHeaderMaxNumberOfLinksUnderOneHead = listConfiguration.maxLinksPerHeadingHeader;
        configHeaderMaxNumberOfColumns = listConfiguration.maxHeaderColumns
        configFooterMaxNumberOfColumns = listConfiguration.maxFooterColumns;
        configTopNavigationMaxLinks = listConfiguration.maxTopNavigationLinks;

        if (JCIGlobalNavigation.isHtml5StorageSupported()) {
            if (JCIGlobalNavigation.checkFromLocalStorage()) {
                // Method will render the navigation, based on navigation setting from local storage cached items 
            }
            else {
                SP.SOD.executeFunc('SP.UserProfiles.js', 'SP.UserProfiles', JCIGlobalNavigation.getUserProperties);
            }
        }
        else {
            SP.SOD.executeFunc('SP.UserProfiles.js', 'SP.UserProfiles', JCIGlobalNavigation.getUserProperties);
        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.setConfigurationsFromList : ' + err.message);
    }

}


JCIGlobalNavigation.extractTermSetId = function (termSetNameAndId, navigtaionType) {
    var termSetId = null;
    if (termSetNameAndId) {
        if (termSetNameAndId.indexOf('|') > 0) {
            var splittedArray = termSetNameAndId.split('|');
            if (splittedArray.length == 2) {
                termSetId = splittedArray[1].trim();
            }
            else {
                JCIGlobalNavigation.logMessage(navigtaionType + ' termset name is not specified correctly. Please specify name and guid separated by one pipe symbol ');
            }

        }
        else {
            JCIGlobalNavigation.logMessage(navigtaionType + ' termset name is not specified correctly. Please specify name and guid separated by pipe symbol ');
        }
    }
    else {
        JCIGlobalNavigation.logMessage(navigtaionType + ' termset details not mentioned in global configuration list');
    }
    return termSetId;
}

/* 
    * Gets current logged in user's profile properties
*/
JCIGlobalNavigation.getUserProperties = function () {


    // Get the current client context and PeopleManager instance.

    context = SP.ClientContext.get_current();
    
    var peopleManager = new SP.UserProfiles.PeopleManager(context);

    // Get user properties for the current logged in user.    
    personProperties = peopleManager.getMyProperties();

    // Load the PersonProperties object and send the request.
    context.load(personProperties);
    context.executeQueryAsync(JCIGlobalNavigation.onUserPropertiesSuccess, JCIGlobalNavigation.onUserPropertiesFail);
}



/*
    * Success handler for user properties async call
*/
JCIGlobalNavigation.onUserPropertiesSuccess = function () {

    personAllProperties = personProperties.get_userProfileProperties();
    userDepartment = personAllProperties[configPopertyForAudienceTargetting];
    SP.SOD.executeOrDelayUntilScriptLoaded(JCIGlobalNavigation.renderGlobalNavigationBasedOnSetting, 'SP.Taxonomy.js');
}


/*
    * Failure handler for user properties async call
*/
JCIGlobalNavigation.onUserPropertiesFail = function (sender, args) {

    JCIGlobalNavigation.logMessage('Error while fetching user profile information for the current user' + args.get_message());

}



/*
    * Checks the local storage 
*/
/*JCIGlobalNavigation.checkFromLocalStorage = function () {
    try {
        if (configGlobalNavigationRenderingOption.toString().toUpperCase() == "HEADER") {
            if (sessionStorage.getItem(JCIGlobalNavigation.Header.constantLocalStorageKeyForDataSource) && sessionStorage.getItem(JCIGlobalNavigation.Header.constantStorageKeyForDataSourceExpirationTime) && (!JCIGlobalNavigation.Header.isLocalStorageExpired())) {
                JCIGlobalNavigation.Header.navigationTermDataSource = JSON.parse(sessionStorage.getItem(JCIGlobalNavigation.Header.constantLocalStorageKeyForDataSource));
                JCIGlobalNavigation.Header.ShowHeader();
                return true;
            }
            else {
                return false;
            }
        }
        else if (configGlobalNavigationRenderingOption.toString().toUpperCase() == "FOOTER") {
            if (sessionStorage.getItem(JCIGlobalNavigation.Footer.constantLocalStorageKeyForDataSource) && sessionStorage.getItem(JCIGlobalNavigation.Footer.constantStorageKeyForDataSourceExpirationTime) && (!JCIGlobalNavigation.Footer.isLocalStorageExpired())) {
                JCIGlobalNavigation.Footer.navigationTermDataSource = JSON.parse(sessionStorage.getItem(JCIGlobalNavigation.Footer.constantLocalStorageKeyForDataSource));
                JCIGlobalNavigation.Footer.ShowFooter();
                return true;
            }
            else {
                return false;
            }
        }
        else {
            if (sessionStorage.getItem(JCIGlobalNavigation.Header.constantLocalStorageKeyForDataSource) && sessionStorage.getItem(JCIGlobalNavigation.Header.constantStorageKeyForDataSourceExpirationTime) && (!JCIGlobalNavigation.Header.isLocalStorageExpired()) && sessionStorage.getItem(JCIGlobalNavigation.Footer.constantLocalStorageKeyForDataSource) && sessionStorage.getItem(JCIGlobalNavigation.Footer.constantStorageKeyForDataSourceExpirationTime) && (!JCIGlobalNavigation.Footer.isLocalStorageExpired())) {
                JCIGlobalNavigation.Header.navigationTermDataSource = JSON.parse(sessionStorage.getItem(JCIGlobalNavigation.Header.constantLocalStorageKeyForDataSource));
                JCIGlobalNavigation.Header.ShowHeader();
                JCIGlobalNavigation.Footer.navigationTermDataSource = JSON.parse(sessionStorage.getItem(JCIGlobalNavigation.Footer.constantLocalStorageKeyForDataSource));
                JCIGlobalNavigation.Footer.ShowFooter();
                return true;

            }
            else {
                return false;
            }

        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.checkFromLocalStorage' + err.message);
    }
}*/

/*
    * Based on setting get terms for navigation
*/
JCIGlobalNavigation.renderGlobalNavigationBasedOnSetting = function () {
    try {
        JCIGlobalNavigation.Footer.footerNavigationItems = new Array();
        JCIGlobalNavigation.Footer.userNavigationItems = new Array();
        JCIGlobalNavigation.Header.headerNavigationItems = new Array();
        JCIGlobalNavigation.Header.userNavigationItems = new Array();
        JCIGlobalNavigation.TopNavigation.topNavigationItems = new Array();


        if (!isEditMode()) {
            if (configHeaderTermSet) {
                JCIGlobalNavigation.Header.getTerms();
            }
            else {
                JCIGlobalNavigation.logMessage('Header termset id missing in configuartion list');
            }
        }


        /*if (!isEditMode()) { commented for showing footer terms in edit mode */
        if (configFooterTermSet) {
            JCIGlobalNavigation.Footer.getTerms();
        }
        else {
            JCIGlobalNavigation.logMessage('Footer termset id missing in configuartion list');
        }
        /* } */

        if (configTopNavigationTermSet) {
            JCIGlobalNavigation.TopNavigation.getTerms(configTopNavigationTermSet);
        }
        else {
            JCIGlobalNavigation.logMessage('TopNavigation termset id missing in configuartion list');
        }

    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.renderGlobalNavigationBasedOnSetting : ' + err.message);
    }
}

/*
    * Gets terms for term set
    * @terSetId - GUID of the term set
*/
JCIGlobalNavigation.Footer.getTerms = function () {

    //Current Taxonomy Session
    var taxSession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);

    //Term Stores
    var termStores = taxSession.get_termStores();

    //Name of the Term Store from which to get the Terms.
    var termStore = termStores.getByName(configTermStoreName);


    //GUID of Term Set from which to get the Terms.
    var termSet = termStore.getTermSet(configFooterTermSet);

    var terms = termSet.getAllTerms();

    context.load(terms);

    context.executeQueryAsync(function () {
        JCIGlobalNavigation.Footer.footerNavigationItems = JCIGlobalNavigation.createHierarchy(terms);
        JCIGlobalNavigation.Footer.filterNavigationForCurrentUser();

    }, function (sender, args) {

        JCIGlobalNavigation.logMessage('Failure in loading footer terms.');

    });


}

/*
    * Filters  navigation items for footer based on term properties mentioned 
    * in term store and the logged in user's profile properties
*/
JCIGlobalNavigation.Footer.filterNavigationForCurrentUser = function () {
    try {
        for (var footerNavigationItemCounter = 0, len = JCIGlobalNavigation.Footer.footerNavigationItems.length ; footerNavigationItemCounter < len ; footerNavigationItemCounter++) {
            var isUserCanSeeTheLink;
            var isUserCanSeeLinkWithAllFilters = true;;
            for (var termFilterCounter = 0 ; termFilterCounter < configFilterArray.length ; termFilterCounter++) {
                isUserCanSeeTheLink = false;
                var allBuisenessUnitsForTheNavigationItem = JCIGlobalNavigation.Footer.footerNavigationItems[footerNavigationItemCounter].get_customProperties()[configFilterArray[termFilterCounter]];
                if (allBuisenessUnitsForTheNavigationItem) {
                    if (isUserCanSeeLinkWithAllFilters) {
                        var arrayAllBusinessUnits = allBuisenessUnitsForTheNavigationItem.split(',');
                        for (var profilePropertiesCounter = 0 ; profilePropertiesCounter < arrayAllBusinessUnits.length; profilePropertiesCounter++) {
                            if (arrayAllBusinessUnits[profilePropertiesCounter]) {
                                if (!SP.ScriptUtility.isNullOrEmptyString(personAllProperties[configCorrespondigUserProfilePropertyArray[termFilterCounter]])) {
                                    if (arrayAllBusinessUnits[profilePropertiesCounter].toUpperCase() == personAllProperties[configCorrespondigUserProfilePropertyArray[termFilterCounter]].toUpperCase()) {
                                        isUserCanSeeTheLink = true;
                                        break;

                                    }
                                }
                                else {
                                    isUserCanSeeTheLink = false;
                                }
                            }

                        }
                        if (isUserCanSeeTheLink != true) {
                            isUserCanSeeTheLink = false;
                        }
                        isUserCanSeeLinkWithAllFilters = isUserCanSeeLinkWithAllFilters && isUserCanSeeTheLink;
                    }
                }
                else {
                    isUserCanSeeTheLink = true;
                    isUserCanSeeLinkWithAllFilters = isUserCanSeeLinkWithAllFilters && isUserCanSeeTheLink;
                }
            }

            var myCustomNavigationObject = {
                myNavigationTerm: JCIGlobalNavigation.Footer.footerNavigationItems[footerNavigationItemCounter],
                isVisibleByCurrentUser: isUserCanSeeLinkWithAllFilters,
                isActive: JCIGlobalNavigation.Footer.footerNavigationItems[footerNavigationItemCounter].get_customProperties()[configIsActiveTermFilterName],
            };
            JCIGlobalNavigation.Footer.userNavigationItems.push(myCustomNavigationObject);

        }

        JCIGlobalNavigation.Footer.createTermObject();
        JCIGlobalNavigation.Footer.ShowFooter();
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.Footer.filterNavigationForCurrentUser : ' + err.message);
    }
}



/*
    * Rendering logic for footer
*/
JCIGlobalNavigation.Footer.ShowFooter = function () {
    try {

        var divClearFix = document.createElement("div");
        divClearFix.setAttribute("class", "clearfix");

        var isVisibleByCurrentUser, isActive, headerLinkClassName;
        var couterForMaxLinksUnderOneHeading; couterForMaxcolumnsinFooter = 0;
        for (var termDataSourceCounter = 0 ; termDataSourceCounter < JCIGlobalNavigation.Footer.navigationTermDataSource.length ; termDataSourceCounter++) {

            isVisibleByCurrentUser = JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].isVisibleByCurrentUser;


            if (JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].isActive) {
                isActive = JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].isActive;
            }
            else {
                isActive = 1;
            }

            if (JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].customProperties[configIsCopyrightTermFilterName]) {
                if (JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].customProperties[configIsCopyrightTermFilterName] === "1") {
                    headerLinkClassName = 'copyrightLink';
                }
            }
            else {
                headerLinkClassName = 'JCIFooterLink';
            }

            if (JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].pathOfTerm.indexOf(';') == -1) {
                footerHeading = JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].termName;
                divFooterHeading = document.createElement("div");

                divFooterHeading.setAttribute("class", "span2 FooterColumn jCIFooterRootLevelHeadings");
                divFooterHeadingId = JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].termId;
                divFooterHeading.setAttribute("id", divFooterHeadingId);
                divFooterHeading.setAttribute("name", footerHeading);

                h2ForFooterHeading = document.createElement("h2");
                h2ForFooterHeading.setAttribute("class", "FooterColumnTitle SubTitles");
                var customPropertiesOfTerm = JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].customProperties;
                if (couterForMaxcolumnsinFooter < configFooterMaxNumberOfColumns) {
                    if (isVisibleByCurrentUser == true && isActive == 1) {
                        JCIGlobalNavigation.addLink(JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].termName, customPropertiesOfTerm[configUrlTermFilterName], customPropertiesOfTerm[configOpenInNewWindowTermFilterName], h2ForFooterHeading, headerLinkClassName);
                        divFooterHeading.appendChild(h2ForFooterHeading)
                        jQuery('#footer > .JCIContainer').append(divFooterHeading);
                        couterForMaxLinksUnderOneHeading = 0;
                        couterForMaxcolumnsinFooter++;
                    }
                    /*else {
                        JCIGlobalNavigation.addLink(JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].userLanguageLabel, '', customPropertiesOfTerm[configOpenInNewWindowTermFilterName], h2ForFooterHeading, headerLinkClassName);

                    }*/

                }

                //divFooterHeading.after(divClearFix);
            }

            else if (JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].pathOfTerm.indexOf(';') > -1 && JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].pathOfTerm.split(';').length === 2) {
                if (isVisibleByCurrentUser == true && isActive == 1 && couterForMaxLinksUnderOneHeading < configFooterMaxNumberOfLinksUnderOneHead) {
                    couterForMaxLinksUnderOneHeading++;
                    var footerLink = JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].termName;
                    var divFooterLink = document.createElement("div");
                    divFooterLink.setAttribute("class", "JCIFooterLinks");
                    footerLinkId = JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].termId;
                    divFooterLink.setAttribute("id", footerLinkId);
                    divFooterLink.setAttribute("name", footerLink);
                    var customPropertiesOfTermLink = JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].customProperties;
                    JCIGlobalNavigation.addLink(JCIGlobalNavigation.Footer.navigationTermDataSource[termDataSourceCounter].termName, customPropertiesOfTermLink[configUrlTermFilterName], customPropertiesOfTermLink[configOpenInNewWindowTermFilterName], divFooterLink);

                    /*jQuery('#' + divFooterHeadingId).append(divFooterLink); */
                    divFooterHeading.appendChild(divFooterLink);
                }
            }

        }
        JCIGlobalNavigation.logMessage("Footer Finshed in " + (startTimeStamp - (new Date().getTime())));

        if (JCIGlobalNavigation.isHtml5StorageSupported() && JCIGlobalNavigation.Footer.isLocalStorageExpired()) {
            sessionStorage.setItem(JCIGlobalNavigation.Footer.constantLocalStorageKeyForDataSource, JSON.stringify(JCIGlobalNavigation.Footer.navigationTermDataSource));
            sessionStorage.setItem(JCIGlobalNavigation.Footer.constantStorageKeyForDataSourceExpirationTime, Math.floor((new Date().getTime()) / 1000));

        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.Footer.ShowFooter : ' + err.message);
    }
}


/*
    * Create array of navigation object which will be used for rendering    
*/
JCIGlobalNavigation.Footer.createTermObject = function () {

    try {

        JCIGlobalNavigation.Footer.navigationTermDataSource = new Array();
        for (var userNavigationItemCounter = 0 ; userNavigationItemCounter < JCIGlobalNavigation.Footer.userNavigationItems.length ; userNavigationItemCounter++) {
            JCIGlobalNavigation.Footer.navigationTermObject = new Object();
            JCIGlobalNavigation.Footer.navigationTermObject.termId = JCIGlobalNavigation.Footer.userNavigationItems[userNavigationItemCounter].myNavigationTerm.get_id().toString();
            JCIGlobalNavigation.Footer.navigationTermObject.termName = JCIGlobalNavigation.Footer.userNavigationItems[userNavigationItemCounter].myNavigationTerm.get_name();
            JCIGlobalNavigation.Footer.navigationTermObject.customProperties = JCIGlobalNavigation.Footer.userNavigationItems[userNavigationItemCounter].myNavigationTerm.get_customProperties();
            JCIGlobalNavigation.Footer.navigationTermObject.pathOfTerm = JCIGlobalNavigation.Footer.userNavigationItems[userNavigationItemCounter].myNavigationTerm.get_pathOfTerm();
            JCIGlobalNavigation.Footer.navigationTermObject.isVisibleByCurrentUser = JCIGlobalNavigation.Footer.userNavigationItems[userNavigationItemCounter].isVisibleByCurrentUser;
            JCIGlobalNavigation.Footer.navigationTermObject.isActive = JCIGlobalNavigation.Footer.userNavigationItems[userNavigationItemCounter].isActive;
            JCIGlobalNavigation.Footer.navigationTermDataSource.push(JCIGlobalNavigation.Footer.navigationTermObject);

        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.Footer.createTermObject : ' + err.message);
    }
}







/*
    * Gets all terms for header
    * @termSetId - GUID of the term set for which terms needs to be fetched
*/
JCIGlobalNavigation.Header.getTerms = function () {

    //Current Taxonomy Session
    var taxSession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);

    //Term Stores
    var termStores = taxSession.get_termStores();

    //Name of the Term Store from which to get the Terms.
    var termStore = termStores.getByName(configTermStoreName);

    //GUID of Term Set from which to get the Terms.
    var termSet = termStore.getTermSet(configHeaderTermSet);

    var terms = termSet.getAllTerms();

    context.load(terms);

    context.executeQueryAsync(function () {
        JCIGlobalNavigation.Header.headerNavigationItems = JCIGlobalNavigation.createHierarchy(terms);
        JCIGlobalNavigation.Header.filterNavigationForCurrentUser();


    }, function (sender, args) {

        JCIGlobalNavigation.logMessage('Failure in loading header terms.');


    });


}


/* 
    * Creates hierarchy from all term fetched by getTerms method
    * @allTerms - all terms of a term set without any sorting and hierarchy
*/
JCIGlobalNavigation.createHierarchy = function (allTerms) {
    try {
        var firstLevelHeading = JCIGlobalNavigation.getAllFirstLevelChildren(allTerms);
        var headerTerms = new Array();;
        JCIGlobalNavigation.getAllChildren(firstLevelHeading, allTerms, 2, headerTerms);

        return headerTerms;
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.createHierarchy : ' + err.message);
    }

}

/* 
    * Gets first level children from all terms of a term set
    * @allTerms - all terms of a term set without any sorting and hierarchy
*/
JCIGlobalNavigation.getAllFirstLevelChildren = function (allTerms) {
    try {
        var firstLevelItems = new Array();
        for (var counter = 0; counter < allTerms.get_count() ; counter++) {
            var currentTerm = allTerms.getItemAtIndex(counter);
            if (currentTerm.get_isRoot()) {
                termWithProcessing = new Object();
                termWithProcessing.term = allTerms.getItemAtIndex(counter);
                termWithProcessing.isProcessed = 0;
                firstLevelItems.push(termWithProcessing);
            }
        }
        return firstLevelItems;
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.getAllFirstLevelChildren : ' + err.message);
    }
}


/*
    * For each first level child  get all children 
    * @firstLevelItems  - only first level terms for the term set 
    * @allTerms - all terms of a term set without any sorting and hierarchy
    * @level - the depth in the tree, used in recursion 
    * @returnTerms -Final Hierarchical structure for the terms
*/
JCIGlobalNavigation.getAllChildren = function (firstLevelItems, allTerms, level, returnTerms) {
    try {
        var sortedHeader = new Array();
        var termWithProcessing = {};
        for (var counter = 0; counter < firstLevelItems.length ; counter++) {
            if (firstLevelItems[counter].isProcessed == 1) {
                sortedHeader.push(firstLevelItems[counter]);
            }

            else {
                if (firstLevelItems[counter].term.get_termsCount() > 0 && (firstLevelItems[counter].isProcessed == 0)) {
                    termWithProcessing = new Object();
                    termWithProcessing.term = firstLevelItems[counter].term;
                    termWithProcessing.isProcessed = 1;
                    sortedHeader.push(termWithProcessing);
                    if (!firstLevelItems[counter].term.get_customSortOrder()) {
                        var children = JCIGlobalNavigation.getChildrenFromPathAtLevel(allTerms, firstLevelItems[counter].term.get_pathOfTerm(), level);
                        for (var childrenCounter = 0; childrenCounter < children.length ; childrenCounter++) {
                            termWithProcessing = new Object();
                            termWithProcessing.term = children[childrenCounter];
                            termWithProcessing.isProcessed = 0;
                            sortedHeader.push(termWithProcessing);
                        }
                    }
                    else {
                        /* Sometime MMS does not return proper sort order , so to handle that scenario we need to validte the  
                            sort order GUIDs with children of the term */

                        var childrenGuidsWithSortOrder = firstLevelItems[counter].term.get_customSortOrder().split(':');
                        var childrenWithoutSortOrder = JCIGlobalNavigation.getChildrenFromPathAtLevel(allTerms, firstLevelItems[counter].term.get_pathOfTerm(), level);
                        var childrenGuidsWithoutSortOrder = new Array();
                        for (var childrenWithoutSortOrderCounter = 0; childrenWithoutSortOrderCounter < childrenWithoutSortOrder.length; childrenWithoutSortOrderCounter++) {
                            childrenGuidsWithoutSortOrder.push(childrenWithoutSortOrder[childrenWithoutSortOrderCounter].get_id().toString());
                        }
                        var isSortOrderCorrect = true;

                        if (childrenGuidsWithSortOrder.length == childrenGuidsWithoutSortOrder.length) {
                            for (var counter1 = 0; counter1 < childrenGuidsWithSortOrder.length ; counter1++) {
                                if (jQuery.inArray(childrenGuidsWithSortOrder[counter1], childrenGuidsWithoutSortOrder) >= 0) {
                                }
                                else {
                                    JCIGlobalNavigation.logMessage("MMS custom sort order incorrect for term  : " + firstLevelItems[counter].term.get_name());
                                    isSortOrderCorrect = false;
                                    break;
                                }
                            }
                        }
                        else {
                            JCIGlobalNavigation.logMessage("MMS custom sort order incorrect for term  : " + firstLevelItems[counter].term.get_name());
                            isSortOrderCorrect = false;
                        }

                        if (isSortOrderCorrect) {

                            for (var childrenCounter = 0; childrenCounter < childrenGuidsWithSortOrder.length ; childrenCounter++) {
                                var newChild = JCIGlobalNavigation.getTermFromid(allTerms, childrenGuidsWithSortOrder[childrenCounter].toString());
                                termWithProcessing = new Object();
                                termWithProcessing.term = newChild;
                                termWithProcessing.isProcessed = 0;
                                sortedHeader.push(termWithProcessing);
                            }

                        }
                        else {
                            JCIGlobalNavigation.logMessage("Wrong custom sort order returned from MMS:" + childrenGuidsWithSortOrder);
                            JCIGlobalNavigation.logMessage("No sorting performed on term  : " + firstLevelItems[counter].term.get_name());


                            var children = JCIGlobalNavigation.getChildrenFromPathAtLevel(allTerms, firstLevelItems[counter].term.get_pathOfTerm(), level);
                            for (var childrenCounter = 0; childrenCounter < childrenWithoutSortOrder.length ; childrenCounter++) {
                                termWithProcessing = new Object();
                                termWithProcessing.term = childrenWithoutSortOrder[childrenCounter];
                                termWithProcessing.isProcessed = 0;
                                sortedHeader.push(termWithProcessing);
                            }
                        }

                    }
                }
                else {
                    termWithProcessing = new Object();
                    termWithProcessing.term = firstLevelItems[counter].term;
                    termWithProcessing.isProcessed = 1;
                    sortedHeader.push(termWithProcessing);
                }
            }
        }


        if (JCIGlobalNavigation.isArrayHasParent(sortedHeader)) {
            JCIGlobalNavigation.getAllChildren(sortedHeader, allTerms, (level + 1), returnTerms);
            //return sortedHeader;
        }
        else {
            for (counter = 0; counter < sortedHeader.length ; counter++) {
                returnTerms.push(sortedHeader[counter].term);
            }

        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.getAllChildren : ' + err.message);
    }

}


/* 
    * Checks whether the term has children or not
    * @currentSortedTerms  - in progress collection of currently sorted terms while creating the hierarchy
*/
JCIGlobalNavigation.isArrayHasParent = function (currentSortedTerms) {
    try {
        for (var counter = 0; counter < currentSortedTerms.length ; counter++) {
            if ((currentSortedTerms[counter].isProcessed == 0) && currentSortedTerms[counter].term.get_termsCount() > 0) {
                return true;
            }
        }
        return false;
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.isArrayHasParent : ' + err.message);
    }


}

/*
    * Gets term object from term id
    * @allTerms - all terms of a term set without any sorting and hierarchy
    * @termId - GUID of the term 
*/
JCIGlobalNavigation.getTermFromid = function (allTerms, termId) {
    try {
        for (var counter = 0; counter < allTerms.get_count() ; counter++) {
            var currentTerm = allTerms.getItemAtIndex(counter);
            if (currentTerm.get_id().toString().toUpperCase() == termId.toUpperCase()) {
                return currentTerm;
            }
        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.getTermFromid  : ' + err.message);
    }
}

/*
    * Get terms at specific level using path of the term
    * @allTerms - all terms of a term set without any sorting and hierarchy
    * @parentPath - Path of the parent term for which we need children
    * @level - Depth of the term in hierarchy 
*/
JCIGlobalNavigation.getChildrenFromPathAtLevel = function (allTerms, parentPath, level) {
    try {
        var childrenTerm = new Array();
        for (var counter = 0; counter < allTerms.get_count() ; counter++) {
            var currentTerm = allTerms.getItemAtIndex(counter);
            var path = currentTerm.get_pathOfTerm();
            if (path) {
                var pathLevels = path.split(';');
            }
            if (path.search(parentPath) >= 0 && pathLevels.length <= level && path.replace(parentPath, '').search(';') == 0) {
                if (parentPath === path) {
                }
                else {
                    childrenTerm.push(currentTerm);
                }
            }
        }
        return childrenTerm;
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.getChildrenFromPathAtLevel  : ' + err.message);
    }
}




/*
    * Filters navigation items for header for audience targeting
*/
JCIGlobalNavigation.Header.filterNavigationForCurrentUser = function () {
    try {
        for (var navigationItemCounter = 0, len = JCIGlobalNavigation.Header.headerNavigationItems.length ; navigationItemCounter < len ; navigationItemCounter++) {
            var isUserCanSeeTheLink;
            var isUserCanSeeLinkWithAllFilters = true;;
            for (var termPropertiesCounter = 0 ; termPropertiesCounter < configFilterArray.length ; termPropertiesCounter++) {
                isUserCanSeeTheLink = false;
                var allBuisenessUnitsForTheNavigationItem = JCIGlobalNavigation.Header.headerNavigationItems[navigationItemCounter].get_customProperties()[configFilterArray[termPropertiesCounter]];
                if (allBuisenessUnitsForTheNavigationItem) {
                    if (isUserCanSeeLinkWithAllFilters) {
                        var arrayAllBusinessUnits = allBuisenessUnitsForTheNavigationItem.split(',');
                        for (var profilePropertiesCounter = 0 ; profilePropertiesCounter < arrayAllBusinessUnits.length; profilePropertiesCounter++) {
                            if (arrayAllBusinessUnits[profilePropertiesCounter]) {
                                if (!SP.ScriptUtility.isNullOrEmptyString(personAllProperties[configCorrespondigUserProfilePropertyArray[termPropertiesCounter]])) {
                                    if (arrayAllBusinessUnits[profilePropertiesCounter].toUpperCase() == personAllProperties[configCorrespondigUserProfilePropertyArray[termPropertiesCounter]].toUpperCase()) {
                                        isUserCanSeeTheLink = true;
                                        break;

                                    }
                                }
                                else {
                                    isUserCanSeeTheLink = false;
                                }
                            }

                        }
                        if (isUserCanSeeTheLink != true) {
                            isUserCanSeeTheLink = false;
                        }
                        isUserCanSeeLinkWithAllFilters = isUserCanSeeLinkWithAllFilters && isUserCanSeeTheLink;
                    }
                }
                else {
                    isUserCanSeeTheLink = true;
                    isUserCanSeeLinkWithAllFilters = isUserCanSeeLinkWithAllFilters && isUserCanSeeTheLink;
                }
            }

            var myCustomNavigationObject = {
                myNavigationTerm: JCIGlobalNavigation.Header.headerNavigationItems[navigationItemCounter],
                isVisibleByCurrentUser: isUserCanSeeLinkWithAllFilters,
                isActive: JCIGlobalNavigation.Header.headerNavigationItems[navigationItemCounter].get_customProperties()[configIsActiveTermFilterName],
            };
            JCIGlobalNavigation.Header.userNavigationItems.push(myCustomNavigationObject);

        }

        JCIGlobalNavigation.Header.createTermObject();
        JCIGlobalNavigation.Header.ShowHeader();

    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.Header.filterNavigationForCurrentUser  : ' + err.message);
    }

}



/*
    * Rendering logic of mega menu
*/
JCIGlobalNavigation.Header.ShowHeader = function () {

    try {
        var isVisibleByCurrentUser, isActive;
        var customPropertiesOfTerm;
        var couterForMaxLinksUnderOneHeading, couterForMaxcolumnsinHeader = 0;
        var mainUl = document.createElement("ul");
        mainUl.setAttribute("class", 'headUl');
        var subUl;
        var headerTermLevel1Id, headerTermLeve2Id, liHeaderTermLevel1;
        var divMegahold, divHeadNavalign, divMegaSection;
        for (var navigationDataSourceItemCounter = 0 ; navigationDataSourceItemCounter < JCIGlobalNavigation.Header.navigationTermDataSource.length ; navigationDataSourceItemCounter++) {

            isVisibleByCurrentUser = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].isVisibleByCurrentUser;


            if (JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].isActive) {
                isActive = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].isActive;
            }
            else {
                isActive = 1;
            }

            if (JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].pathOfTerm.indexOf(';') === -1) {
                var headerTermLevel1Name = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termName;
                headerTermLevel1Id = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termId;
                liHeaderTermLevel1 = document.createElement("li");


                liHeaderTermLevel1.setAttribute("class", "has-sub");

                var onclickBehaviour = "javascript:JCIGlobalNavigation.Header.headerOnclick('" + headerTermLevel1Id + "');";

                jQuery(liHeaderTermLevel1).click(function (event) {
                    javascript: JCIGlobalNavigation.Header.headerOnclick(this.id, event);
                });


                liHeaderTermLevel1.setAttribute("id", headerTermLevel1Id);
                liHeaderTermLevel1.setAttribute("name", headerTermLevel1Name);
                customPropertiesOfTerm = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].customProperties;
                if (couterForMaxcolumnsinHeader < configHeaderMaxNumberOfColumns) {
                    if (isVisibleByCurrentUser == true && isActive == 1) {
                        JCIGlobalNavigation.addLink(JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termName, customPropertiesOfTerm[configUrlTermFilterName], customPropertiesOfTerm[configOpenInNewWindowTermFilterName], liHeaderTermLevel1, 'JCIHeaderMainLinks');
                        couterForMaxcolumnsinHeader++;
                        mainUl.appendChild(liHeaderTermLevel1);
                    }
                    /*else {
                        JCIGlobalNavigation.addLink(JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].userLanguageLabel, '', customPropertiesOfTerm[configOpenInNewWindowTermFilterName], liHeaderTermLevel1, 'JCIHeaderMainLinks');

                    }*/


                }

                if ((navigationDataSourceItemCounter + 1) < JCIGlobalNavigation.Header.navigationTermDataSource.length) {
                    if (JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter + 1].pathOfTerm.indexOf(';') > -1 && JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter + 1].pathOfTerm.split(';').length === 2) {

                        subUl = document.createElement("ul");

                        subUl.setAttribute("id", 'Sub' + headerTermLevel1Id);
                        subUl.setAttribute("class", 'secondLevelUL');
                        var headerTermLeve2Name = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termName;
                        headerTermLeve2Id = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termId;
                        var liHeaderTermLevel2 = document.createElement("li");
                        liHeaderTermLevel2.setAttribute("class", "secondLevelLi");

                        liHeaderTermLevel2.setAttribute("id", 'SubLI' + headerTermLeve2Id);
                        liHeaderTermLevel2.setAttribute("name", headerTermLeve2Name);

                        divMegahold = document.createElement("div");
                        divMegahold.setAttribute("class", "mega-hold");
                        divMegahold.setAttribute("id", "mega-hold");

                        divHeadNavalign = document.createElement("div");
                        divHeadNavalign.setAttribute("class", "headNavalign");
                        divMegahold.appendChild(divHeadNavalign);
                        liHeaderTermLevel2.appendChild(divMegahold);
                        subUl.appendChild(liHeaderTermLevel2);

                        liHeaderTermLevel1.appendChild(subUl);

                    }
                    else {
                        liHeaderTermLevel1.setAttribute("class", "has-sub1");
                    }
                }
                else {
                    liHeaderTermLevel1.setAttribute("class", "has-sub1");
                }
            }
            else if (JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].pathOfTerm.indexOf(';') > -1 && JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].pathOfTerm.split(';').length === 2) {

                divMegaSection = document.createElement("div");
                divMegaSection.setAttribute("class", "mega-section HeadNavSunHeadLink");

                customPropertiesOfTerm = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].customProperties;
                if (isVisibleByCurrentUser == true && isActive == 1) {
                    JCIGlobalNavigation.addLink(JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termName, customPropertiesOfTerm[configUrlTermFilterName], customPropertiesOfTerm[configOpenInNewWindowTermFilterName], divMegaSection, 'JCISubHeaderLinks');
                    divHeadNavalign.appendChild(divMegaSection);
                }
                /* else {
                    JCIGlobalNavigation.addLink(JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].userLanguageLabel, '', customPropertiesOfTerm[configOpenInNewWindowTermFilterName], divMegaSection, 'JCISubHeaderLinksInActive');

                }*/

                couterForMaxLinksUnderOneHeading = 0;
            }
            else if (JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].pathOfTerm.indexOf(';') > -1 && JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].pathOfTerm.split(';').length === 3) {


                var headerTermLeve3Name = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termName;
                var headerTermLeve3Id = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termId;
                var liHeaderTermLevel3 = document.createElement("div");


                liHeaderTermLevel3.setAttribute("class", "");

                liHeaderTermLevel3.setAttribute("id", headerTermLeve2Id);
                liHeaderTermLevel3.setAttribute("name", headerTermLeve2Name);
                customPropertiesOfTerm = JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].customProperties;
                if (isVisibleByCurrentUser == true && isActive == 1 && couterForMaxLinksUnderOneHeading < configHeaderMaxNumberOfLinksUnderOneHead) {
                    couterForMaxLinksUnderOneHeading++;
                    JCIGlobalNavigation.addLink(JCIGlobalNavigation.Header.navigationTermDataSource[navigationDataSourceItemCounter].termName, customPropertiesOfTerm[configUrlTermFilterName], customPropertiesOfTerm[configOpenInNewWindowTermFilterName], liHeaderTermLevel3, 'JCIHeaderLinks');
                }

                divMegaSection.appendChild(liHeaderTermLevel3);
            }
        }
        jQuery('#HeaderNav').append(mainUl);
        JCIGlobalNavigation.Header.HeaderCheckForNoChildren();
        JCIGlobalNavigation.logMessage("Mega Menu Finshed in " + (startTimeStamp - (new Date().getTime())));

        if (JCIGlobalNavigation.isHtml5StorageSupported() && JCIGlobalNavigation.Header.isLocalStorageExpired()) {
            sessionStorage.setItem(JCIGlobalNavigation.Header.constantLocalStorageKeyForDataSource, JSON.stringify(JCIGlobalNavigation.Header.navigationTermDataSource));
            sessionStorage.setItem(JCIGlobalNavigation.Header.constantStorageKeyForDataSourceExpirationTime, Math.floor((new Date().getTime()) / 1000));


        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.Header.ShowHeader  : ' + err.message);
    }

}


/*
    *  Processing the header terms to check
               *  Any parent term with no children and apply appropriate changes
*/
JCIGlobalNavigation.Header.HeaderCheckForNoChildren = function () {
    try {
        jQuery('#HeaderNav .headNavalign').each(function () {
            if (jQuery(this).children().length == 0) {
                jQuery(this).parents('li').parents('li').removeClass('has-sub').addClass('has-sub1');
                jQuery(this).closest('ul').remove();
            }
        }
                              );

        jQuery('#HeaderNav >ul .has-sub1 >a').each(function () {
            if (jQuery(this).attr('href')) {
                jQuery(this).removeClass('JCIHeaderMainLinks').addClass('JCIHeaderSingleMainLinks');
            }
            else {
                jQuery(this).css('cursor', 'auto');
            }
        }
                              );

        jQuery('#HeaderNav >ul .has-sub >a').each(function () {
            if (jQuery(this).attr('href')) {
                jQuery(this).removeAttr('href');
            }
        }
                              );
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.Header.HeaderCheckForNoChildren  : ' + err.message);
    }

}

/*
    * Create objects which is used for rendering header
*/
JCIGlobalNavigation.Header.createTermObject = function () {

    try {

        JCIGlobalNavigation.Header.navigationTermDataSource = new Array();

        for (var navigationItemCounter = 0 ; navigationItemCounter < JCIGlobalNavigation.Header.userNavigationItems.length ; navigationItemCounter++) {
            JCIGlobalNavigation.Header.navigationTermObject = new Object();
            JCIGlobalNavigation.Header.navigationTermObject.termId = JCIGlobalNavigation.Header.userNavigationItems[navigationItemCounter].myNavigationTerm.get_id().toString();
            JCIGlobalNavigation.Header.navigationTermObject.termName = JCIGlobalNavigation.Header.userNavigationItems[navigationItemCounter].myNavigationTerm.get_name();
            JCIGlobalNavigation.Header.navigationTermObject.customProperties = JCIGlobalNavigation.Header.userNavigationItems[navigationItemCounter].myNavigationTerm.get_customProperties();
            JCIGlobalNavigation.Header.navigationTermObject.pathOfTerm = JCIGlobalNavigation.Header.userNavigationItems[navigationItemCounter].myNavigationTerm.get_pathOfTerm();
            JCIGlobalNavigation.Header.navigationTermObject.isVisibleByCurrentUser = JCIGlobalNavigation.Header.userNavigationItems[navigationItemCounter].isVisibleByCurrentUser;
            JCIGlobalNavigation.Header.navigationTermObject.isActive = JCIGlobalNavigation.Header.userNavigationItems[navigationItemCounter].isActive;
            JCIGlobalNavigation.Header.navigationTermDataSource.push(JCIGlobalNavigation.Header.navigationTermObject);

        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.Header.createTermObject  : ' + err.message);
    }
}


/*
    * Handles on click of the first level header items
    * @headerTermLevel1Id - Header main menu item id for which click is triggered
    * @e - Click event argument
*/
JCIGlobalNavigation.Header.headerOnclick = function (headerTermLevel1Id, e) {
    try {
        if (e.target.tagName.toUpperCase() == 'A') {
            var currentLi = jQuery("#" + headerTermLevel1Id);
            jQuery('.has-sub1').css('background-color', '#fff');
            jQuery('.has-sub').not(currentLi).each(function () {
                jQuery(this).css('background-color', '#fff');
            });

            var highLightedItem = jQuery("#" + headerTermLevel1Id).children('ul');
            jQuery('.secondLevelUL').not(highLightedItem).each(function () {

                jQuery(this).css('display', 'none');
            });

            //JCIGlobalNavigation.Header.resizeHeaderExpando();
            jQuery(highLightedItem).toggle();
            var currentBgColor = jQuery("#" + headerTermLevel1Id).css('background-color');

            if (currentBgColor == 'rgb(204, 204, 204)') {
                jQuery("#" + headerTermLevel1Id).css('background-color', '#fff');
            }
            else {
                jQuery("#" + headerTermLevel1Id).css('background-color', '#cccccc');
                jQuery('#s4-bodyContainer').addClass('OverflowXHidden');
            }
        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.Header.headerOnclick : ' + err.message);
    }


}


/*
               * Resizes the mega-hold div width to match the width of the window 
               * reassigns left-margin to align the text
*/
JCIGlobalNavigation.Header.resizeHeaderExpando = function () {
    var windowWidth = jQuery(window).width();
    jQuery('.mega-hold').css('width', windowWidth + 'px');
    var mainHeaderLeft = jQuery('.HeaderNavHold').position().left + 20;
    jQuery('.mega-hold').css('left', '-' + mainHeaderLeft + 'px');
    jQuery('.headNavalign').css('margin-left', mainHeaderLeft + 'px');

}


/*
    * Gets all terms for top navigation
    * @termSetId - GUID of the term set for which terms needs to be fetched
*/
JCIGlobalNavigation.TopNavigation.getTerms = function () {

    //Current Taxonomy Session
    var taxSession = SP.Taxonomy.TaxonomySession.getTaxonomySession(context);

    //Term Stores
    var termStores = taxSession.get_termStores();

    //Name of the Term Store from which to get the Terms.
    var termStore = termStores.getByName(configTermStoreName);


    //GUID of Term Set from which to get the Terms.
    var termSet = termStore.getTermSet(configTopNavigationTermSet);

    var terms = termSet.getAllTerms();

    context.load(terms);

    context.executeQueryAsync(function () {
        JCIGlobalNavigation.TopNavigation.prepareTopNavigationObject(terms);


    }, function (sender, args) {

        JCIGlobalNavigation.logMessage('Failure in loading top navigation terms.');

    });


}

JCIGlobalNavigation.TopNavigation.prepareTopNavigationObject = function (allTerms) {
    for (var topNavigationItemsCounter = 0; topNavigationItemsCounter < allTerms.get_count() ; topNavigationItemsCounter++) {
        var myCustomNavigationObject = {
            myNavigationTerm: allTerms.getItemAtIndex(topNavigationItemsCounter),

        };
        JCIGlobalNavigation.TopNavigation.topNavigationItems.push(myCustomNavigationObject);
    }

    // Filter top navigation for current user
    JCIGlobalNavigation.TopNavigation.filterNavigationForCurrentUser();
    JCIGlobalNavigation.TopNavigation.createTopNavigationObject();
    JCIGlobalNavigation.TopNavigation.ShowTopNavigation();
}

/*  Filter navigation links for current user with respect to user profile and term properties
    Also show only top level terms in navigation
*/
JCIGlobalNavigation.TopNavigation.filterNavigationForCurrentUser = function () {
    try {
        var userNavigationItems = new Array();
        for (var navigationItemCounter = 0; navigationItemCounter < JCIGlobalNavigation.TopNavigation.topNavigationItems.length ; navigationItemCounter++) {
            var isUserCanSeeTheLink;
            var isUserCanSeeLinkWithAllFilters = true;;
            for (var termPropertiesCounter = 0 ; termPropertiesCounter < configFilterArray.length ; termPropertiesCounter++) {
                isUserCanSeeTheLink = false;
                var allBuisenessUnitsForTheNavigationItem = JCIGlobalNavigation.TopNavigation.topNavigationItems[navigationItemCounter].myNavigationTerm.get_customProperties()[configFilterArray[termPropertiesCounter]];
                if (allBuisenessUnitsForTheNavigationItem) {
                    if (isUserCanSeeLinkWithAllFilters) {
                        var arrayAllBusinessUnits = allBuisenessUnitsForTheNavigationItem.split(',');
                        for (var profilePropertiesCounter = 0 ; profilePropertiesCounter < arrayAllBusinessUnits.length; profilePropertiesCounter++) {
                            if (arrayAllBusinessUnits[profilePropertiesCounter]) {
                                if (!SP.ScriptUtility.isNullOrEmptyString(personAllProperties[configCorrespondigUserProfilePropertyArray[termPropertiesCounter]])) {
                                    if (arrayAllBusinessUnits[profilePropertiesCounter].toUpperCase() == personAllProperties[configCorrespondigUserProfilePropertyArray[termPropertiesCounter]].toUpperCase()) {
                                        isUserCanSeeTheLink = true;
                                        break;
                                    }
                                }
                                else {
                                    isUserCanSeeTheLink = false;
                                }
                            }
                        }
                        if (isUserCanSeeTheLink != true) {
                            isUserCanSeeTheLink = false;
                        }
                        isUserCanSeeLinkWithAllFilters = isUserCanSeeLinkWithAllFilters && isUserCanSeeTheLink;
                    }
                }
                else {
                    isUserCanSeeTheLink = true;
                    isUserCanSeeLinkWithAllFilters = isUserCanSeeLinkWithAllFilters && isUserCanSeeTheLink;
                }
            }
            // Add the term to top navigation only if term is root term
            if (isUserCanSeeLinkWithAllFilters == true && JCIGlobalNavigation.TopNavigation.topNavigationItems[navigationItemCounter].myNavigationTerm.get_isRoot()) {
                userNavigationItems.push(JCIGlobalNavigation.TopNavigation.topNavigationItems[navigationItemCounter]);
            }
        }
        JCIGlobalNavigation.TopNavigation.topNavigationItems = userNavigationItems;
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.TopNavigation.filterNavigationForCurrentUser  : ' + err.message);
    }

}

/*
    * Create objects which is used for rendering top navigation
*/
JCIGlobalNavigation.TopNavigation.createTopNavigationObject = function () {

    try {

        JCIGlobalNavigation.TopNavigation.navigationTermDataSource = new Array();

        for (var navigationItemCounter = 0 ; navigationItemCounter < JCIGlobalNavigation.TopNavigation.topNavigationItems.length ; navigationItemCounter++) {
            JCIGlobalNavigation.TopNavigation.navigationTermObject = new Object();
            JCIGlobalNavigation.TopNavigation.navigationTermObject.termId = JCIGlobalNavigation.TopNavigation.topNavigationItems[navigationItemCounter].myNavigationTerm.get_id().toString();
            JCIGlobalNavigation.TopNavigation.navigationTermObject.termName = JCIGlobalNavigation.TopNavigation.topNavigationItems[navigationItemCounter].myNavigationTerm.get_name();
            JCIGlobalNavigation.TopNavigation.navigationTermObject.customProperties = JCIGlobalNavigation.TopNavigation.topNavigationItems[navigationItemCounter].myNavigationTerm.get_customProperties();
            JCIGlobalNavigation.TopNavigation.navigationTermDataSource.push(JCIGlobalNavigation.TopNavigation.navigationTermObject);

        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.TopNavigation.createTopNavigationObject : ' + err.message);
    }
}



JCIGlobalNavigation.TopNavigation.ShowTopNavigation = function () {
    var counterTopNavLinks = 0;
    var isActive;
    var customPropertiesOfTerm;
    var topNavigationUl = document.createElement("ul");
    topNavigationUl.setAttribute("class", "TopNavigation");
    for (topNavigationCounter = 0 ; topNavigationCounter < JCIGlobalNavigation.TopNavigation.navigationTermDataSource.length ; topNavigationCounter++) {
        customPropertiesOfTerm = JCIGlobalNavigation.TopNavigation.navigationTermDataSource[topNavigationCounter].customProperties;
        if (customPropertiesOfTerm['IsActive']) {
            isActive = customPropertiesOfTerm['IsActive'];
        }
        else {
            isActive = "1";
        }


        if (isActive === "1" && counterTopNavLinks < configTopNavigationMaxLinks) {
            var topNavigationLi = document.createElement("li");
            topNavigationLi.setAttribute("id", JCIGlobalNavigation.TopNavigation.navigationTermDataSource[topNavigationCounter].termId);
            JCIGlobalNavigation.addLink(JCIGlobalNavigation.TopNavigation.navigationTermDataSource[topNavigationCounter].termName, customPropertiesOfTerm[configUrlTermFilterName],
            customPropertiesOfTerm[configOpenInNewWindowTermFilterName], topNavigationLi, 'JCITopNavigationLinks');
            topNavigationUl.appendChild(topNavigationLi);
            counterTopNavLinks++;
        }
    }

    jQuery('#TopNavigationCustom').prepend(topNavigationUl);
    JCIGlobalNavigation.logMessage("Top Navigation Finshed in " + (startTimeStamp - (new Date().getTime())));

}

/*
    * Checks whether browser supports html storage or not 
*/
JCIGlobalNavigation.isHtml5StorageSupported = function () {
    /* try {
         return 'sessionStorage' in window && window['sessionStorage'] !== null;
     } catch (e) {
         return false;
     }*/
    return false;
}


/*
    * Checks for expiry of the cached objects
*/
JCIGlobalNavigation.Footer.isLocalStorageExpired = function () {
    var currentTime = Math.floor((new Date().getTime()) / 1000);
    if (currentTime - sessionStorage.getItem(JCIGlobalNavigation.Footer.constantStorageKeyForDataSourceExpirationTime) > configLocalStorageExpirationInSeconds) {
        return true;
    }
    else {
        return false;
    }
}

/*
    * Checks for expiry of the cached objects
*/
JCIGlobalNavigation.Header.isLocalStorageExpired = function () {
    var currentTime = Math.floor((new Date().getTime()) / 1000);
    if (currentTime - sessionStorage.getItem(JCIGlobalNavigation.Header.constantStorageKeyForDataSourceExpirationTime) > configLocalStorageExpirationInSeconds) {
        return true;
    }
    else {
        return false;
    }
}

/*
    * Adds a link to the parent elements
    * @linkName - Link text 
    * @linkUrl - Href attribute for the link
    * @isNewWindow - Target attribute for the link ,only if value is 1  the target is set to be _blank
    * @parentElement - Parent element in which this link is going to be added
    * @className - Class name for the link
*/
JCIGlobalNavigation.addLink = function (linkName, linkUrl, isNewWindow, parentElement, className) {
    try {
        var newlink = document.createElement('a');
        // By default MMS stores ampersand as full width ampersand - Below code is added to handle full width ampersand stored in managed meta data
        // Logic to check if link Name contains full width Ampersand and replace it with normal ampersand
        if (linkName.indexOf("＆") != -1) {
            linkName = linkName.replace("＆", "&");
        }

        newlink.innerHTML = linkName;
        if (className) {
            jQuery(newlink).addClass(className);
        }
        else {
            jQuery(newlink).addClass('JCIFooterLink');
        }

        if (linkUrl) {
            newlink.setAttribute('href', linkUrl);
        }
        else if (className != 'JCIHeaderMainLinks') {
            jQuery(newlink).addClass('InActiveLinks');
        }

        if (isNewWindow == 1) {
            newlink.setAttribute('target', '_blank');
        }

        parentElement.appendChild(newlink);

    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.addLink : ' + err.message);
    }


}



/*
    * Gets the root site collection url
*/
JCIGlobalNavigation.getRootSiteUrl = function () {
    try {
        var absoluteUrl = _spPageContextInfo.webAbsoluteUrl;
        var relativeUrl = _spPageContextInfo.webServerRelativeUrl;
        if (relativeUrl == '/') {
            return absoluteUrl;
        }
        else {
            return absoluteUrl.substring(0, absoluteUrl.indexOf(relativeUrl));
        }
    }
    catch (err) {
        JCIGlobalNavigation.logMessage('Error in function JCIGlobalNavigation.getRootSiteUrl' + err.message);
    }
}

/*
    * Logs message to console after checking console object
*/
JCIGlobalNavigation.logMessage = function (messageToLog) {
    if (window.console) {
        window.console.log(messageToLog);
    }
}