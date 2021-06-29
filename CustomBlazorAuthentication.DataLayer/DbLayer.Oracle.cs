using CustomBlazorAuthentication.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomBlazorAuthentication.DataLayer
{
    public class OracleDbLayer: DbLayer
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        public OracleDbLayer(string connectionString)
        {
            ConnectionString = connectionString;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get user info
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User info</returns>
        public override Model.User GetUser(Guid userId)
        {
            return users.FirstOrDefault(obj => obj.UserId.Equals(userId)); 
        }
        /// <summary>
        /// Get user info
        /// </summary>
        /// <param name="userName">UserName</param>
        /// <returns>User info</returns>
        public override Model.User GetUser(string userName)
        {
            return users.FirstOrDefault(obj => obj.UserName.Equals(userName));
        }
        /// <summary>
        /// Insert user
        /// </summary>
        /// <param name="user">User info</param>
        /// <returns>False on error, else True</returns>
        public override bool InsertUser(Model.User user)
        {
            if (users.FirstOrDefault(obj => obj.UserName.Equals(user.UserName)) != null)
            {
                ErrorMessage = $"User {user.UserName} already exists!";
                return false;
            }

            user.UserId = Guid.NewGuid();
            users.Add(user);

            return true;
        }
        /// <summary>
        /// Remove user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>False on error, else True</returns>
        public override bool RemoveUser(Guid userId)
        {
            var user = users.FirstOrDefault(obj => obj.UserId.Equals(userId));
            if (user == null)
            {
                ErrorMessage = $"User not found!";
                return false;
            }

            users.Remove(user);

            return true;
        }
        #endregion
    }
}
