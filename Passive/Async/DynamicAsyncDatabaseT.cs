#if !NO_ASYNC
// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Async
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Disposables;
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
        public IAsyncEnumerable<object> QueryAsync(string sql, params object[] args)
        {
            return this.QueryAsync(new DynamicCommand { Sql = sql, Arguments = args, });
        }

        /// <summary>
        /// Asynchronously runs a query against the database.
        /// </summary>
        /// <returns></returns>
        public IAsyncEnumerable<object> QueryAsync(DynamicCommand command)
        {
            return new AsyncQuery(this, command);
        }

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
            return this.QueryAsync(command)
                       .ToList()
                       .Select(l => l.AsEnumerable());
        }

        /// <summary>
        /// Asynchronously returns a single result;
        /// </summary>
        public Task<object> ScalarAsync(DynamicCommand command)
        {
            return TaskHelpers.Using(this.OpenConnection(),
                                     connection =>
                                         {
                                             var dbCommand = this.CreateDbCommand(command, connection: connection);
                                             return this.GetExecuteScalarTask(dbCommand).Finally(dbCommand.Dispose);
                                         });
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
            return this.ExecuteAsync(commands);
        }

        /// <summary>
        ///   Executes a series of commands optionally in a transaction
        /// </summary>
        public Task<int> ExecuteAsync(IEnumerable<DynamicCommand> commands, bool transaction = false)
        {
            return TaskHelpers.Using(
                new CompositeDisposable(),
                disposable =>
                    {
                        var connection = this.OpenConnection();
                        disposable.Add(connection);
                        DbTransaction tx = null;
                        if (transaction)
                        {
                            tx = connection.BeginTransaction();
                            disposable.Add(tx);
                        }

                        var tasks = commands
                            .Select(cmd => this.CreateDbCommand(cmd, tx, connection))
                            .Do(disposable.Add)
                            .Select(this.GetExecuteNonQueryTask)
                            .ToArray();

                        var result = Task<int>.Factory.ContinueWhenAll(tasks, _ => tasks.Sum(t => t.Result));

                        if (tx != null)
                        {
                            result = result.Do(_ => tx.Commit());
                        }

                        return result;
                    });
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

        private class AsyncQuery : IAsyncEnumerable<object>
        {
            private readonly DynamicAsyncDatabase<TFactory, TConnection, TCommand> database;
            private readonly DynamicCommand command;

            public AsyncQuery(DynamicAsyncDatabase<TFactory, TConnection, TCommand> database, DynamicCommand command)
            {
                this.database = database;
                this.command = command;
            }

            public IAsyncEnumerator<object> GetEnumerator()
            {
                return new QueryEnumerator(database, this.command);
            }

            private class QueryEnumerator : IAsyncEnumerator<object>
            {
                private readonly CompositeDisposable disposable = new CompositeDisposable();

                private readonly DynamicAsyncDatabase<TFactory, TConnection, TCommand> database;
                private readonly DynamicCommand command;
                private DbDataReader reader;

                public QueryEnumerator(DynamicAsyncDatabase<TFactory, TConnection, TCommand> database, DynamicCommand command)
                {
                    this.database = database;
                    this.command = command;
                }

                public void Dispose()
                {
                    disposable.Dispose();
                }

                public Task<bool> MoveNext()
                {
                    Task<bool> task;
                    if (reader == null)
                    {
                        var readerTask = this.GetReader();
                        task = readerTask.Select(
                            result =>
                                {
                                    this.reader = result;
                                    this.disposable.Add(this.reader);
                                    return this.reader.Read();
                                });
                    }
                    else
                    {
                        task = Task<bool>.Factory.StartNew(reader.Read);
                    }

                    return task.Do(() => { this.Current = database.GetRow(this.reader); });
                }

                public object Current { get; private set; }

                private Task<DbDataReader> GetReader()
                {
                    var connection = this.database.OpenConnection();
                    this.disposable.Add(connection);
                    var dbCommand = this.database.CreateDbCommand(this.command, connection: connection);
                    this.disposable.Add(dbCommand);

                    return this.database.GetExecuteReaderTask(dbCommand, CommandBehavior.CloseConnection)
                                        .Do(r => this.disposable.Add(r));
                }
            }
        }
    }
}
#endif
