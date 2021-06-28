using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Server
{
    using Microsoft.AspNetCore.Identity;
    using Shared;

    public static partial class CustomIdentity
    {
        public class PasswordHasher : IPasswordHasher<Model.User>
        {
            //
            // Summary:
            //     Returns a hashed representation of the supplied password for the specified user.
            //
            // Parameters:
            //   user:
            //     The user whose password is to be hashed.
            //
            //   password:
            //     The password to hash.
            //
            // Returns:
            //     A hashed representation of the supplied password for the specified user.
            public string HashPassword(Model.User user, string password)
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException(nameof(password));

                user.PasswordSalt = Utils.GeneratePasswordSalt();
                user.Password = Utils.GetPasswordHash(password, user.PasswordSalt);
                user.IsPasswordEncrypted = true;

                return user.Password;
            }
            //
            // Summary:
            //     Returns a Microsoft.AspNetCore.Identity.PasswordVerificationResult indicating
            //     the result of a password hash comparison.
            //
            // Parameters:
            //   user:
            //     The user whose password should be verified.
            //
            //   hashedPassword:
            //     The hash value for a user's stored password.
            //
            //   providedPassword:
            //     The password supplied for comparison.
            //
            // Returns:
            //     A Microsoft.AspNetCore.Identity.PasswordVerificationResult indicating the result
            //     of a password hash comparison.
            //
            // Remarks:
            //     Implementations of this method should be time consistent.
            public PasswordVerificationResult VerifyHashedPassword(Model.User user, string hashedPassword, string providedPassword)
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrEmpty(hashedPassword))
                    throw new ArgumentNullException(nameof(hashedPassword));

                if (string.IsNullOrEmpty(providedPassword))
                    throw new ArgumentNullException(nameof(providedPassword));

                var password = Utils.GetPasswordHash(providedPassword, user.PasswordSalt);

                if (password.Equals(hashedPassword))
                    return PasswordVerificationResult.Success;

                return PasswordVerificationResult.Failed;
            }
        }
    }
    
}
