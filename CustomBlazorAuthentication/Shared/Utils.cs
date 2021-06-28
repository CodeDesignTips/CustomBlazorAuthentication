using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.Shared
{
    public static partial class Utils
    {
        #region Password hashing
        /// <summary>
        /// Return the salt to use in password encryption
        /// </summary>
        /// <returns>Salt to use in password encryption</returns>
        public static string GeneratePasswordSalt()
        {
            //base64 salt length:
            //You need 4*(n/3) chars to represent n bytes, and this needs to be rounded up to a multiple of 4.

            var size = 48; // = base64 string of 64 chars
            var random = new RNGCryptoServiceProvider();
            var salt = new byte[size];
            random.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
        /// <summary>
        /// Return hash of salt + password
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="salt">Salt</param>
        /// <returns>Hash of (salt + password)</returns>
        public static string GetPasswordHash(string password, string salt)
        {
            //https://crackstation.net/hashing-security.htm
            //http://www.codeproject.com/Questions/1063132/How-to-Match-Hash-with-Salt-Password-in-Csharp

            //base64 salt length:
            //You need 4*(n/3) chars to represent n bytes, and this needs to be rounded up to a multiple of 4.
            //512bit = 64bytes => stringa base64 di 88 caratteri

            var combinedPassword = string.Concat(salt, password);
            var bytes = new UTF8Encoding().GetBytes(combinedPassword);

            byte[] hashBytes;
            using (var algorithm = new System.Security.Cryptography.SHA512Managed())
            {
                hashBytes = algorithm.ComputeHash(bytes);
            }
            return Convert.ToBase64String(hashBytes);
        }
        #endregion

        #region JWT Token parse
        /// <summary>
        /// Manage token payload parse
        /// </summary>
        /// <param name="base64TokenPayload">Token payload</param>
        /// <returns></returns>
        private static byte[] ParseTokenPayload(string base64TokenPayload)
        {
            switch (base64TokenPayload.Length % 4)
            {
                case 2:
                    base64TokenPayload += "==";
                    break;
                case 3:
                    base64TokenPayload += "=";
                    break;
            }
            return Convert.FromBase64String(base64TokenPayload);
        }
        /// <summary>
        /// Manage JWT token claims parse
        /// </summary>
        /// <param name="accessToken">JWT token</param>
        /// <returns>Claims list</returns>
        public static IEnumerable<Claim> ParseClaimsFromJwt(string accessToken)
        {
            var claims = new List<Claim>();
            var payload = accessToken.Split('.')[1];
            var jsonBytes = ParseTokenPayload(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
        #endregion

        /// <summary>
        /// Get enum value corresponding to description
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="description">Description</param>
        /// <returns>Enum value</returns>
        public static T GetEnumValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
        }
    }
}
