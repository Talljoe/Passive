// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    ///   A class that wraps your database in Dynamic Funtime
    /// </summary>
    public class DynamicDatabase : DynamicDatabase<DbProviderFactory, DbConnection, DbCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="databaseDetectors">Classes used to probe the database.</param>
        public DynamicDatabase(string connectionStringName = "", IEnumerable<IDatabaseDetector> databaseDetectors = null)
            : base(connectionStringName, databaseDetectors) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="providerName">Invariant name of the database provider</param>
        /// <param name="databaseDetectors">Classes used to probe the database.</param>
        public DynamicDatabase(string connectionString, string providerName, IEnumerable<IDatabaseDetector> databaseDetectors = null)
            :base(connectionString, providerName, databaseDetectors) {}
    }
}