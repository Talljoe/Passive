// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Async
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    ///   A class that wraps your database table in Dynamic, asynchronous Funtime
    /// </summary>
    public class DynamicAsyncModel : DynamicModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicAsyncModel"/> class using a SqlDynamicAsyncDatabase.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeyField">The primary key field.</param>
        public DynamicAsyncModel(string connectionStringName, string tableName, string primaryKeyField)
            : this(new SqlDynamicAsyncDatabase(connectionStringName), tableName, primaryKeyField) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicAsyncModel"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeyField">The primary key field.</param>
        public DynamicAsyncModel(IDynamicAsyncDatabase database, string tableName, string primaryKeyField)
            : base(database, tableName, primaryKeyField) {}

        /// <summary>
        ///   Gets the database for this model.
        /// </summary>
        public new IDynamicAsyncDatabase Database
        {
            get { return (IDynamicAsyncDatabase) base.Database; }
        }

        /// <summary>
        ///   Returns all records complying with the passed-in WHERE clause and arguments,  ordered as specified, limited (TOP) by limit.
        /// </summary>
        public virtual Task<IEnumerable<dynamic>> AllAsync(object where = null, string orderBy = "",
                                                           int limit = 0, object columns = null, params object[] args)
        {
            return this.DoAll(where, orderBy, limit, columns, args, command => this.Database.QueryAsync(command));
        }

        /// <summary>
        ///   Returns a dynamic PagedResult. Result properties are Items, CurrentPage, TotalPages, and TotalRecords.
        /// </summary>
        public Task<dynamic> PagedAsync(object where = null, string orderBy = "", object columns = null, int pageSize = 20,
                             int currentPage = 1, params object[] args)
        {
            return this.DoPaged(where, orderBy, columns, pageSize, currentPage, args,
                                (count, query) =>
                                {
                                    var totalRecordsTask = this.Database.ScalarAsync(count).ContinueWith(t => (int)t.Result);
                                    var itemsTask = this.Database.QueryAsync(query);
                                    return Task<dynamic>.Factory.ContinueWhenAll(
                                        new Task[] {totalRecordsTask, itemsTask},
                                        t => this.GetPagedResult(currentPage, pageSize, totalRecordsTask.Result, itemsTask.Result));

                                });
        }

        /// <summary>
        ///   Returns a single row from the database
        /// </summary>
        public virtual Task<dynamic> SingleAsync(object key = null, object where = null, object columns = null)
        {
            return this.DoSingle(key, where, columns,
                command => this.Database.FetchAsync(command).ContinueWith(t => t.Result.FirstOrDefault()));
        }
    }
}