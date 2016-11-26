﻿using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IUserClientModule
    {
        /// <summary>
        /// Gets all available users.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>
        /// Gets a list of granted database privileges for a user.
        /// </summary>
        /// <param name="username">The name of the user to get granted privilges for.</param>
        Task<IEnumerable<Grant>> GetPrivilegesAsync(string username);

        /// <summary>
        /// Creates a new InfluxDB user with the given user name, password, and admin privileges.
        /// </summary>
        /// <param name="username">The user's name.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="isAdmin">Whether or not to make the user an administrator.</param>
        /// <returns>The query response.</returns>
        Task<IInfluxDataApiResponse> CreateUserAsync(string username, string password, bool isAdmin);

        /// <summary>
        /// Drops an existing InfluxDB user with the given user name.
        /// </summary>
        /// <param name="username">The user's name.</param>
        /// <returns>The query response.</returns>
        Task<IInfluxDataApiResponse> DropUserAsync(string username);

        /// <summary>
        /// Sets a user's password.
        /// </summary>
        /// <param name="username">The user's name.</param>
        /// <param name="password">The password to set.</param>
        /// <returns>The query response.</returns>
        Task<IInfluxDataApiResponse> SetPasswordAsync(string username, string password);
    }
}
