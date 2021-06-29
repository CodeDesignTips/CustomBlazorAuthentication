using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Client.Pages
{
    using CustomBlazorAuthentication.Shared;
    using Microsoft.AspNetCore.Components.Forms;
    using System.ComponentModel.DataAnnotations;

    public partial class Register : ComponentBase
    {
        #region Services
        /// <summary>
        /// Manage authentication
        /// </summary>
        [Inject]
        private CustomAuthenticationStateProvider AuthStateProvider { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Form
        /// </summary>
        public EditForm Form { get; set; }
        /// <summary>
        /// User info
        /// </summary>
        public Model.User User { get; set; }
        /// <summary>
        /// Error message
        /// </summary>
        private string Message { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Register()
        {
            User = new Model.User();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Manage component initialization
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            User = new Model.User();
        }
        /// <summary>
        /// Manage user registration
        /// </summary>
        private async Task RegisterUser()
        {
            //Data validation
            if (!Form.EditContext.Validate())
                return;

            var result = await AuthStateProvider.RegisterAsync(User);
            if (result == null || !result.Result)
                Message = $"Registration failed: {result?.ErrorMessage}";
            else
                Message = "Registration completed successfully!";

            StateHasChanged();
        }
        #endregion
    }
}
