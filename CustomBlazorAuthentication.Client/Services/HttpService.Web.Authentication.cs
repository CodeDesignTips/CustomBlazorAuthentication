using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Client.Services
{
    using CustomBlazorAuthentication.Shared;
    using System.Net.Http.Json;

    public partial class AuthenticationHttpService: HttpService
    {
        #region Private members
        protected readonly ILocalStorageService m_localStorage;
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether to use LocalStorage to read / store access token
        /// </summary>
        public bool UseLocalStorageForAccessToken { get; set; }
        #endregion

        #region Costruttore
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="localStorage"></param>
        public AuthenticationHttpService(HttpClient httpClient, ILocalStorageService localStorage) : base(httpClient)
        {
            m_localStorage = localStorage;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Set access token in http header
        /// </summary>
        /// <param name="accessToken">Access token</param>
        private async Task SetAccessTokenAsync(string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                if (UseLocalStorageForAccessToken)
                    await m_localStorage.SetItemAsync("accessToken", accessToken);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = null;

                if (UseLocalStorageForAccessToken)
                    await m_localStorage.RemoveItemAsync("accessToken");
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Return access token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessTokenAsync()
        {
            //Read token from object
            if (httpClient.DefaultRequestHeaders.Authorization != null)
            {
                if (!string.IsNullOrEmpty(httpClient.DefaultRequestHeaders.Authorization.Parameter))
                    return httpClient.DefaultRequestHeaders.Authorization.Parameter;
            }

            //Read token from LocalStorage
            var accessToken = "";
            if (UseLocalStorageForAccessToken)
                accessToken = await m_localStorage.GetItemAsync<string>("accessToken");

            return accessToken;
        }
        /// <summary>
        /// Manage login
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        public async Task<Model.LoginResponse> LoginAsync(Model.LoginRequest loginRequest)
        {
            Model.LoginResponse result = null;

            try
            {
                var response = await httpClient.PostAsJsonAsync("authentication/login", loginRequest);
                result = await CheckJsonResponseAsync<Model.LoginResponse>(response);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            if (result != null && result.Result)
                await SetAccessTokenAsync(result.AccessToken);

            return result;
        }
        /// <summary>
        /// Manage logout
        /// </summary>
        /// <returns>Request response</returns>
        public async Task<Model.LogoutResponse> LogoutAsync()
        {
            Model.LogoutResponse result = null;

            try
            {
                var response = await httpClient.PostAsync("authentication/logout", null);
                result = await CheckJsonResponseAsync<Model.LogoutResponse>(response);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            if (result != null && result.Result)
                await SetAccessTokenAsync(null);

            return result;
        }
        #endregion
    }
}
