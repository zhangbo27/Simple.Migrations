﻿using System;
using System.Data;
using System.Data.Common;

namespace SimpleMigrations.DatabaseProvider
{
    /// <summary>
    /// Class which can read from / write to a version table in an MSSQL database
    /// </summary>
    public class MssqlDatabaseProvider : DatabaseProviderBase
    {
        private DbTransaction databaseLockTransaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="MssqlDatabaseProvider"/> class
        /// </summary>
        public MssqlDatabaseProvider()
        {
            this.MaxDescriptionLength = 256;
        }

        protected override void AcquireDatabaseLock(DbConnection connection)
        {
            this.databaseLockTransaction = connection.BeginTransaction(IsolationLevel.Serializable);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {this.TableName} WITH (TABLOCKX)";
                command.Transaction = this.databaseLockTransaction;
                command.ExecuteNonQuery();
            }
        }

        protected override void ReleaseDatabaseLock(DbConnection connection)
        {
            this.databaseLockTransaction?.Commit();
            this.databaseLockTransaction?.Dispose();
            this.databaseLockTransaction = null;
        }

        /// <summary>
        /// Returns SQL to create the version table
        /// </summary>
        /// <returns>SQL to create the version table</returns>
        public override string GetCreateVersionTableSql()
        {
            return $@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[dbo].[{this.TableName}]') AND type in (N'U'))
                BEGIN
                CREATE TABLE [dbo].[{this.TableName}](
                    [Id] [int] IDENTITY(1,1)  PRIMARY KEY NOT NULL,
                    [Version] [int] NOT NULL,
                    [AppliedOn] [datetime] NOT NULL,
                    [Description] [nvarchar]({this.MaxDescriptionLength}) NOT NULL,
                )
                END;";
        }

        /// <summary>
        /// Returns SQL to fetch the current version from the version table
        /// </summary>
        /// <returns>SQL to fetch the current version from the version table</returns>
        public override string GetCurrentVersionSql()
        {
            return $@"SELECT TOP 1 [Version] FROM [dbo].[{this.TableName}] ORDER BY [Id] desc;";
        }

        /// <summary>
        /// Returns SQL to update the current version in the version table
        /// </summary>
        /// <returns>SQL to update the current version in the version table</returns>
        public override string GetSetVersionSql()
        {
            return $@"INSERT INTO [dbo].[{this.TableName}] ([Version], [AppliedOn], [Description]) VALUES (@Version, GETDATE(), @Description);";
        }

        
    }
}
