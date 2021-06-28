using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Swashbuckle.AspNetCore.Filters
{
    public class PolicySelectorWithLabel<T> where T : Attribute
    {
        public Func<IEnumerable<T>, IEnumerable<string>> Selector { get; set; }

        public string SingleLabel { get; set; }
        public string MultipleLabel { get; set; }
    }

    public class SwaggerAppendAuthorizeToSummaryOperationFilter : IOperationFilter
    {
        private readonly AppendAuthorizeToSummaryOperationFilter<AuthorizeAttribute> m_filter;

        public SwaggerAppendAuthorizeToSummaryOperationFilter()
        {
            var policySelector = new PolicySelectorWithLabel<AuthorizeAttribute>
            {
                SingleLabel = "policy",
                MultipleLabel = "policies",

                Selector = authAttributes =>
                    authAttributes
                        .Where(a => !string.IsNullOrEmpty(a.Policy))
                        .Select(a => a.Policy)
            };

            var rolesSelector = new PolicySelectorWithLabel<AuthorizeAttribute>
            {
                SingleLabel = "role",
                MultipleLabel = "roles",

                Selector = authAttributes =>
                    authAttributes
                        .Where(a => !string.IsNullOrEmpty(a.Roles))
                        .Select(a => a.Roles)
            };

            m_filter = new AppendAuthorizeToSummaryOperationFilter<AuthorizeAttribute>(new[] { policySelector, rolesSelector }.AsEnumerable());
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            m_filter.Apply(operation, context);
        }
    }
}