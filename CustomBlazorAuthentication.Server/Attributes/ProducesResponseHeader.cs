using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Server
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ProducesResponseHeaderAttribute : Attribute
    {
        #region Properties
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// String type
        /// </summary>
        public string StringType { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Status code
        /// </summary>
        public int StatusCode { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="type">Type</param>
        /// <param name="statusCode">Status code</param>
        public ProducesResponseHeaderAttribute(string name, string description, string type, int statusCode)
        {
            Name = name;
            Description = description;
            Type = null;
            StringType = type;
            StatusCode = statusCode;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="description">Description</param>
        /// <param name="type">Type</param>
        /// <param name="statusCode">Status code</param>
        public ProducesResponseHeaderAttribute(string name, string description, Type type, int statusCode)
        {
            Name = name;
            Description = description;
            Type = type;
            StringType = "";
            StatusCode = statusCode;
        }
        #endregion
    }
}
