using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace CustomBlazorAuthentication.Client
{
    using Services.Web;
    using CustomBlazorAuthentication.Shared;

    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        #region Private members
        /// <summary>
        /// HttpService to manage http requests
        /// </summary>
        private readonly HttpService httpService;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpService">HttpService to manage http requests</param>
        public CustomAuthenticationStateProvider(HttpService httpService)
        {
            this.httpService = httpService;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Return authentication info
        /// </summary>
        /// <returns>Authentication info</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var accessToken = await httpService.Authentication.GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(accessToken))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(Utils.ParseClaimsFromJwt(accessToken), "jwt")));
        }
        /// <summary>
        /// Manage user login
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns>Login response info</returns>
        public async Task<Model.LoginResponse> LoginAsync(Model.LoginRequest loginRequest)
        {
            var result = await httpService.Authentication.LoginAsync(loginRequest);
            if (result.Result)
            {
                var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, loginRequest.UserName) }, "apiauth"));
                var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
                NotifyAuthenticationStateChanged(authState);
            }

            return result;
        }
        /// <summary>
        /// Manage user logout
        /// </summary>
        /// <returns>Logout response info</returns>
        public async Task<Model.LogoutResponse> LogoutAsync()
        {
            var result = await httpService.Authentication.LogoutAsync();
            if (result.Result)
            {
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                var authState = Task.FromResult(new AuthenticationState(anonymousUser));
                NotifyAuthenticationStateChanged(authState);
            }

            return result;
        }
        /// <summary>
        /// Manage user registration
        /// </summary>
        /// <param name="user">User info</param>
        /// <returns>Request result</returns>
        public async Task<Model.UsersPostResponse> RegisterAsync(Model.User user)
        {
            return await httpService.Users.PostAsync(user);
        }
        #endregion
    }
}
