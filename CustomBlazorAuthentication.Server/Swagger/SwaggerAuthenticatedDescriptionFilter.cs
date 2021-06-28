using CustomBlazorAuthentication.Shared;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CustomBlazorAuthentication.Server
{
    //https://stackoverflow.com/questions/38070950/generate-swashbuckle-api-documentation-based-on-roles-api-key
    //https://github.com/jenyayel/SwaggerSecurityTrimming
    public class SwaggerAuthenticatedDescriptionFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {
        private IHttpContextAccessor httpContext;
        private string description;
        private string authenticatedDescription;

        public SwaggerAuthenticatedDescriptionFilter(IHttpContextAccessor httpContext, string description, string authenticatedDescription)
        {
            this.httpContext = httpContext;
            this.description = description;
            this.authenticatedDescription = authenticatedDescription;
        }

        public void Apply(Microsoft.OpenApi.Models.OpenApiDocument swaggerDoc, Swashbuckle.AspNetCore.SwaggerGen.DocumentFilterContext context)
        {
            // Prevent IE caching of Swagger.json file 
            httpContext.HttpContext.Response.Headers.Append("Cache-Control", new string[] { "no-store" });

            var isAuthenticated = false;
            List<string> userRoles = null;
            var authorizationHeader = httpContext.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var accessToken = authorizationHeader.Substring(7);
                var claims = Utils.ParseClaimsFromJwt(accessToken);
                if (claims != null)
                {
                    isAuthenticated = true;
                    var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                    if (roleClaim != null)
                        userRoles = roleClaim.Value.Split(new char[] { ',' }, System.StringSplitOptions.TrimEntries).ToList();
                }
            }

            if (!isAuthenticated)
                swaggerDoc.Info.Description = description;
            else
                swaggerDoc.Info.Description = authenticatedDescription;
        }
    }
}
