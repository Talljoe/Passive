// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;

    /// <summary>
    ///   A class that wraps your database in Dynamic Funtime
    /// </summary>
    public class DynamicDatabase
    {
        private string _connectionString;
        private DbProviderFactory _factory;
        private Lazy<DatabaseCapabilities> _capabilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public DynamicDatabase(string connectionStringName = "")
        {
            if (connectionStringName == "")
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
        	Initialize(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString, _providerName);
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicDatabase"/> class.
		/// </summary>
		/// <param name="connectionString">Database connection string</param>
		/// <param name="providerName">Invariant name of the database provider</param>
		public DynamicDatabase(string connectionString, string providerName)
		{
			Initialize(connectionString, providerName);
		}
		private void Initialize(string connectionString, string providerName)
		{
			_factory = DbProviderFactories.GetFactory(providerName);
			_connectionString = connectionString;
			_capabilities = new Lazy<DatabaseCapabilities>(() => GetCapabilities(providerName));
		}

        /// <summary>
        /// Gets the capabilities for this database.
        /// </summary>
        public DatabaseCapabilities Capabilities
        {
            get { return this._capabilities.Value; }
        }

        /// <summary>
        /// Gets the database capabilities.
        /// </summary>
        protected virtual DatabaseCapabilities GetCapabilities(string factoryType)
        {
            switch(factoryType)
            {
                case "System.Data.SqlClient":
                    return new DatabaseCapabilities(supportsRowNumber: true);

                case "System.Data.SqlServerCe.4.0":
                    return new DatabaseCapabilities(supportsOffset: true);

                default:
                    return new DatabaseCapabilities();
            }
        }

        private DbCommand CreateDbCommand(DynamicCommand command, DbTransaction tx = null,
                                          DbConnection connection = null)
        {
            Debug.WriteLine(command.Sql);
            var result = this._factory.CreateCommand();
            result.Connection = connection;
            result.CommandText = command.Sql;
            result.Transaction = tx;
            result.AddParams(command.Arguments);
            return result;
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
            using (var conn = this.OpenConnection())
            {
                var dbCommand = this.CreateDbCommand(command, connection: conn);
                using (var rdr = dbCommand.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        var d = (IDictionary<string, object>) new ExpandoObject();
                        for (var i = 0; i < rdr.FieldCount; i++)
                        {
                            d.Add(rdr.GetName(i), rdr[i]);
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
            using (var conn = this.OpenConnection())
                return this.CreateDbCommand(command, connection: conn).ExecuteScalar();
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
                    .Select(cmd => this.CreateDbCommand(cmd, tx, connection))
                    .Aggregate(0, (a, cmd) => a + cmd.ExecuteNonQuery());
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
    }
}