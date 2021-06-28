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

    [Route("[controller]/[action]")]
    public class AuthenticationController : BaseController
    {
        #region Private members
        /// <summary>
        /// Authentication service
        /// </summary>
        private readonly AuthenticationService authenticationService;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration settings</param>
        /// <param name="signInManager">Authentication manager</param>
        public AuthenticationController(IConfiguration configuration, SignInManager<Model.User> signInManager)
        {
            authenticationService = new AuthenticationService(configuration, signInManager);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Manage user authentication
        /// </summary>
        /// <param name="request">Authentication info</param>
        /// <returns>Request result</returns>
        /// <response code="200">Request completed successfully</response>
        /// <response code="400">Request failed</response>
        [HttpPost]
        [ProducesResponseType(typeof(Model.LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Model.LoginResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(Model.LoginRequest request)
        {
            var result = await authenticationService.LoginAsync(request.UserName, request.Password);

            var loginResponse = new Model.LoginResponse
            {
                Result = result.Result,
                AccessToken = result.AccessToken,
                ErrorMessage = authenticationService.ErrorMessage
            };

            if (!result.Result)
                return BadRequest(loginResponse);

            return Ok(loginResponse);
        }
        /// <summary>
        /// Manage user logout
        /// </summary>
        /// <returns>Request result</returns>
        /// <response code="200">Request completed successfully</response>
        /// <response code="400">Request failed</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Model.LogoutResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Model.LogoutResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            var errorMessage = "";
            var ret = await authenticationService.LogoutAsync();
            if (!ret)
                errorMessage = authenticationService.ErrorMessage;

            var result = new Model.LogoutResponse
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
