// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageExtensions.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//  List Extensions
// </summary>
// -------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{    
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using JCI.CAM.Common.Entity;
    using JCI.CAM.Common.Utilities;
    using Microsoft.SharePoint.Client;
    using Microsoft.SharePoint.Client.WebParts;

    /// <summary>
    /// Page extensions
    /// </summary>
    public static partial class PageExtensions
    {
        /// <summary>
        /// List the web parts on a page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl">Server relative url of the page containing the web parts</param>
        /// <returns>Web part definition</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when serverRelativePageUrl is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when serverRelativePageUrl is a zero-length string or contains only white space</exception>
        public static IEnumerable<WebPartDefinition> GetWebParts(this Web web, string serverRelativePageUrl)
        {
            if (string.IsNullOrEmpty(serverRelativePageUrl))
            {
                throw (serverRelativePageUrl == null)
                  ? new ArgumentNullException("serverRelativePageUrl")
                  : new ArgumentException("Webpart server relative url is null or empty", "serverRelativePageUrl");
            }

            Microsoft.SharePoint.Client.File file = web.GetFileByServerRelativeUrl(serverRelativePageUrl);
            LimitedWebPartManager limitedWebPartManager = file.GetLimitedWebPartManager(PersonalizationScope.Shared);

            var query = web.Context.LoadQuery(limitedWebPartManager.WebParts.IncludeWithDefaultProperties(wp => wp.Id, wp => wp.WebPart, wp => wp.WebPart.Title, wp => wp.WebPart.Properties, wp => wp.WebPart.Hidden));

            web.Context.ExecuteQuery();

            return query;
        }

        /// <summary>
        /// Inserts a web part on a web part page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="webPart">Information about the web part to insert</param>
        /// <param name="page">Page to add the web part on</param>
        /// <exception cref="System.ArgumentException">Thrown when page is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when webPart or page is null</exception>
        public static void AddWebPartToWebPartPage(this Web web, WebPartEntity webPart, string page)
        {
            if (webPart == null)
            {
                throw new ArgumentNullException("webPart");
            }

            if (string.IsNullOrEmpty(page))
            {
                throw (page == null)
                  ? new ArgumentNullException("page")
                  : new ArgumentException("Webpart page is null or empty", "page");
            }

            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQuery();
            }

            var serverRelativeUrl = UrlUtility.Combine(web.ServerRelativeUrl, page);

            AddWebPartToWebPartPage(web, serverRelativeUrl, webPart);
        }

        /// <summary>
        /// Inserts a web part on a web part page
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="serverRelativePageUrl">Page to add the web part on</param>
        /// <param name="webPart">Information about the web part to insert</param>
        /// <exception cref="System.ArgumentException">Thrown when serverRelativePageUrl is a zero-length string or contains only white space</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when serverRelativePageUrl or webPart is null</exception>
        public static void AddWebPartToWebPartPage(this Web web, string serverRelativePageUrl, WebPartEntity webPart)
        {
            if (string.IsNullOrEmpty(serverRelativePageUrl))
            {
                throw (serverRelativePageUrl == null)
                  ? new ArgumentNullException("serverRelativePageUrl")
                  : new ArgumentException("Webpart server relative url is null or empty", "serverRelativePageUrl");
            }

            if (webPart == null)
            {
                throw new ArgumentNullException("webPart");
            }

            var webPartPage = web.GetFileByServerRelativeUrl(serverRelativePageUrl);

            if (webPartPage == null)
            {
                return;
            }

            web.Context.Load(webPartPage);
            web.Context.ExecuteQuery();

            LimitedWebPartManager limitedWebPartManager = webPartPage.GetLimitedWebPartManager(PersonalizationScope.Shared);
            WebPartDefinition webPartDefinition = limitedWebPartManager.ImportWebPart(webPart.WebPartXml);

            WebPartDefinition newWebPartDefinition = limitedWebPartManager.AddWebPart(webPartDefinition.WebPart, webPart.WebPartZone, webPart.WebPartIndex);
            web.Context.Load(newWebPartDefinition);
            web.Context.ExecuteQuery();

            if (string.Compare(webPart.WebPartZone, "wpz", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                web.AddWebPartToWikiPage(webPartPage, newWebPartDefinition, webPart);
            }
        }

        /// <summary>
        /// Adds the web part to wiki page.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="webPartPage">The web part page.</param>
        /// <param name="newWebPartDefinition">The new web part definition.</param>
        /// <param name="webPartEntity">The web part entity.</param>
        public static void AddWebPartToWikiPage(this Web web, File webPartPage, WebPartDefinition newWebPartDefinition, WebPartEntity webPartEntity)
        {
            if (webPartPage == null)
            {
                return;
            }

            web.Context.Load(webPartPage.ListItemAllFields);
            web.Context.ExecuteQuery();

            string wikiField = (string)webPartPage.ListItemAllFields["WikiField"];            

            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.LoadXml(wikiField);

            // Sometimes the wikifield content seems to be surrounded by an additional div? 
            XmlElement layoutsTable = xd.SelectSingleNode("div/div/table") as XmlElement;
            if (layoutsTable == null)
            {
                layoutsTable = xd.SelectSingleNode("div/table") as XmlElement;
            }

            XmlElement layoutsZoneInner = layoutsTable.SelectSingleNode(string.Format("tbody/tr[{0}]/td[{1}]/div/div", webPartEntity.Row, webPartEntity.Column)) as XmlElement;

            //// - space element
            ////XmlElement space = xd.CreateElement("p");
            ////XmlText text = xd.CreateTextNode(" ");
            ////space.AppendChild(text);

            //// - wpBoxDiv
            XmlElement webPartBoxDiv = xd.CreateElement("div");
            layoutsZoneInner.AppendChild(webPartBoxDiv);

            ////if (webPartEntity.AddSpace)
            ////{
            ////    layoutsZoneInner.AppendChild(space);
            ////}

            XmlAttribute attribute = xd.CreateAttribute("class");
            webPartBoxDiv.Attributes.Append(attribute);
            attribute.Value = "ms-rtestate-read ms-rte-wpbox";
            attribute = xd.CreateAttribute("contentEditable");
            webPartBoxDiv.Attributes.Append(attribute);
            attribute.Value = "false";
            //// - div1
            XmlElement div1 = xd.CreateElement("div");
            webPartBoxDiv.AppendChild(div1);
            div1.IsEmpty = false;
            attribute = xd.CreateAttribute("class");
            div1.Attributes.Append(attribute);
            attribute.Value = "ms-rtestate-read " + newWebPartDefinition.Id.ToString("D");
            attribute = xd.CreateAttribute("id");
            div1.Attributes.Append(attribute);
            attribute.Value = "div_" + newWebPartDefinition.Id.ToString("D");
            //// - div2
            XmlElement div2 = xd.CreateElement("div");
            webPartBoxDiv.AppendChild(div2);
            div2.IsEmpty = false;
            attribute = xd.CreateAttribute("style");
            div2.Attributes.Append(attribute);
            attribute.Value = "display:none";
            attribute = xd.CreateAttribute("id");
            div2.Attributes.Append(attribute);
            attribute.Value = "vid_" + newWebPartDefinition.Id.ToString("D");

            ListItem listItem = webPartPage.ListItemAllFields;
            listItem["WikiField"] = xd.OuterXml;
            listItem.Update();
            web.Context.ExecuteQuery();
        }
    }
}
