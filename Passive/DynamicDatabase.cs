﻿// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Dynamic;
    using System.Linq;
    using Passive.Diagnostics;
    using Passive.Dialect;

    /// <summary>
    ///   A class that wraps your database in Dynamic Funtime
    /// </summary>
    public class DynamicDatabase
    {
        private string _connectionString;
        private DbProviderFactory _factory;
        private Lazy<DatabaseDialect> _dialect;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        public DynamicDatabase() : this(null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public DynamicDatabase(string connectionStringName) : this(connectionStringName, (IEnumerable<IDatabaseDetector>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="dialect">The dialect.</param>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public DynamicDatabase(DatabaseDialect dialect, string connectionStringName = "")
            : this(connectionStringName, new[] {new ConstantDialectDetector(dialect)}) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="databaseDetectors">Classes used to probe the database.</param>
        public DynamicDatabase(string connectionStringName, IEnumerable<IDatabaseDetector> databaseDetectors)
        {
            if (String.IsNullOrWhiteSpace(connectionStringName))
            {
                connectionStringName = ConfigurationManager.ConnectionStrings[0].Name;
            }
            var _providerName = "System.Data.SqlClient";
            if (ConfigurationManager.ConnectionStrings[connectionStringName] != null)
            {
                if (!string.IsNullOrWhiteSpace(ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName))
                {
                    _providerName = ConfigurationManager.ConnectionStrings[connectionStringName].ProviderName;
                }
            }
            else
            {
                throw new InvalidOperationException("Can't find a connection string with the name '" +
                                                    connectionStringName + "'");
            }

            Initialize(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString, _providerName, databaseDetectors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="dialect">The dialect.</param>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="providerName">Invariant name of the database provider</param>
        public DynamicDatabase(DatabaseDialect dialect, string connectionString, string providerName)
            : this(connectionString, providerName, new[] {new ConstantDialectDetector(dialect)}) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="providerName">Invariant name of the database provider</param>
        public DynamicDatabase(string connectionString, string providerName) : this(connectionString, providerName, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="providerName">Invariant name of the database provider</param>
        /// <param name="databaseDetectors">Classes used to probe the database.</param>
        public DynamicDatabase(string connectionString, string providerName, IEnumerable<IDatabaseDetector> databaseDetectors)
        {
            Initialize(connectionString, providerName, databaseDetectors);
        }

        private void Initialize(string connectionString, string providerName, IEnumerable<IDatabaseDetector> databaseDetectors = null)
        {
            databaseDetectors = (databaseDetectors ?? Enumerable.Empty<DatabaseDetector>()).DefaultIfEmpty(new DatabaseDetector()).OfType<IDatabaseDetector>();

            this._factory = DbProviderFactories.GetFactory(providerName);
            this._connectionString = connectionString;
            this._dialect = new Lazy<DatabaseDialect>(
                () => databaseDetectors.Select(dd => dd.Probe(this, providerName, connectionString))
                                       .Where(dc => dc != null)
                                       .FirstOrDefault()
                                       ?? new DatabaseDialect());
        }

        /// <summary>
        /// Gets the capabilities for this database.
        /// </summary>
        public DatabaseDialect Dialect
        {
            get { return this._dialect.Value; }
        }

        /// <summary>
        ///   Enumerates the reader yielding the result
        /// </summary>
        public IEnumerable<object> Query(string sql, params object[] args)
        {
            return this.Query(new DynamicCommand {Sql = sql, Arguments = args,});
        }

        /// <summary>
        ///   Enumerates the reader yielding the results
        /// </summary>
        public IEnumerable<object> Query(DynamicCommand command)
        {
            var queryTraceEventArgs = new QueryTraceEventArgs(command.Sql, command.Arguments, command.Context);
            QueryTrace.InvokeQueryBegin(queryTraceEventArgs);
            using (var conn = this.OpenConnection())
            {
                var dbCommand = this.CreateDbCommand(command, connection: conn);
                using (var rdr = dbCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    QueryTrace.InvokeQueryEnd(queryTraceEventArgs);
                    while (rdr.Read())
                    {
                        var d = (IDictionary<string, object>) new ExpandoObject();
                        for (var i = 0; i < rdr.FieldCount; i++)
                        {
                            var value = rdr[i];
                            d.Add(rdr.GetName(i), DBNull.Value.Equals(value) ? null : value);
                        }
                        yield return d;
                    }
                }
            }
        }

        /// <summary>
        ///   Runs a query against the database.
        /// </summary>
        public IList<object> Fetch(string sql, params object[] args)
        {
            return this.Fetch(new DynamicCommand {Sql = sql, Arguments = args,}).ToList();
        }

        /// <summary>
        ///   Runs a query against the database.
        /// </summary>
        public IList<object> Fetch(DynamicCommand command)
        {
            return Query(command).ToList();
        }

        /// <summary>
        ///   Returns a single result
        /// </summary>
        public object Scalar(string sql, params object[] args)
        {
            return this.Scalar(new DynamicCommand {Sql = sql, Arguments = args,});
        }

        /// <summary>
        ///   Returns a single result
        /// </summary>
        public object Scalar(DynamicCommand command)
        {
            var queryTraceEventArgs = new QueryTraceEventArgs(command.Sql, command.Arguments, command.Context);
            QueryTrace.InvokeQueryBegin(queryTraceEventArgs);

            using (var conn = this.OpenConnection())
            {
                var scalar = this.CreateDbCommand(command, connection: conn).ExecuteScalar();
                QueryTrace.InvokeQueryEnd(queryTraceEventArgs);
                return scalar;
            }
        }

        /// <summary>
        ///   Executes a series of DBCommands in a transaction
        /// </summary>
        public int ExecuteTransaction(params DynamicCommand[] commands)
        {
            return this.Execute(commands, transaction: true);
        }

        /// <summary>
        ///   Executes a single DBCommand
        /// </summary>
        public int Execute(string sql, params object[] args)
        {
            return this.Execute(new DynamicCommand {Sql = sql, Arguments = args,});
        }

        /// <summary>
        ///   Executes a series of DBCommands
        /// </summary>
        public int Execute(params DynamicCommand[] commands)
        {
            return this.Execute(commands, transaction: false);
        }

        /// <summary>
        ///   Executes a series of DBCommands optionally in a transaction
        /// </summary>
        public int Execute(IEnumerable<DynamicCommand> commands, bool transaction = false)
        {
            using (var connection = this.OpenConnection())
            using (var tx = (transaction) ? connection.BeginTransaction() : null)
            {
                var result = commands
                    .Select(cmd => new { Args = new QueryTraceEventArgs(cmd.Sql, cmd.Arguments, cmd.Context), Command = this.CreateDbCommand(cmd, tx, connection) })
                    .Aggregate(0, (a, x) =>
                    {
                        QueryTrace.InvokeQueryBegin(x.Args);
                        var r = a + x.Command.ExecuteNonQuery();
                        QueryTrace.InvokeQueryEnd(x.Args);
                        return r;
                    });
                if (tx != null)
                {
                    tx.Commit();
                }
                return result;
            }
        }

        /// <summary>
        ///   Gets a table in the database.
        /// </summary>
        public DynamicModel GetTable(string tableName, string primaryKeyField = "")
        {
            return new DynamicModel(this, tableName, primaryKeyField);
        }

        /// <summary>
        ///   Returns a database connection.
        /// </summary>
        public DbConnection OpenConnection()
        {
            var conn = this._factory.CreateConnection();
            conn.ConnectionString = this._connectionString;
            conn.Open();
            return conn;
        }

        private DbCommand CreateDbCommand(DynamicCommand command, DbTransaction tx = null,
                                          DbConnection connection = null)
        {
            var result = this._factory.CreateCommand();
            result.Connection = connection;
            result.CommandText = command.Sql;
            result.Transaction = tx;
            result.AddParams(command.Arguments);
            return result;
        }

        private class ConstantDialectDetector : IDatabaseDetector
        {
            private readonly DatabaseDialect dialect;

            public ConstantDialectDetector(DatabaseDialect dialect)
            {
                this.dialect = dialect ?? new DatabaseDialect();
            }

            public DatabaseDialect Probe(DynamicDatabase database, string providerName, string connectionString)
            {
                return this.dialect;
            }
        }
    }
}