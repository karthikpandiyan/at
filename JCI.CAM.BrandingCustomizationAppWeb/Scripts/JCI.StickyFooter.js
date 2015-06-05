var StickyFooter = StickyFooter || {};

StickyFooter.AdjustFooter = function () {
    var footerHeight = StickyFooter.GetTotalHeightOfObject(document.getElementById("Footer"));
    var bodyHeight = StickyFooter.GetTotalHeightOfObject(document.getElementById("contentRow"));
    var ribbonHeight = StickyFooter.GetTotalHeightOfObject(document.getElementById("s4-ribbonrow"));
    var suiteBarHeight = StickyFooter.GetTotalHeightOfObject(document.getElementById("suiteBar"));
    var titleAreaHeight = StickyFooter.GetTotalHeightOfObject(document.getElementById("s4-titlerow"));
    var sideNavBoxHeight = StickyFooter.GetTotalHeightOfObject(document.getElementById("sideNavBox"));

    var OuterMainContainer = 46;

    if (typeof sideNavBoxHeight !== 'undefined' && sideNavBoxHeight > bodyHeight) {
        bodyHeight = sideNavBoxHeight;
    }

    if (typeof titleAreaHeight == 'undefined') {
        titleAreaHeight = 0;
    }

    if (typeof footerHeight !== undefined && footerHeight == 0) {
        footerHeight = 200;
    }

    var headerHeight = ribbonHeight + suiteBarHeight + titleAreaHeight + OuterMainContainer;
    var viewportHeight = document.documentElement.clientHeight;

    var totalBodyHeight = bodyHeight + headerHeight + footerHeight;
    if (viewportHeight > totalBodyHeight) {
        var adjustment = viewportHeight - totalBodyHeight;
        var newBodyHeight = adjustment + bodyHeight;
        document.getElementById("contentRow").style.height = newBodyHeight + "px";
    }
}

StickyFooter.OnPageLoad = function () {
    StickyFooter.AdjustFooter();
    window.addEventListener("resize", StickyFooter.AdjustFooter, false);
}

StickyFooter.GetTotalHeightOfObject = function (object) {
    if (object == null || object.length == 0) {
        return 0;
    }

    if (object.offsetHeight) {
        return object.offsetHeight;
    }
    else if (object.style.pixelHeight) {
        return object.style.pixelHeight;
    }
}

//_spBodyOnLoadFunctionNames.push("StickyFooter.OnPageLoad");