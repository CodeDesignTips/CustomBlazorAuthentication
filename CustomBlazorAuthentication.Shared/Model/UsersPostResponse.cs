using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Shared
{
    public static partial class Model
    { 
        public class UsersPostResponse
        {
            #region Properties
            /// <summary>
            /// Result
            /// </summary>
            public bool Result { get; set; }
            /// <summary>
            /// Error message
            /// </summary>
            public string ErrorMessage { get; set; }
            #endregion
        }
    }
}
