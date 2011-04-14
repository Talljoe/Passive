// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System.Data.Common;

    /// <summary>
    ///   A class that wraps your database in Dynamic Funtime
    /// </summary>
    public class DynamicDatabase : DynamicDatabase<DbProviderFactory, DbConnection, DbCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        public DynamicDatabase() : this(null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public DynamicDatabase(string connectionStringName) : base(connectionStringName) {}
    }
}