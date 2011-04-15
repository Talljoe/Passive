// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Async
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Threading.Tasks;

    /// <summary>
    /// Class that represents an Sql database that can be accesses asynchronously.
    /// </summary>
    public class SqlDynamicAsyncDatabase : DynamicAsyncDatabase<SqlClientFactory, SqlConnection, SqlCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDynamicAsyncDatabase"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public SqlDynamicAsyncDatabase(string connectionStringName = null) : base(connectionStringName) {}

        /// <summary>
        /// Gets the task for executing a query.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="behavior">The behavior of the command.</param>
        /// <returns></returns>
        protected override Task<DbDataReader> GetExecuteReaderTask(SqlCommand command, CommandBehavior behavior)
        {
            return Task<DbDataReader>.Factory.FromAsync(command.BeginExecuteReader(behavior),
                                                        command.EndExecuteReader);
        }

        /// <summary>
        /// Gets the task for executing a scalar query.
        /// </summary>
        protected override Task<object> GetExecuteScalarTask(SqlCommand command)
        {
            return Task<DbDataReader>.Factory.FromAsync(
                command.BeginExecuteReader(CommandBehavior.SingleRow | CommandBehavior.SingleResult),
                command.EndExecuteReader).ContinueWith(t => t.Result.Read() ? t.Result[0] : null);
        }

        /// <summary>
        /// Gets the task for executing a non-query.
        /// </summary>
        protected override Task<int> GetExecuteNonQueryTask(SqlCommand command)
        {
            return Task<int>.Factory.FromAsync(command.BeginExecuteNonQuery(), command.EndExecuteNonQuery);
        }
    }
}