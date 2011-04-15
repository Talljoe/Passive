// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Async
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;
    using System.Linq;

    /// <summary>
    /// Abstract class that represents a database that can be accessed asynchronously
    /// </summary>
    /// <typeparam name="TFactory">The type of the factory.</typeparam>
    /// <typeparam name="TConnection">The type of the connection.</typeparam>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    public abstract class DynamicAsyncDatabase<TFactory, TConnection, TCommand>
        : DynamicDatabase<TFactory, TConnection, TCommand>, IDynamicAsyncDatabase
        where TFactory : DbProviderFactory
        where TConnection : DbConnection
        where TCommand : DbCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicAsyncDatabase&lt;TFactory, TConnection, TCommand&gt;"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        protected DynamicAsyncDatabase(string connectionStringName = null) : base(connectionStringName) {}

        /// <summary>
        /// Asynchronously runs a query against the database.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<object>> FetchAsync(string sql, params object[] args)
        {
            return this.FetchAsync(new DynamicCommand { Sql = sql, Arguments = args, });
        }

        /// <summary>
        /// Asynchronously runs a query against the database.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<object>> FetchAsync(DynamicCommand command)
        {
            var connection = this.OpenConnection();
            var dbCommand = this.CreateDbCommand(command, connection: connection);
            var task = this.GetExecuteReaderTask(dbCommand, CommandBehavior.CloseConnection)
                .ContinueWith(r =>
                {
                    using (dbCommand)
                    {
                        var result = new List<object>();
                        var reader = r.Result;
                        while (reader.Read())
                        {
                            result.Add(this.GetRow(reader));
                        }
                        return result.AsEnumerable();
                    }
                });
            return task;
        }

        /// <summary>
        /// Asynchronously returns a single result;
        /// </summary>
        public Task<object> ScalarAsync(DynamicCommand command)
        {
            var connection = this.OpenConnection();
            var dbCommand = this.CreateDbCommand(command, connection: connection);
            var task = this.GetExecuteScalarTask(dbCommand);
            task.ContinueWith(_ => connection.Close());
            return task;
        }

        /// <summary>
        /// Asynchronously returns a single result;
        /// </summary>
        public Task<object> ScalarAsync(string sql, params object[] args)
        {
            return this.ScalarAsync(new DynamicCommand {Sql = sql, Arguments = args});
        }

        /// <summary>
        ///   Executes a series of commands in a transaction
        /// </summary>
        public Task<int> ExecuteTransactionAsync(params DynamicCommand[] commands)
        {
            return this.ExecuteAsync(commands, transaction: true);
        }

        /// <summary>
        ///   Executes a single command
        /// </summary>
        public Task<int> ExecuteAsync(string sql, params object[] args)
        {
            return this.ExecuteAsync(new DynamicCommand { Sql = sql, Arguments = args, });
        }

        /// <summary>
        ///   Executes a series of commands
        /// </summary>
        public Task<int> ExecuteAsync(params DynamicCommand[] commands)
        {
            return this.ExecuteAsync(commands, transaction: false);
        }

        /// <summary>
        ///   Executes a series of commands optionally in a transaction
        /// </summary>
        public Task<int> ExecuteAsync(IEnumerable<DynamicCommand> commands, bool transaction = false)
        {
            var connection = this.OpenConnection();
            var tx = (transaction) ? connection.BeginTransaction() : null;
            var tasks = commands
                .Select(cmd => this.CreateDbCommand(cmd, tx, connection))
                .Select(this.GetExecuteNonQueryTask)
                .ToArray();

            var result = Task<int>.Factory.ContinueWhenAll(tasks, _ => tasks.Sum(t => t.Result));

            if (tx != null)
            {
                result.ContinueWith(_ => tx.Commit());
            }
            return result;
        }

        /// <summary>
        /// Gets the task for executing a query.
        /// </summary>
        protected abstract Task<DbDataReader> GetExecuteReaderTask(TCommand command, CommandBehavior behavior);

        /// <summary>
        /// Gets the task for executing a scalar query.
        /// </summary>
        protected abstract Task<object> GetExecuteScalarTask(TCommand command);

        /// <summary>
        /// Gets the task for executing a non-query.
        /// </summary>
        protected abstract Task<int> GetExecuteNonQueryTask(TCommand command);
    }
}