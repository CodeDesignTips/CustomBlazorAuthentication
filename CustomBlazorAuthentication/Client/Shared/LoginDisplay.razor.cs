using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Client.Shared
{
    public partial class LoginDisplay : ComponentBase
    {
        #region Services
        /// <summary>
        /// Manage page navigation
        /// </summary>
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        /// <summary>
        /// Manage authentication
        /// </summary>
        [Inject]
        private CustomAuthenticationStateProvider AuthStateProvider { get; set; }
        #endregion

        /// <summary>
        /// Manage logout
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task Logout(MouseEventArgs args)
        {
            var result = await AuthStateProvider.LogoutAsync();
            if (result.Result)
                NavigationManager.NavigateTo("/");
        }
    }
}
