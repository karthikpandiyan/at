//-----------------------------------------------------------------------
// <copyright file= "HomeController.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.ListProvisioningAppWeb.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Xml.Serialization;
    using JCI.CAM.Common.AppModelExtensions;
    using JCI.CAM.Common.Logging;
    using JCI.CAM.Common.Models;
    using JCI.CAM.Common.SPHelpers;
    using JCI.CAM.ListProvisioningAppWeb.Models;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// Home controller
    /// </summary>
    [SharePointContextFilter]
    [Serializable]
    public class HomeController : Controller
    {
        /// <summary>
        /// Index this instance.
        /// </summary>
        /// <returns>
        /// Return view
        /// </returns>
        public ActionResult Index()
        {
            try
            {
                ListModel listModel = new ListModel { Templates = this.GetListTemplates() };
                return this.View(listModel);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.ExceptionHandling);
                ListModel listModel = new ListModel { Templates = this.GetListTemplates() };
                return this.LoadIndex(listModel, ex.Message);
            } 
        }

        /// <summary>
        /// Creates the specified list model.
        /// </summary>
        /// <param name="listModel">The list model.</param>
        /// <returns>
        /// Return view
        /// </returns>
        [HttpPost]
        public ActionResult Create(ListModel listModel)
        {
            string message = string.Empty;
            try
            {
                if (!ModelState.IsValid)
                {
                   return this.LoadIndex(listModel, "List view model is invalid.");
                }

                ListDefinition definition = this.GetListDefinitionByTemplate(listModel);
                if (definition == null)
                {
                    return this.LoadIndex(listModel, string.Format("{0} template not found.", listModel.TemplateTitle));
                }

                return this.ListProvision(listModel, definition);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex, LogEventID.InformationWrite);              
                return this.LoadIndex(listModel, ex.Message);
            }
        }

        /// <summary>
        /// Lists the provision.
        /// </summary>
        /// <param name="listModel">The list model.</param>
        /// <param name="definition">The definition.</param>
        /// <returns>Return result view</returns>
        private ActionResult ListProvision(ListModel listModel, ListDefinition definition)
        {
            var spcontext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var context = spcontext.CreateUserClientContextForSPHost())
            {
                if (context == null)
                {
                    return this.LoadIndex(listModel, "SharePoint context is null.");
                }

                context.Load(context.Site.RootWeb, r => r.ServerRelativeUrl);
                context.ExecuteQuery();
                Web rootWeb = context.Site.RootWeb;

                context.Load(context.Web, w => w.ServerRelativeUrl);
                context.ExecuteQuery();
                Web web = context.Web;

                if (!web.CheckCurrentUserAuthorization(PermissionKind.ManageLists))
                {
                    return this.LoadIndex(listModel, "You do not have enough rights to create list.");
                }

                if (web.ListExists(listModel.Title))
                {
                    return this.LoadIndex(listModel, "This list name is already existed. Please try with different list name.");
                }

                definition.Title = listModel.Title;
                definition.Description = listModel.Description;

                List newList = ListProvisionHelper.CreateList(web, rootWeb, definition);
                if (newList != null)
                {
                    context.Load(newList, l => l.DefaultViewUrl);
                    context.ExecuteQuery();

                    string hostSiteUrl = spcontext.SPHostUrl.AbsoluteUri;
                    hostSiteUrl = hostSiteUrl.Replace(web.ServerRelativeUrl, string.Empty).TrimEnd("/".ToCharArray());

                    return this.Redirect(string.Concat(hostSiteUrl, newList.DefaultViewUrl));
                }
                else
                {
                    return this.LoadIndex(listModel, "Failed to create new list.");
                }
            }
        }

        /// <summary>
        /// Loads the index.
        /// </summary>
        /// <param name="listModel">The list model.</param>
        /// <param name="message">The message.</param>
        /// <returns>Return index view</returns>
        private ActionResult LoadIndex(ListModel listModel, string message = "")
        {
            if (!string.IsNullOrEmpty(message))
            {
                ModelState.AddModelError("error", message);
            }

            listModel.Templates = this.GetListTemplates();
            return this.View("Index", listModel);
        }

        /// <summary>
        /// Gets the list definition by template.
        /// </summary>
        /// <param name="listModel">The list model.</param>
        /// <returns>
        /// Returns list definition
        /// </returns>
        private ListDefinition GetListDefinitionByTemplate(ListModel listModel)
        {
            ListDefinitions listDefinitions = this.GetListDefinitions();
            if (listDefinitions != null)
            {
                List<ListDefinition> listDefinitionColl = listDefinitions.List;

                return listDefinitionColl.FirstOrDefault(listDefinition => listDefinition.Title.ToLower() == listModel.TemplateTitle.ToLower());
            }

            return null;
        }

        /// <summary>
        /// Gets the list templates.
        /// </summary>
        /// <returns>
        /// Returns list of templates
        /// </returns>
        private List<string> GetListTemplates()
        {
            List<string> listTemplates = new List<string>();
            ListDefinitions listDefinitions = this.GetListDefinitions();
            List<ListDefinition> listColl = listDefinitions.List;
            
            foreach (var list in listColl)
            {
                listTemplates.Add(list.Title);
            }

            return listTemplates;
        }

        /// <summary>
        /// Get the list definition.
        /// </summary>
        /// <returns>
        /// Return the list definition
        /// </returns>
        private ListDefinitions GetListDefinitions()
        {
            ListDefinitions listDefinitions = new ListDefinitions();

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["ListDefinitionXmlLocation"]))
            {
                return listDefinitions;
            }

                string xmlLocation = ConfigurationManager.AppSettings["ListDefinitionXmlLocation"];
                string path = HttpContext.Server.MapPath(xmlLocation);
                XmlSerializer deserializer = new XmlSerializer(typeof(ListDefinitions));
                TextReader textReader = new System.IO.StreamReader(path);
                listDefinitions = (ListDefinitions)deserializer.Deserialize(textReader);
                textReader.Close();
                return listDefinitions;           
        }
    }
}