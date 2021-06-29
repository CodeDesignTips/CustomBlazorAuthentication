using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Server
{
    using Shared;

    public static partial class CustomIdentity
    { 
        public class RoleStore : IRoleStore<Model.Role>
        {
            #region Properties
            /// <summary>
            /// Provider name
            /// </summary>
            protected string ProviderName { get; private set; }
            /// <summary>
            /// Connection string
            /// </summary>
            protected string ConnectionString { get; private set; }
            #endregion

            #region Constructor
            /// <summary>
            /// Constructor
            /// </summary>
            public RoleStore()
            {

            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="providerName">Provider name</param>
            /// <param name="connectionString">Connection string</param>
            public RoleStore(string providerName, string connectionString)
            {
                ProviderName = providerName;
                ConnectionString = connectionString;
            }
            #endregion

            #region Methods
            //
            // Summary:
            //     Creates a new role in a store as an asynchronous operation.
            //
            // Parameters:
            //   role:
            //     The role to create in the store.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     A System.Threading.Tasks.Task`1 that represents the Microsoft.AspNetCore.Identity.IdentityResult
            //     of the asynchronous query.
            public Task<IdentityResult> CreateAsync(Model.Role role, CancellationToken cancellationToken)
            {
                return null;
            }
            //
            // Summary:
            //     Deletes a role from the store as an asynchronous operation.
            //
            // Parameters:
            //   role:
            //     The role to delete from the store.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     A System.Threading.Tasks.Task`1 that represents the Microsoft.AspNetCore.Identity.IdentityResult
            //     of the asynchronous query.
            public Task<IdentityResult> DeleteAsync(Model.Role role, CancellationToken cancellationToken)
            {
                return null;
            }

            //
            // Summary:
            //     Finds the role who has the specified ID as an asynchronous operation.
            //
            // Parameters:
            //   roleId:
            //     The role ID to look for.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     A System.Threading.Tasks.Task`1 that result of the look up.
            public Task<Model.Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
            {
                return null;
            }
            //
            // Summary:
            //     Finds the role who has the specified normalized name as an asynchronous operation.
            //
            // Parameters:
            //   normalizedRoleName:
            //     The normalized role name to look for.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     A System.Threading.Tasks.Task`1 that result of the look up.
            public Task<Model.Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
            {
                return null;
            }
            //
            // Summary:
            //     Get a role's normalized name as an asynchronous operation.
            //
            // Parameters:
            //   role:
            //     The role whose normalized name should be retrieved.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     A System.Threading.Tasks.Task`1 that contains the name of the role.
            public Task<string> GetNormalizedRoleNameAsync(Model.Role role, CancellationToken cancellationToken)
            {
                return null;
            }
            //
            // Summary:
            //     Gets the ID for a role from the store as an asynchronous operation.
            //
            // Parameters:
            //   role:
            //     The role whose ID should be returned.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     A System.Threading.Tasks.Task`1 that contains the ID of the role.
            public Task<string> GetRoleIdAsync(Model.Role role, CancellationToken cancellationToken)
            {
                return null;
            }
            //
            // Summary:
            //     Gets the name of a role from the store as an asynchronous operation.
            //
            // Parameters:
            //   role:
            //     The role whose name should be returned.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     A System.Threading.Tasks.Task`1 that contains the name of the role.
            public Task<string> GetRoleNameAsync(Model.Role role, CancellationToken cancellationToken)
            {
                return null;
            }
            //
            // Summary:
            //     Set a role's normalized name as an asynchronous operation.
            //
            // Parameters:
            //   role:
            //     The role whose normalized name should be set.
            //
            //   normalizedName:
            //     The normalized name to set
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation.
            public Task SetNormalizedRoleNameAsync(Model.Role role, string normalizedName, CancellationToken cancellationToken)
            {
                return null;
            }
            //
            // Summary:
            //     Sets the name of a role in the store as an asynchronous operation.
            //
            // Parameters:
            //   role:
            //     The role whose name should be set.
            //
            //   roleName:
            //     The name of the role.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation.
            public Task SetRoleNameAsync(Model.Role role, string roleName, CancellationToken cancellationToken)
            {
                return null;
            }
            //
            // Summary:
            //     Updates a role in a store as an asynchronous operation.
            //
            // Parameters:
            //   role:
            //     The role to update in the store.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     A System.Threading.Tasks.Task`1 that represents the Microsoft.AspNetCore.Identity.IdentityResult
            //     of the asynchronous query.
            public Task<IdentityResult> UpdateAsync(Model.Role role, CancellationToken cancellationToken)
            {
                return null;
            }
            /// <summary>
            /// Dispose the object
            /// </summary>
            public void Dispose()
            {

            }
            #endregion
        }
    }
}
