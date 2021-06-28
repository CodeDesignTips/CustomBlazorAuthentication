using CustomBlazorAuthentication.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Server.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        #region Properties
        /// <summary>
        /// Access token
        /// </summary>
        protected string AccessToken
        {
            get
            {
                var accessToken = "";
                if (Request != null && Request.Headers != null && Request.Headers.TryGetValue("Authorization", out var stringValues))
                    accessToken = stringValues[0];

                if (!string.IsNullOrEmpty(accessToken))
                    accessToken = accessToken.Substring(accessToken.IndexOf(" ") + 1);

                return accessToken;
            }
        }
        /// <summary>
        /// User id
        /// </summary>
        protected Guid? UserId
        {
            get
            {
                //Get user info from access token
                if (string.IsNullOrEmpty(AccessToken) || base.User == null)
                    return null;

                var identity = (ClaimsIdentity)base.User.Identity;
                var userId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrEmpty(userId))
                    return null;

                return Guid.Parse(userId);
            }
        }
        /// <summary>
        /// User name
        /// </summary>
        protected string UserName
        {
            get
            {
                //Get user info from access token
                if (string.IsNullOrEmpty(AccessToken) || base.User == null)
                    return null;

                var identity = (ClaimsIdentity)base.User.Identity;
                var userName = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault();

                return userName;
            }
        }
        /// <summary>
        /// User role
        /// </summary>
        protected Model.UserRole UserRole
        {
            get
            {
                //Get user info from access token
                if (string.IsNullOrEmpty(AccessToken) || base.User == null)
                    return Model.UserRole.None;

                var identity = (ClaimsIdentity)base.User.Identity;
                var userRole = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
                if (string.IsNullOrEmpty(userRole))
                    return Model.UserRole.None;

                return Utils.GetEnumValueFromDescription<Model.UserRole>(userRole);
            }
        }
        #endregion
    }
}
