using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Shared
{
    public static partial class Model
    { 
        public class Role
        {
            #region Properties
            /// <summary>
            /// Role Id
            /// </summary>
            public Guid RoleId { get; set; }
            /// <summary>
            /// Role name
            /// </summary>
            public string RoleName { get; set; }
            #endregion

            #region Constructor
            /// <summary>
            /// Constructor
            /// </summary>
            public Role()
            {

            }
            #endregion
        }
    }
}
