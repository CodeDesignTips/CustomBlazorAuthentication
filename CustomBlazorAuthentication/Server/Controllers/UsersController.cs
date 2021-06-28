using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Server.Controllers
{
    using Shared;
    using ServiceLayer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Http;

    [Route("[controller]")]
    public class UsersController : BaseController
    {
        #region Private members
        /// <summary>
        /// User service
        /// </summary>
        private UsersService usersService;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager">User manager</param>
        public UsersController(UserManager<Model.User> userManager)
        {
            usersService = new UsersService(userManager);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Manage user insert
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Request result</returns>
        /// <response code="200">Request completed successfully</response>
        /// <response code="400">Request failed</response>
        [HttpPost]
        [ProducesResponseType(typeof(Model.UsersPostResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Model.UsersPostResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(Model.User user)
        {
            if (UserRole == Model.UserRole.User)
                user.UserRole = Model.UserRole.User;

            var errorMessage = "";
            var ret = await usersService.InsertAsync(user);
            if (!ret)
                errorMessage = usersService.ErrorMessage;

            var result = new Model.UsersPostResponse
            {
                Result = ret,
                ErrorMessage = errorMessage
            };

            if (!result.Result)
                return BadRequest(result);

            return Ok(result);
        }
        /// <summary>
        /// Manage user delete
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Request result</returns>
        /// <response code="200">Request completed successfully</response>
        /// <response code="400">Request failed</response>
        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        [ProducesResponseType(typeof(Model.UsersPostResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Model.UsersPostResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var errorMessage = "";
            var ret = await usersService.RemoveAsync(userId);
            if (!ret)
                errorMessage = usersService.ErrorMessage;

            var result = new Model.UsersDeleteResponse
            {
                Result = ret,
                ErrorMessage = errorMessage
            };

            if (!result.Result)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion
    }
}
