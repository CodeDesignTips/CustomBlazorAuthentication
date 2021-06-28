using CustomBlazorAuthentication.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CustomBlazorAuthentication.Server
{
    //https://stackoverflow.com/questions/38070950/generate-swashbuckle-api-documentation-based-on-roles-api-key
    //https://github.com/jenyayel/SwaggerSecurityTrimming
    public class SwaggerAuthorizeRoleFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
    {
        private IHttpContextAccessor httpContext;

        public SwaggerAuthorizeRoleFilter(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext;
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
            
            foreach (var apiDescription in context.ApiDescriptions)
            {
                var actionDescriptor = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)apiDescription.ActionDescriptor;
                var controllerAuthorize = actionDescriptor.ControllerTypeInfo.CustomAttributes.Where(a => a.AttributeType == typeof(AuthorizeAttribute)).ToList();
                var methodAuthorize = actionDescriptor.MethodInfo.CustomAttributes.Where(a => a.AttributeType == typeof(AuthorizeAttribute)).ToList();
                var methodAllowAnonymous = actionDescriptor.MethodInfo.CustomAttributes.Where(a => a.AttributeType == typeof(AllowAnonymousAttribute)).FirstOrDefault();

                //if (methodAllowAnonymous != null)
                //    continue;

                var authorizeAttributes = new List<System.Reflection.CustomAttributeData>();
                authorizeAttributes.AddRange(controllerAuthorize);
                authorizeAttributes.AddRange(methodAuthorize);

                if (authorizeAttributes.Count == 0)
                    continue;

                //Read roles from Authorize attribute
                var roles = new List<string>();
                foreach (var attribute in authorizeAttributes)
                {
                    if (attribute.NamedArguments.Count == 0)
                        continue;

                    foreach (var arg in attribute.NamedArguments)
                    {
                        if (arg.MemberName != "Roles")
                            continue;

                        var attributeRoles = arg.TypedValue.Value?.ToString();
                        if (string.IsNullOrEmpty(attributeRoles))
                            continue;

                        var values = attributeRoles.Split(",", System.StringSplitOptions.TrimEntries).ToList();
                        foreach (var value in values)
                        {
                            if (!roles.Contains(value))
                                roles.Add(value);
                        }
                    }
                }

                //If user is not authenticated or if authenticated but there are role filters where user if not included,
                //action is removed from swagger documentation
                if (!isAuthenticated || (roles.Count > 0 && !roles.Any(role => userRoles != null && userRoles.Contains(role))))
                {
                    var key = "/" + apiDescription.RelativePath;
                    var pathItem = swaggerDoc.Paths[key];
                    switch (apiDescription.HttpMethod?.ToLower())
                    {
                        case "get":
                            pathItem.Operations.Remove(Microsoft.OpenApi.Models.OperationType.Get);
                            break;
                        case "put":
                            pathItem.Operations.Remove(Microsoft.OpenApi.Models.OperationType.Put);
                            break;
                        case "post":
                            pathItem.Operations.Remove(Microsoft.OpenApi.Models.OperationType.Post);
                            break;
                        case "delete":
                            pathItem.Operations.Remove(Microsoft.OpenApi.Models.OperationType.Delete);
                            break;
                        case "options":
                            pathItem.Operations.Remove(Microsoft.OpenApi.Models.OperationType.Options);
                            break;
                        case "head":
                            pathItem.Operations.Remove(Microsoft.OpenApi.Models.OperationType.Head);
                            break;
                        case "patch":
                            pathItem.Operations.Remove(Microsoft.OpenApi.Models.OperationType.Patch);
                            break;
                    }
                    if (!pathItem.Operations.ContainsKey(Microsoft.OpenApi.Models.OperationType.Get) &&
                        !pathItem.Operations.ContainsKey(Microsoft.OpenApi.Models.OperationType.Put) &&
                        !pathItem.Operations.ContainsKey(Microsoft.OpenApi.Models.OperationType.Post) &&
                        !pathItem.Operations.ContainsKey(Microsoft.OpenApi.Models.OperationType.Delete) &&
                        !pathItem.Operations.ContainsKey(Microsoft.OpenApi.Models.OperationType.Options) &&
                        !pathItem.Operations.ContainsKey(Microsoft.OpenApi.Models.OperationType.Head) &&
                        !pathItem.Operations.ContainsKey(Microsoft.OpenApi.Models.OperationType.Patch))
                    {
                        swaggerDoc.Paths.Remove(key);
                    }
                }
            }

            if (swaggerDoc.Paths.Count == 0)
                swaggerDoc.Components.Schemas.Clear();
        }
    }
}
