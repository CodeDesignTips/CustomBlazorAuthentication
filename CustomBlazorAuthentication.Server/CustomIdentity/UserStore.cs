using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Server
{
    using DataLayer;
    using Shared;

    public static partial class CustomIdentity
    { 
        public class UserStore : IUserStore<Model.User>, IUserPasswordStore<Model.User>
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
            public UserStore() : base()
            {

            }
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="providerName">Provider name</param>
            /// <param name="connectionString">Connection string</param>
            public UserStore(string providerName, string connectionString)
            {
                ProviderName = providerName;
                ConnectionString = connectionString;
            }
            #endregion

            #region Methods
            //
            // Summary:
            //     Creates the specified user in the user store.
            //
            // Parameters:
            //   user:
            //     The user to create.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
            //     the Microsoft.AspNetCore.Identity.IdentityResult of the creation operation.
            public Task<IdentityResult> CreateAsync(Model.User user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (!user.IsPasswordEncrypted)
                {
                    user.PasswordSalt = Utils.GeneratePasswordSalt();
                    user.Password = Utils.GetPasswordHash(user.Password, user.PasswordSalt);
                    user.IsPasswordEncrypted = true;
                }

                var ret = false;
                var errorMessage = "";
                using (var db = DbLayer.CreateObject(ProviderName, ConnectionString))
                {
                    ret = db.InsertUser(user);
                    if (!ret)
                        errorMessage = db.ErrorMessage;
                }

                if (ret)
                    return Task.FromResult(IdentityResult.Success);

                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = errorMessage }));
            }
            //
            // Summary:
            //     Deletes the specified user from the user store.
            //
            // Parameters:
            //   user:
            //     The user to delete.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
            //     the Microsoft.AspNetCore.Identity.IdentityResult of the update operation.
            public Task<IdentityResult> DeleteAsync(Model.User user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                var ret = false;
                var errorMessage = "";
                using (var db = DbLayer.CreateObject(ProviderName, ConnectionString))
                {
                    ret = db.RemoveUser(user.UserId);
                    if (!ret)
                        errorMessage = db.ErrorMessage;
                }

                if (ret)
                    return Task.FromResult(IdentityResult.Success);

                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = errorMessage }));

            }
            //
            // Summary:
            //     Finds and returns a user, if any, who has the specified userId.
            //
            // Parameters:
            //   userId:
            //     The user ID to search for.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
            //     the user matching the specified userId if it exists.
            public Task<Model.User> FindByIdAsync(string userId, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!Guid.TryParse(userId, out var userIdValue))
                    throw new ArgumentException("Not a valid Guid id", nameof(userId));

                using (var db = DbLayer.CreateObject(ProviderName, ConnectionString))
                {
                    var user = db.GetUser(userIdValue);

                    return Task.FromResult(user);
                }
            }
            //
            // Summary:
            //     Finds and returns a user, if any, who has the specified normalized user name.
            //
            // Parameters:
            //   normalizedUserName:
            //     The normalized user name to search for.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
            //     the user matching the specified normalizedUserName if it exists.
            public Task<Model.User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var db = DbLayer.CreateObject(ProviderName, ConnectionString))
                {
                    var user = db.GetUser(normalizedUserName);
                    return Task.FromResult(user);
                }
            }
            //
            // Summary:
            //     Gets the normalized user name for the specified user.
            //
            // Parameters:
            //   user:
            //     The user whose normalized name should be retrieved.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
            //     the normalized user name for the specified user.
            public Task<string> GetNormalizedUserNameAsync(Model.User user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.UserName);
            }
            //
            // Summary:
            //     Gets the user identifier for the specified user.
            //
            // Parameters:
            //   user:
            //     The user whose identifier should be retrieved.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
            //     the identifier for the specified user.
            public Task<string> GetUserIdAsync(Model.User user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.UserId.ToString());
            }
            //
            // Summary:
            //     Gets the user name for the specified user.
            //
            // Parameters:
            //   user:
            //     The user whose name should be retrieved.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
            //     the name for the specified user.
            public Task<string> GetUserNameAsync(Model.User user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(user.UserName);
            }
            //
            // Summary:
            //     Sets the given normalized name for the specified user.
            //
            // Parameters:
            //   user:
            //     The user whose name should be set.
            //
            //   normalizedName:
            //     The normalized name to set.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation.
            public Task SetNormalizedUserNameAsync(Model.User user, string normalizedName, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult<object>(null);
            }
            //
            // Summary:
            //     Sets the given userName for the specified user.
            //
            // Parameters:
            //   user:
            //     The user whose name should be set.
            //
            //   userName:
            //     The user name to set.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation.
            public Task SetUserNameAsync(Model.User user, string userName, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                user.UserName = userName;

                return Task.FromResult<object>(null);
            }
            //
            // Summary:
            //     Updates the specified user in the user store.
            //
            // Parameters:
            //   user:
            //     The user to update.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, containing
            //     the Microsoft.AspNetCore.Identity.IdentityResult of the update operation.
            public Task<IdentityResult> UpdateAsync(Model.User user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                //Not supported
                return Task.FromResult(IdentityResult.Failed());
            }

            //
            // Summary:
            //     Gets the password hash for the specified user.
            //
            // Parameters:
            //   user:
            //     The user whose password hash to retrieve.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, returning
            //     the password hash for the specified user.
            public Task<string> GetPasswordHashAsync(Model.User user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrEmpty(user.Password))
                    throw new ArgumentNullException(nameof(user.Password));

                if (user.IsPasswordEncrypted)
                    return Task.FromResult(user.Password);

                user.PasswordSalt = Utils.GeneratePasswordSalt();
                user.Password = Utils.GetPasswordHash(user.Password, user.PasswordSalt);
                user.IsPasswordEncrypted = true;

                return Task.FromResult(user.Password);
            }
            //
            // Summary:
            //     Gets a flag indicating whether the specified user has a password.
            //
            // Parameters:
            //   user:
            //     The user to return a flag for, indicating whether they have a password or not.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation, returning
            //     true if the specified user has a password otherwise false.
            public Task<bool> HasPasswordAsync(Model.User user, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                return Task.FromResult(!string.IsNullOrEmpty(user.Password));
            }
            //
            // Summary:
            //     Sets the password hash for the specified user.
            //
            // Parameters:
            //   user:
            //     The user whose password hash to set.
            //
            //   passwordHash:
            //     The password hash to set.
            //
            //   cancellationToken:
            //     The System.Threading.CancellationToken used to propagate notifications that the
            //     operation should be canceled.
            //
            // Returns:
            //     The System.Threading.Tasks.Task that represents the asynchronous operation.
            public Task SetPasswordHashAsync(Model.User user, string passwordHash, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                user.Password = passwordHash;
                user.IsPasswordEncrypted = true;

                return Task.FromResult<object>(null);
            }

            public void Dispose()
            {

            }
            #endregion
        }
    }
}
