using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace CustomBlazorAuthentication.Client.Services
{
    public class HttpService
    {
        #region Private members
        /// <summary>
        /// Manage http requests
        /// </summary>
        protected HttpClient httpClient;
        #endregion

        #region Properties
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Last http request StatusCode
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
        #endregion

        #region Constructor
        public HttpService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        #endregion

        #region Methods
        #region Check json response
        /// <summary>
        /// Deserialize response returning the json object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="response">Response info</param>
        /// <returns>Json object</returns>
        protected async Task<T> CheckJsonResponseAsync<T>(HttpResponseMessage response) where T : class
        {
            StatusCode = response.StatusCode;

            T result = null;

            if (response.IsJsonResponse())
            {
                try
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        result = JsonSerializer.Deserialize<T>(responseString);
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex.Message);
                }
            }
            else if (response.IsHtmlResponse() || response.IsTextPlainResponse())
            {
                try
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    if (response.IsHtmlResponse())
                        errorMessage = GetHtmlResponseErrorMessage(errorMessage);

                    HandleError(errorMessage);
                }
                catch (Exception ex)
                {
                    HandleError(ex.Message);
                }
            }
            else if (StatusCode == HttpStatusCode.Unauthorized)
                HandleError(nameof(HttpStatusCode.Unauthorized));

            return result;
        }
        /// <summary>
        /// Deserialize response header return json corresponding object
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="response">Response information</param>
        /// <param name="headerKey">Key header</param>
        /// <returns>Json corresponding object</returns>
        protected T CheckJsonResponseHeaderAsync<T>(HttpResponseMessage response, string headerKey) where T : class
        {
            StatusCode = response.StatusCode;

            T result = null;

            try
            {
                //Check header
                var header = response.Headers.FirstOrDefault(h => h.Key.Equals(headerKey.ToLower()));
                if (header.Equals(default(KeyValuePair<string, IEnumerable<string>>)))
                {
                    HandleError($"Header {headerKey} not found!");
                    return null;
                }

                var responseString = header.Value?.FirstOrDefault();
                if (!string.IsNullOrEmpty(responseString))
                    result = JsonSerializer.Deserialize<T>(responseString);
            }
            catch (Exception ex)
            {
                HandleError(ex.Message);
            }

            return result;
        }
        #endregion

        #region Error handling
        /// <summary>
        /// Return error description
        /// </summary>
        /// <param name="htmlResponse">Html response</param>
        /// <returns>Error description</returns>
        protected string GetHtmlResponseErrorMessage(string htmlResponse)
        {
            //Parse error from html title tag
            var tag = "<title>";
            var ret = htmlResponse.IndexOf(tag);
            if (ret >= 0)
            {
                htmlResponse = htmlResponse.Substring(ret + tag.Length);
                ret = htmlResponse.IndexOf("</title>");
                if (ret >= 0)
                {
                    htmlResponse = htmlResponse.Substring(0, ret);
                    return HttpUtility.HtmlDecode(htmlResponse);
                }
            }

            return htmlResponse;
        }
        /// <summary>
        /// Handle error
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        protected void HandleError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Console.WriteLine(errorMessage);
        }
        #endregion
        #endregion
    }
}
