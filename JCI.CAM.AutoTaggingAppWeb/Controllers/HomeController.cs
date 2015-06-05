////-----------------------------------------------------------------------
//// <copyright file= "HomeController.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
//// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
//// All rights reserved.
//// </copyright>
////-----------------------------------------------------------------------

//namespace JCI.CAM.AutoTaggingAppWeb.Controllers
//{
//    using System.Web.Mvc;

//    /// <summary>
//    /// Controller class
//    /// </summary>  
//    public class HomeController : Controller
//    {
//        /// <summary>
//        /// Index this instance.
//        /// </summary>
//        /// <returns>Return view</returns>
//        public ActionResult Index()
//        {
//            return this.View();
//        }
//    }
//}

//-----------------------------------------------------------------------
// <copyright file= "HomeController.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.AutoTaggingAppWeb.Controllers
{
    using Microsoft.SharePoint.Client;
    using System;
    using System.Web.Mvc;
    using System.Xml.Linq;
    using System.Linq;
    using System.Security;
    /// <summary>
    /// Controller class
    /// </summary>  
    public class HomeController : Controller
    {
        XNamespace ns = "http://schemas.microsoft.com/sharepoint/";
        /// <summary>
        /// Index this instance.
        /// </summary>
        /// <returns>Return view</returns>
        [SharePointContextFilter]
        public ActionResult Index()
        {

            // var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);
            // var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var spContext = SharePointContextProvider.Current.GetSharePointContext( HttpContext);
             SecureString passWord = new SecureString();
 
    foreach (char c in "Change@15".ToCharArray()) passWord.AppendChar(c);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
              //  var cred = new SharePointOnlineCredentials("rajkumar@jcistage.onmicrosoft.com", passWord);

                try
                {
                    //////bool exist = false;
                    //////List _list = clientContext.Web.Lists.GetByTitle("doc");
                    //////if (DoesEventReceiverExistByName(clientContext, _list, "PnPAutoTaggingItemAddIng2"))
                    //////{
                    //////    exist = true;
                    //////}
                    //////else
                    //////{
                    //////    exist = false;
                    //////}
                    List _list = clientContext.Web.Lists.GetByTitle("Jci");
                    EventReceiverDefinitionCreationInformation _rec = CreateEventReciever("ItemAdding1", EventReceiverType.ItemAdding);
                 bool exist=   AddEventReceiver(clientContext, _list, _rec);
                   
                   // List _list = clientContext.Web.Lists.GetByTitle("doc");
                    //if (DoesEventReceiverExistByName(clientContext, _list, "PnPAutoTaggingItemAddIng1010"))
                    //{
                    //    exist = true;
                    //}
                    //else
                    //{
                    //    exist = false;
                    //}
                    ViewBag.Message = exist+ " - Event Receiver registered successfully!!!";
                }
                catch(Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
               
               /*
              
                clientContext.Load(clientContext.Web, web => web.UserCustomActions);
                clientContext.ExecuteQuery();

                // get the xml elements file and get the CommandUIExtension node
                var customActionNode = GetCustomActionXmlNode();
                var customActionName = customActionNode.Attribute("Id").Value;
                var commandUIExtensionNode = customActionNode.Element(ns + "CommandUIExtension");
                var xmlContent = commandUIExtensionNode.ToString();
                var location = customActionNode.Attribute("Location").Value;
                var registrationId = customActionNode.Attribute("RegistrationId").Value;
                var registrationTypeString = customActionNode.Attribute("RegistrationType").Value;
                var registrationType = (UserCustomActionRegistrationType)Enum.Parse(typeof(UserCustomActionRegistrationType), registrationTypeString);

                var sequence = 1000;
                if (customActionNode.Attribute(ns + "Sequence") != null)
                {
                    sequence = Convert.ToInt32(customActionNode.Attribute(ns + "Sequence").Value);
                }

                // see of the custom action already exists
                var customAction = clientContext.Web.UserCustomActions.FirstOrDefault(uca => uca.Name == customActionName);

                // if it does not exist, create it
                if (customAction == null)
                {
                    // create the ribbon
                    customAction = clientContext.Web.UserCustomActions.Add();
                    customAction.Name = customActionName;
                }

                // set custom action properties
                customAction.Location = location;
                customAction.CommandUIExtension = xmlContent; // CommandUIExtension xml
                customAction.RegistrationId = registrationId;
                customAction.RegistrationType = registrationType;
                customAction.Sequence = sequence;

                customAction.Update();
                clientContext.Load(customAction);
                clientContext.ExecuteQuery();
                */
            }
            return this.View();
        }

        XElement GetCustomActionXmlNode()
        {
            var filePath = Server.MapPath("~/Resources/RibbonCommands.xml");
            var xdoc = XDocument.Load(filePath);
            var customActionNode = xdoc.Element(ns + "Elements").Element(ns + "CustomAction");
            return customActionNode;
        }

        /// <summary>
        /// Creates a Remote Event Receiver
        /// </summary>
        /// <param name="receiverName">The name of the remote event receiver</param>
        /// <param name="type"><see cref="Microsoft.SharePoint.Client.EventReceiverType"/></param>
        /// <returns><see cref="Microsoft.SharePoint.Client.EventReceiverDefinitionRemoCreationInformation"/></returns>
        public  EventReceiverDefinitionCreationInformation CreateEventReciever(string receiverName, EventReceiverType type)
        {

            EventReceiverDefinitionCreationInformation _rer = new EventReceiverDefinitionCreationInformation();
            _rer.EventType = type;
            _rer.ReceiverName = receiverName;
            _rer.ReceiverClass = "ECM.AutoTaggingWeb.Services.AutoTaggingService";
            // _rer.ReceiverUrl = "https://amsecm.azurewebsites.net/Services/AutoTaggingService.svc";
            _rer.ReceiverUrl = "https://jciautotag.azurewebsites.net/jciautotagging/services/autotaggingservice.svc";
            _rer.Synchronization = EventReceiverSynchronization.Synchronous;
            return _rer;
        }

        /// <summary>
        /// Add a Remote Event Receiver to a List
        /// </summary>
        /// <param name="ctx">An Authenticated ClientContext</param>
        /// <param name="list">The list</param>
        /// <param name="eventReceiverInfo"><see cref="Microsoft.SharePoint.Client.EventReceiverDefinitionCreationInformation"/></param>
        public  bool AddEventReceiver(ClientContext ctx, List list, EventReceiverDefinitionCreationInformation eventReceiverInfo)
        {
            bool created = false;
            if (!DoesEventReceiverExistByName(ctx, list, eventReceiverInfo.ReceiverName))
            {
               
                list.EventReceivers.Add(eventReceiverInfo);
                ctx.ExecuteQuery();
                created = true;
            }
            return created;
            //else
            //{

            //}
        }
        public  bool DoesEventReceiverExistByName(ClientContext ctx, List list, string eventReceiverName)
        {
            bool _doesExist = false;
            ctx.Load(list, lib => lib.EventReceivers);
            ctx.ExecuteQuery();

            var _rer = list.EventReceivers.Where(e => e.ReceiverName == eventReceiverName).FirstOrDefault();
            if (_rer != null)
            {
                _doesExist = true;
            }

            return _doesExist;
        }
    }
}