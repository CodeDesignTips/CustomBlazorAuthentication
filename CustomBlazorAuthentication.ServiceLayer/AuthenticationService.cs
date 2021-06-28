using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.ServiceLayer
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Shared;
    using System.Security.Claims;
    using System.Text;

    public class AuthenticationService: BaseService
    {
        #region Private members
        /// <summary>
        /// Configuration settings
        /// </summary>
        private readonly IConfiguration configuration;
        /// <summary>
        /// Authentication manager
        /// </summary>
        private readonly SignInManager<Model.User> signInManager;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="signInManager">Authentication manager</param>
        public AuthenticationService(IConfiguration configuration, SignInManager<Model.User> signInManager)
        {
            this.configuration = configuration;
            this.signInManager = signInManager;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Manage the authentication
        /// </summary>
        /// <param name="request">Authentication info</param>
        /// <returns>Request result</returns>
        public async Task<(bool Result, string AccessToken)> LoginAsync(string userName, string password)
        {
            var accessToken = "";
            var ret = false;

            try
            { 
                var user = await signInManager.UserManager.FindByNameAsync(userName);
                ret = user != null && user.Password == Utils.GetPasswordHash(password, user.PasswordSalt);
                if (!ret)
                    HandleError("User name or password not valid!");
            
                if (ret)
                {
                    await signInManager.SignInAsync(user, false);

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.UserRole.ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expiry = DateTime.Now.AddDays(Convert.ToInt32(configuration["JwtExpiryInDays"]));

                    var token = new JwtSecurityToken(
                            configuration["JwtIssuer"],
                            configuration["JwtAudience"],
                            claims,
                            expires: expiry,
                            signingCredentials: creds
                        );

                    accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                }
            }
            catch(Exception ex)
            {
                ret = false;
                HandleError(ex.Message);
            }

            return (ret, accessToken);
        }
        /// <summary>
        /// Manage user logout
        /// </summary>
        /// <returns>Request result</returns>
        public async Task<bool> LogoutAsync()
        {
            var ret = false;

            try
            {
                await signInManager.SignOutAsync();
                ret = true;
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
