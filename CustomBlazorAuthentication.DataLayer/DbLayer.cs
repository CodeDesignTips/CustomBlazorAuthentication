using System;

namespace CustomBlazorAuthentication.DataLayer
{
    using CustomBlazorAuthentication.Shared;
    using Shared;
    using System.Collections.Generic;

    public abstract class DbLayer: IDisposable
    {
        #region Private members
        /// <summary>
        /// User list
        /// </summary>
        protected static List<Model.User> users;
        #endregion

        #region Properties
        /// <summary>
        /// Provider name
        /// </summary>
        public string ProviderName { get; set; }
        /// <summary>
        /// ConnectionString
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
        #endregion

        #region Costructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DbLayer()
        {
            //Add demo user
            var user = new Model.User();
            user.UserId = Guid.NewGuid();
            user.UserName = "demo";
            user.PasswordSalt = Utils.GeneratePasswordSalt();
            user.Password = Utils.GetPasswordHash("demo", user.PasswordSalt);
            user.IsPasswordEncrypted = true;
            user.Name = "demo";
            user.Surname = "demo";
            user.Email = "demo@codedesigntips.com";
            user.UserRole = Model.UserRole.Administrator;

            if (users == null)
                users = new List<Model.User> { user };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create a DbLayer object
        /// </summary>
        /// <param name="providerName">Provider name</param>
        /// <param name="connectionString">ConnectionString</param>
        /// <returns>DbLayer object</returns>
        public static DbLayer CreateObject(string providerName, string connectionString)
        {
            if (providerName == "System.Data.SqlClient")
                return new SqlServerDbLayer(connectionString);
            else
                return new OracleDbLayer(connectionString);
        }
        /// <summary>
        /// Return user info
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User info</returns>
        public abstract Model.User GetUser(Guid userId);
        /// <summary>
        /// Return user info
        /// </summary>
        /// <param name="userName">UserName</param>
        /// <returns>User info</returns>
        public abstract Model.User GetUser(string userName);
        /// <summary>
        /// Insert user
        /// </summary>
        /// <param name="user">User info</param>
        /// <returns>False on error, else True</returns>
        public abstract bool InsertUser(Model.User user);
        /// <summary>
        /// Remove user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>False on error, else True</returns>
        public abstract bool RemoveUser(Guid userId);
        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {

        }
        #endregion
    }
}
