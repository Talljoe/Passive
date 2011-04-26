// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System.Collections.Generic;
    using Passive.Dialect;

    /// <summary>
    ///   Describes the interface that wraps your database in Dynamic Funtime
    /// </summary>
    public interface IDynamicDatabase
    {
        /// <summary>
        ///   Enumerates the reader yielding the result
        /// </summary>
        IEnumerable<object> Query(string sql, params object[] args);

        /// <summary>
        ///   Enumerates the reader yielding the results
        /// </summary>
        IEnumerable<object> Query(DynamicCommand command);

        /// <summary>
        ///   Runs a query against the database.
        /// </summary>
        IList<object> Fetch(string sql, params object[] args);

        /// <summary>
        ///   Runs a query against the database.
        /// </summary>
        IList<object> Fetch(DynamicCommand command);

        /// <summary>
        ///   Returns a single result
        /// </summary>
        object Scalar(string sql, params object[] args);

        /// <summary>
        ///   Returns a single result
        /// </summary>
        object Scalar(DynamicCommand command);

        /// <summary>
        ///   Executes a series of commands in a transaction
        /// </summary>
        int ExecuteTransaction(params DynamicCommand[] commands);

        /// <summary>
        ///   Executes a single command
        /// </summary>
        int Execute(string sql, params object[] args);

        /// <summary>
        ///   Executes a series of commands
        /// </summary>
        int Execute(params DynamicCommand[] commands);

        /// <summary>
        ///   Executes a series of commands optionally in a transaction
        /// </summary>
        int Execute(IEnumerable<DynamicCommand> commands, bool transaction = false);

        /// <summary>
        ///   Gets a table in the database.
        /// </summary>
        DynamicModel GetTable(string tableName, string primaryKeyField = "");

        /// <summary>
        /// Gets the capabilities for this database.
        /// </summary>
        DatabaseDialect Dialect { get; }
    }
}