using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Shared
{
    public static partial class Model
    { 
        public enum UserRole
        {
            None = -1,
            Administrator = 0,
            User = 1
        }
    }
}
