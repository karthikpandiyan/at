//-----------------------------------------------------------------------
// <copyright file= "SecurityExtensions.cs" company="Microsoft Corporation &amp; Johnson Controls Inc.">
// Copyright (c) Microsoft Corporation &amp; Johnson Controls Inc.
// All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace JCI.CAM.Common.AppModelExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.SharePoint.Client;

    /// <summary>
    /// This manager class holds security related methods
    /// </summary>
    public static partial class SecurityExtensions
    {
        /// <summary>
        /// Adds a group
        /// </summary>
        /// <param name="web">Site to add the group to</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="groupDescription">Description of the group</param>
        /// <param name="groupIsOwner">Sets the created group as group owner if true</param>
        /// <param name="updateAndExecuteQuery">Set to false to postpone the execute query call</param>
        /// <returns>The created group</returns>
        public static Group AddGroup(this Web web, string groupName, string groupDescription, bool groupIsOwner, bool updateAndExecuteQuery = true)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentNullException("groupName");
            }

            GroupCreationInformation groupCreationInformation = new GroupCreationInformation();
            groupCreationInformation.Title = groupName;
            groupCreationInformation.Description = groupDescription;
            Group group = web.SiteGroups.Add(groupCreationInformation);

            if (groupIsOwner)
            {
                group.Owner = group;
            }

            group.OnlyAllowMembersViewMembership = false;
            group.Update();

            if (updateAndExecuteQuery)
            {
                web.Context.ExecuteQueryRetry();
            }

            return group;
        }

        /// <summary>
        /// Associate the provided groups as default owners, members or visitors groups. If a group is null then the 
        /// association is not done
        /// </summary>
        /// <param name="web">Site to operate on</param>
        /// <param name="owners">Owners group</param>
        /// <param name="members">Members group</param>
        /// <param name="visitors">Visitors group</param>
        public static void AssociateDefaultGroups(this Web web, Group owners, Group members, Group visitors)
        {
            if (owners != null)
            {
                web.AssociatedOwnerGroup = owners;
                web.AssociatedOwnerGroup.Update();
            }

            if (members != null)
            {
                web.AssociatedMemberGroup = members;
                web.AssociatedMemberGroup.Update();
            }

            if (visitors != null)
            {
                web.AssociatedVisitorGroup = visitors;
                web.AssociatedVisitorGroup.Update();
            }

            web.Update();
            web.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Add a permission level (e.g.Contribute, Reader,...) to a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="permissionLevel">Permission level to add</param>
        /// <param name="removeExistingPermissionLevels">Set to true to remove all other permission levels for that group</param>
        public static void AddPermissionLevelToGroup(this Web web, string groupName, RoleType permissionLevel, bool removeExistingPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentNullException("groupName");
            }

            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(permissionLevel);
            web.AddPermissionLevelImplementation(group, roleDefinition, removeExistingPermissionLevels);
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="user">User object representing the user</param>
        /// <exception cref="System.ArgumentNullException">
        /// groupName
        /// or
        /// user
        /// </exception>
        public static void AddUserToGroup(this Web web, string groupName, User user)
        {
            if (groupName == null)
            {
                throw new ArgumentNullException("groupName");
            }

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            // Add the user to the group
            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQuery();

            if (group != null)
            {
                web.AddUserToGroup(group, user);
            }          
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="group">Group object representing the group</param>
        /// <param name="user">User object representing the user</param>
        /// <exception cref="System.ArgumentNullException">
        /// group
        /// or
        /// user
        /// </exception>
        public static void AddUserToGroup(this Web web, Group group, User user)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            group.Users.AddUser(user);
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Adds the permission level implementation.
        /// </summary>
        /// <param name="web">The web.</param>
        /// <param name="principal">The principal.</param>
        /// <param name="roleDefinition">The role definition.</param>
        /// <param name="removeExistingPermissionLevels">if set to <c>true</c> [remove existing permission levels].</param>
        private static void AddPermissionLevelImplementation(this Web web, Principal principal, RoleDefinition roleDefinition, bool removeExistingPermissionLevels = false)
        {
            if (principal != null)
            {
                bool processed = false;
                RoleAssignmentCollection roleAssignmentCollection = web.RoleAssignments;
                web.Context.Load(roleAssignmentCollection);
                web.Context.ExecuteQueryRetry();

                // Find the roles assigned to the principal
                foreach (RoleAssignment ra in roleAssignmentCollection)
                {
                    // correct role assignment found
                    if (ra.PrincipalId == principal.Id)
                    {
                        // load the role definitions for this role assignment
                        RoleDefinitionBindingCollection rdc = ra.RoleDefinitionBindings;
                        web.Context.Load(rdc);
                        web.Context.Load(web.RoleDefinitions);
                        web.Context.ExecuteQueryRetry();

                        // Load the role definition to add (e.g. contribute)
                        // RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(permissionLevel);
                        if (removeExistingPermissionLevels)
                        {
                            // Remove current role definitions by removing all current role definitions
                            rdc.RemoveAll();
                        }

                        // Add the selected role definition
                        rdc.Add(roleDefinition);

                        // update                        
                        ra.ImportRoleDefinitionBindings(rdc);
                        ra.Update();
                        web.Context.ExecuteQueryRetry();

                        // Leave the for each loop
                        processed = true;
                        break;
                    }
                }

                // For a principal without role definitions set we follow a different code path
                if (!processed)
                {
                    RoleDefinitionBindingCollection rdc = new RoleDefinitionBindingCollection(web.Context);
                    rdc.Add(roleDefinition);
                    web.RoleAssignments.Add(principal, rdc);
                    web.Context.ExecuteQueryRetry();
                }
            }
        }
    }
}
