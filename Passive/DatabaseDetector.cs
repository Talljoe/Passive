// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using Passive.Dialect;

    /// <summary>
    /// Class that probes for databases in a default manner.
    /// </summary>
    public class DatabaseDetector : IDatabaseDetector
    {
        /// <summary>
        /// Probes the specified database.
        /// </summary>
        public DatabaseDialect Probe(DynamicDatabase database, string providerName, string connectionString)
        {
            switch (providerName)
            {
                case "System.Data.SqlClient":
                    return ProbeSqlServer(database);

                case "System.Data.SqlServerCe.4.0":
                    return new SqlCompact4Dialect();

                default:
                    return new DatabaseDialect();
            }
        }

        private static DatabaseDialect ProbeSqlServer(DynamicDatabase database)
        {
            string versionString;
            try
            {
                versionString = (string)database.Scalar(@"SELECT SERVERPROPERTY('productversion');");
            }
            catch (SqlException)
            {
                versionString = "0";
            }

            int version;
            if (Int32.TryParse(versionString.Split('.').First(), out version) && version > 0)
            {
                return new SqlServerDialect(version);
            }

            return null;
        }
    }
}