// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;

    /// <summary>
    /// Class that probes for databases in a default manner.
    /// </summary>
    public class DatabaseDetector : IDatabaseDetector
    {
        /// <summary>
        /// Probes the specified database.
        /// </summary>
        public DatabaseCapabilities Probe(DynamicDatabase database, string providerName, string connectionString)
        {
            switch (providerName)
            {
                case "System.Data.SqlClient":
                    return ProbeSqlServer(database);

                case "System.Data.SqlServerCe.4.0":
                    return new DatabaseCapabilities(supportsOffset: true);

                default:
                    return new DatabaseCapabilities();
            }
        }

        private static DatabaseCapabilities ProbeSqlServer(DynamicDatabase database)
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
                var supportsRowNumber = version >= 8; // SQL SERVER 2005
                var supportsOffset = version >= 11; // DENALI

                return new DatabaseCapabilities(supportsRowNumber, supportsOffset);
            }

            return null;
        }
    }
}