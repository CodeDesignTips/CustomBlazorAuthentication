using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Client.Services.Web
{
    public class HttpService
    {
        /// <summary>
        /// Authentication http service
        /// </summary>
        public AuthenticationHttpService Authentication { get; private set; }
        /// <summary>
        /// Users http service
        /// </summary>
        public UsersHttpService Users { get; private set; }

        public HttpService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            Authentication = new AuthenticationHttpService(httpClient, localStorage);
            Users = new UsersHttpService(httpClient);
        }
    }
}
