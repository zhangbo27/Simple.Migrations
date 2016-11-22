﻿using System;

namespace SimpleMigrations
{
    /// <summary>
    /// Interface representing database-type-specific operations which needs to be performed
    /// </summary>
    /// <typeparam name="TConection">Type of database connection</typeparam>
    public interface IDatabaseProvider<TConnection>
    {
        TConnection BeginOperation(Func<TConnection> connectionFactory);
        void EndOperation();

        ///// <summary>
        ///// Sets the connection to use. Must be set before calling other methods
        ///// </summary>
        //void SetConnection(TConection connection);

        /// <summary>
        /// Ensure that the version table exists, creating it if necessary
        /// </summary>
        long EnsureCreatedAndGetCurrentVersion(Func<TConnection> connectionFactory);

        /// <summary>
        /// Return the current version from the version table
        /// </summary>
        /// <returns>Current version</returns>
        long GetCurrentVersion();

        //void AcquireDatabaseLock(TConnection connection);
        //void ReleaseDatabaseLock(TConnection connection);



        /// <summary>
        /// Update the current version in the version table
        /// </summary>
        /// <param name="oldVersion">Version being upgraded from</param>
        /// <param name="newVersion">Version being upgraded to</param>
        /// <param name="newDescription">Description to associate with the new version</param>
        void UpdateVersion(long oldVersion, long newVersion, string newDescription);

        //void AcquireDatabaseLock();
        //void ReleaseDatabaseLock();
    }
}
