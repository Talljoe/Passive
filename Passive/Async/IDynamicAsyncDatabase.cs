#if !NO_ASYNC
// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Async
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface that represents a database that can be accessed asynchronously
    /// </summary>
    public interface IDynamicAsyncDatabase : IDynamicDatabase
    {
        /// <summary>
        ///   Asynchronously runs a query against the database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> FetchAsync(string sql, params object[] args);

        /// <summary>
        ///   Asynchronously runs a query against the database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> FetchAsync(DynamicCommand command);

        /// <summary>
        ///   Asynchronously returns a single result;
        /// </summary>
        Task<dynamic> ScalarAsync(DynamicCommand command);

        /// <summary>
        ///   Asynchronously returns a single result;
        /// </summary>
        Task<dynamic> ScalarAsync(string sql, params object[] args);

        /// <summary>
        ///   Executes a series of commands in a transaction
        /// </summary>
        Task<int> ExecuteTransactionAsync(params DynamicCommand[] commands);

        /// <summary>
        ///   Executes a single command
        /// </summary>
        Task<int> ExecuteAsync(string sql, params object[] args);

        /// <summary>
        ///   Executes a series of commands
        /// </summary>
        Task<int> ExecuteAsync(params DynamicCommand[] commands);

        /// <summary>
        ///   Executes a series of commands optionally in a transaction
        /// </summary>
        Task<int> ExecuteAsync(IEnumerable<DynamicCommand> commands, bool transaction = false);

        /// <summary>
        /// Asynchronously runs a query against the database.
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<dynamic> QueryAsync(string sql, params object[] args);

        /// <summary>
        /// Asynchronously runs a query against the database.
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<dynamic> QueryAsync(DynamicCommand command);
    }
}
#endif
