using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace CustomBlazorAuthentication.Client
{   
    public static class Extensions
    {
        /// <summary>
        /// Get the content-type header value
        /// </summary>
        /// <param name="response">Response info</param>
        /// <param name="headerValue">content-type header value</param>
        /// <returns>False on error, else True</returns>
        private static bool GetResponseContentTypeHeaderValue(HttpResponseMessage response, out string headerValue)
        {
            headerValue = "";

            var headers = response.Content?.Headers;
            if (headers == null)
                return false;

            var header = headers.FirstOrDefault(h => h.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase));
            if (header.Equals(default(KeyValuePair<string, IEnumerable<string>>)))
                return false;

            headerValue = header.Value.FirstOrDefault();
            if (string.IsNullOrEmpty(headerValue))
                return false;

            return true;
        }
        /// <summary>
        /// Check if the response is in json format
        /// </summary>
        /// <param name="response">Response info</param>
        /// <returns>False if not json format, also True</returns>
        public static bool IsJsonResponse(this HttpResponseMessage response)
        {
            if (!GetResponseContentTypeHeaderValue(response, out var contentType))
                return false;

            return contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Check if the response is in html format
        /// </summary>
        /// <param name="response">Response info</param>
        /// <returns>False if not html format, also True</returns>
        public static bool IsHtmlResponse(this HttpResponseMessage response)
        {
            if (!GetResponseContentTypeHeaderValue(response, out var contentType))
                return false;

            return contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Check if the response is in plain text format
        /// </summary>
        /// <param name="response">Response info</param>
        /// <returns>False if not plain text format, also True</returns>
        public static bool IsTextPlainResponse(this HttpResponseMessage response)
        {
            if (!GetResponseContentTypeHeaderValue(response, out var contentType))
                return false;

            return contentType.Contains("text/plain", StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Configure the JsonSerializerOptions of JSRuntime
        /// </summary>
        /// <param name="services">Services info</param>
        /// <returns>Service provider</returns>
        public static IServiceProvider ConfigureJsRuntimeSerialization(this IServiceProvider services)
        {
            try
            {
                //https://github.com/dotnet/aspnetcore/issues/12685
                //Keep property case in jsRuntimeInvokeAsync serialization 
                var jsRuntime = services.GetService<IJSRuntime>();
                var prop = typeof(JSRuntime).GetProperty("JsonSerializerOptions", BindingFlags.NonPublic | BindingFlags.Instance);
                var value = (JsonSerializerOptions)Convert.ChangeType(prop.GetValue(jsRuntime, null), typeof(JsonSerializerOptions));
                value.PropertyNamingPolicy = null;
                value.DictionaryKeyPolicy = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ConfigureJsRuntimeSerialization Error: {ex.Message}");
            }

            return services;
        }
    }
}
