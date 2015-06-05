
/*
    Document.Ready is an event that fires after the DOM is ready.  
    
    .inputWatermark method which will assign the "Sign..." to the search text box
    the ribbon is then moved into a new container called RibbonITems as is the status bar messages. 

    if the site is in edit mode a class is assigned to an element to mitigate the size of the ribbon.

*/

var JCIMasterJS = {};

JCIMasterJS.ready = function () {
    jQuery(document).ready(function () {

        HideElements();
        // jQuery('input#SearchText').inputWatermark();
        jQuery('#s4-ribbonrow').appendTo('#RibbonItems');
        jQuery('#DeltaPageStatusBar').appendTo('#Message');
        jQuery('.ms-cui-topBar2').addClass('NoBottomBorder');
        jQuery('#RibbonContainer-TabRowRight').css('display', 'block');

        var spacer = jQuery('#JCISpacer');

        if (isEditMode() == false) {
            //jQuery(spacer).removeClass('push2');
            //jQuery(spacer).addClass('push');
            HideElements();
            jQuery('.SearchArea2').show();
            jQuery('.ms-siteicon-img').show();

        }

        if (isEditMode() == true) {
            //jQuery(spacer).removeClass('push');
            //jQuery(spacer).addClass('push2');
            jQuery('.ms-siteicon-img').hide();
            jQuery('.SearchArea2').hide();
            jQuery('.ms-cui-topBar2').removeClass('NoBottomBorder');
            ShowElements();
        }

        jQuery('.ms-welcome-root').click(function () { hideMegaMenu(); });
        jQuery('.ms-siteactions-normal').click(function () { hideMegaMenu(); });
    });
};

function HideElements() {
    jQuery('.Messages').hide();
}

function ShowElements() {
    jQuery('.Messages').show();

}

function isEditMode() {
    var publishingEdit = window.g_disableCheckoutInEditMode;
    try {
        form = document.forms[MSOWebPartPageFormName];
        input = form.MSOLayout_InDesignMode || form._wikiPageMode;
    }
    catch (err) {
        return !!(publishingEdit || false);
    }
    return !!(publishingEdit || (input && input.value));
}

function hideMegaMenu() {
    jQuery(".secondLevelUL").css('display', 'none');
    jQuery('.has-sub').css('background-color', '#fff');
    jQuery('.has-sub1').css('background-color', '#fff');
    jQuery('#s4-bodyContainer').removeClass('OverflowXHidden');
}
