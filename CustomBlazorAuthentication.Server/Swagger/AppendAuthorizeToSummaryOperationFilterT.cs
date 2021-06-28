using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Swashbuckle.AspNetCore.Filters
{
    public class AppendAuthorizeToSummaryOperationFilter<T> : IOperationFilter where T : Attribute
    {
        private readonly IEnumerable<PolicySelectorWithLabel<T>> policySelectors;

        /// <summary>
        /// Constructor for AppendAuthorizeToSummaryOperationFilter
        /// </summary>
        /// <param name="policySelectors">Used to select the authorization policy from the attribute e.g. (a => a.Policy)</param>
        public AppendAuthorizeToSummaryOperationFilter(IEnumerable<PolicySelectorWithLabel<T>> policySelectors)
        {
            this.policySelectors = policySelectors;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.GetControllerAndActionAttributes<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var authorizeAttributes = context.GetControllerAndActionAttributes<T>();

            if (authorizeAttributes.Any())
            {
                var authorizationDescription = "";

                foreach (var policySelector in policySelectors)
                    AppendPolicies(authorizeAttributes, ref authorizationDescription, policySelector);

                if (!string.IsNullOrEmpty(authorizationDescription))
                { 
                    authorizationDescription = $"\nRequire authentication with {authorizationDescription.TrimEnd(';')}";
                    operation.Summary += authorizationDescription;
                }
            }
        }

        private void AppendPolicies(IEnumerable<T> authorizeAttributes, ref string authorizationDescription, PolicySelectorWithLabel<T> policySelector)
        {
            var policies = policySelector.Selector(authorizeAttributes)
                .OrderBy(policy => policy);

            var label = policySelector.SingleLabel;
            if (policies.Count() > 1)
                label = policySelector.MultipleLabel;

            if (policies.Any())
                authorizationDescription += $" {label} {string.Join(", ", policies)};";
        }
    }
}