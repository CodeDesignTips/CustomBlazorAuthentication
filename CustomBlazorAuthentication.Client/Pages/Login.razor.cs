using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Client.Pages
{
    using CustomBlazorAuthentication.Shared;
    using Microsoft.AspNetCore.Components.Forms;
    using System.ComponentModel.DataAnnotations;

    public partial class Login : ComponentBase
    {
        #region Services
        /// <summary>
        /// Manage page navigation
        /// </summary>
        [Inject]
        private NavigationManager Navigation { get; set; }
        /// <summary>
        /// Manage authentication
        /// </summary>
        [Inject]
        private CustomAuthenticationStateProvider AuthStateProvider { get; set; }
        #endregion

        #region Proprties
        /// <summary>
        /// Edit context
        /// </summary>
        private EditContext EditContext { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// Error message
        /// </summary>
        private string ErrorMessage { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Login()
        {
            EditContext = new EditContext(this);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Manage user login
        /// </summary>
        private async void Authenticate()
        {
            //Data validation
            if (!EditContext.Validate())
                return;

            var loginRequest = new Model.LoginRequest
            {
                UserName = UserName,
                Password = Password
            };

            //Set return url from querystring param
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var returnUrl = "/";
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("returnUrl", out var param))
                returnUrl = param.First();

            //Login
            var result = await AuthStateProvider.LoginAsync(loginRequest);
            if (result.Result)
                Navigation.NavigateTo(returnUrl);
            else
            { 
                ErrorMessage = result.ErrorMessage;
                StateHasChanged();
            }
        }
        #endregion
    }
}
