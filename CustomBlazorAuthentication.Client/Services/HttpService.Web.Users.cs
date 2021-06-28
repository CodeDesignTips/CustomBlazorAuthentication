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

    public partial class UsersHttpService: HttpService
    {
        #region Costruttore
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClient"></param>
        public UsersHttpService(HttpClient httpClient) : base(httpClient)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Inser user
        /// </summary>
        /// <param name="user">User info</param>
        /// <returns>Request response</returns>
        public async Task<Model.UsersPostResponse> PostAsync(Model.User user)
        {
            Model.UsersPostResponse result = null;

            try
            {
                var response = await httpClient.PostAsJsonAsync("users", user);
                result = await CheckJsonResponseAsync<Model.UsersPostResponse>(response);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return result;
        }
        /// <summary>
        /// Remove user
        /// </summary>
        /// <returns>Request response</returns>
        public async Task<Model.UsersDeleteResponse> DeleteAsync(Guid userId)
        {
            Model.UsersDeleteResponse result = null;

            try
            {
                var response = await httpClient.DeleteAsync($"users?userId={userId}");
                result = await CheckJsonResponseAsync<Model.UsersDeleteResponse>(response);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return result;
        }
        #endregion
    }
}
