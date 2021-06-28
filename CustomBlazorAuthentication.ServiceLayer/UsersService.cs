using CustomBlazorAuthentication.Shared;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.ServiceLayer
{
    public class UsersService: BaseService
    {
        #region Private members
        /// <summary>
        /// User service
        /// </summary>
        private UserManager<Model.User> userManager;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager">User manager</param>
        public UsersService(UserManager<Model.User> userManager)
        {
            this.userManager = userManager;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Manage user registration
        /// </summary>
        /// <param name="user">Use info</param>
        /// <returns>Request result</returns>
        public async Task<bool> InsertAsync(Model.User user)
        {
            var ret = false;
            var errorMessage = "";

            try
            {
                var createResult = await userManager.CreateAsync(user);
                ret = createResult.Succeeded;
                if (!ret)
                {
                    foreach (var error in createResult.Errors)
                    {
                        if (!string.IsNullOrEmpty(errorMessage))
                            errorMessage += "\n";
                        errorMessage += error.Description;
                    }

                    HandleError(errorMessage);
                }
            }
            catch (Exception ex)
            {
                HandleError(ex.Message);
            }

            return ret;
        }
        /// <summary>
        /// Manage user delete
        /// </summary>
        /// <param name="userId">Use id</param>
        /// <returns>Request result</returns>
        public async Task<bool> RemoveAsync(Guid userId)
        {
            var ret = false;
            var errorMessage = "";

            try
            {
                var user = await userManager.FindByIdAsync(userId.ToString());
                ret = user != null;
                if (!ret)
                    HandleError("User not found!");
                else
                {
                    ret = false;
                    var deleteResult = await userManager.DeleteAsync(user);
                    ret = deleteResult.Succeeded;
                    if (!ret)
                    {
                        foreach (var error in deleteResult.Errors)
                        {
                            if (!string.IsNullOrEmpty(errorMessage))
                                errorMessage += "\n";
                            errorMessage += error.Description;
                        }

                        HandleError(errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex.Message);
            }

            return ret;
        }
        #endregion

    }
}
