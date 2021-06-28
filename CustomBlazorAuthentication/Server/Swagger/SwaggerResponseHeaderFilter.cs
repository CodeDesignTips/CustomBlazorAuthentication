using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Server
{
    public class ResponseHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get all response header declarations for a given operation
            var actionResponsesWithHeaders = context.ApiDescription.CustomAttributes()
                .OfType<ProducesResponseHeaderAttribute>()
                .ToArray();

            if (!actionResponsesWithHeaders.Any())
                return;

            foreach (var responseCode in operation.Responses.Keys)
            {
                // Do we have one or more headers for the specific response code
                var responseHeaders = actionResponsesWithHeaders.Where(resp => resp.StatusCode.ToString() == responseCode);
                if (!responseHeaders.Any())
                    continue;

                var response = operation.Responses[responseCode];
                if (response.Headers == null)
                    response.Headers = new Dictionary<string, OpenApiHeader>();

                foreach (var responseHeader in responseHeaders)
                {
                    if (responseHeader.Type != null)
                    {
                        var schemaId = responseHeader.Type.ToString();

                        OpenApiSchema schema = null;
                        if (!context.SchemaRepository.Schemas.ContainsKey(schemaId))
                        {
                            context.SchemaRepository.RegisterType(responseHeader.Type, schemaId);
                            schema = context.SchemaGenerator.GenerateSchema(responseHeader.Type, context.SchemaRepository);
                        }
                        else
                            schema = context.SchemaRepository.Schemas[schemaId];

                        if (schema != null)
                            responseHeader.StringType = SchemaToJson("", schema);
                    }

                    var header = new OpenApiHeader
                    {
                        Description = responseHeader.Description,
                        Schema = new OpenApiSchema
                        {
                            Type = responseHeader.StringType,
                            Description = responseHeader.Description
                        }
                    };

                    response.Headers[responseHeader.Name] = header;
                }
            }
        }

        private string SchemaToJson(string propertyName, OpenApiSchema propertyValue)
        {
            if (propertyValue.Properties.Count == 0 && !string.IsNullOrEmpty(propertyName))
                return $"\"{propertyName}\": \"{propertyValue.Type}\"";

            var jsonSchema = "";
            foreach (var kvp in propertyValue.Properties)
            {
                if (!string.IsNullOrEmpty(jsonSchema))
                    jsonSchema += ", ";

                var jsonProperty = SchemaToJson(kvp.Key, kvp.Value);

                jsonSchema += jsonProperty;
            }

            if (!string.IsNullOrEmpty(jsonSchema))
                jsonSchema = $"{{ {jsonSchema} }}";

            return jsonSchema;
        }
    }
}
