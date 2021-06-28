using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Shared
{
    public static partial class Model
    { 
        public class User
        {
            #region Properties
            /// <summary>
            /// User id
            /// </summary>
            public Guid UserId { get; set; }
            /// <summary>
            /// Username
            /// </summary>
            [Required]
            public string UserName { get; set; }
            /// <summary>
            /// Password
            /// </summary>
            [Required]
            public string Password { get; set; }
            /// <summary>
            /// Password salt
            /// </summary>
            [SwaggerIgnore]
            public string PasswordSalt { get; set; }
            /// <summary>
            /// Check if the password is encrypted
            /// </summary>
            [SwaggerIgnore]
            public bool IsPasswordEncrypted { get; set; }
            /// <summary>
            /// Password confirm
            /// </summary>
            [Required]
            [Compare("Password")]
            [SwaggerIgnore]
            public string PasswordConfirm { get; set; }
            /// <summary>
            /// Email
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            /// <summary>
            /// User role
            /// </summary>
            public UserRole UserRole { get; set; }
            /// <summary>
            /// Name
            /// </summary>
            [Required]
            public string Name { get; set; }
            /// <summary>
            /// Surname
            /// </summary>
            [Required]
            public string Surname { get; set; }
            #endregion

            #region Constructor
            /// <summary>
            /// Constructor
            /// </summary>
            public User()
            {
                UserRole = UserRole.User;
            }
            #endregion
        }
    }
}
