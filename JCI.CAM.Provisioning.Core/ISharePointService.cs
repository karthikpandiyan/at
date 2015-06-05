// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISharePointService.cs" company="Microsoft">
//   Copyright (c) 2014. All rights reserved.
// </copyright>
// <summary>
//   Interface used by to implement Services that use SharePoint
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace JCI.CAM.Provisioning.Core
{
    using System;
    using Microsoft.SharePoint.Client;
    
    /// <summary>
    /// Interface used by to implement Services that use SharePoint
    /// </summary>
    public interface ISharePointService
    {
        /// <summary>
        /// Delegate that is used by the implementation class for working with
        /// ClientContext Object
        /// </summary>
        /// <param name="action">The action.</param>
        void UsingContext(Action<ClientContext> action);

        /// <summary>
        /// Usings the context.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="csomTimeout">The CSOM timeout.</param>
        void UsingContext(Action<ClientContext> action, int csomTimeout);
    }
}
