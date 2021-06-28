using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Shared
{
    public static partial class Model
    { 
        public class LoginRequest
        {
            #region Properties
            /// <summary>
            /// Username
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// Password
            /// </summary>
            public string Password { get; set; }
            #endregion
        }
    }
}
