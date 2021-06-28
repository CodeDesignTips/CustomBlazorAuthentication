using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.ServiceLayer
{
    public abstract class BaseService
    {
        #region Properties
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; private set; }
        #endregion

        #region Private methods
        /// <summary>
        /// Handle error message
        /// </summary>
        /// <param name="message">Error message</param>
        protected void HandleError(string message)
        {
            ErrorMessage = message;
        }
        #endregion

    }
}
