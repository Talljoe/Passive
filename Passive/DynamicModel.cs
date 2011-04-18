// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    ///   A class that wraps your database table in Dynamic Funtime
    /// </summary>
    public class DynamicModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicModel"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeyField">The primary key field.</param>
        public DynamicModel(string connectionStringName = null, string tableName = null, string primaryKeyField = null)
            : this(new DynamicDatabase(connectionStringName), tableName, primaryKeyField) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicModel"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeyField">The primary key field.</param>
        public DynamicModel(IDynamicDatabase database, string tableName = null, string primaryKeyField = null)
        {
            this.Database = database ?? new DynamicDatabase();
            this.TableName = String.IsNullOrEmpty(tableName) ? this.GetType().Name : tableName;
            this.PrimaryKeyField = String.IsNullOrEmpty(primaryKeyField) ? "ID" : primaryKeyField;
        }

        /// <summary>
        ///   Gets the database for this model.
        /// </summary>
        public IDynamicDatabase Database { get; private set; }

        /// <summary>
        ///   Gets or sets the primary key for this model.
        /// </summary>
        public string PrimaryKeyField { get; private set; }

        /// <summary>
        ///   Gets or sets the table name for this model.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        ///   Builds a set of Insert and Update commands based on the passed-on objects.
        ///   These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects With a PK property (whatever PrimaryKeyField is set to) will be created at UPDATEs
        /// </summary>
        public List<DynamicCommand> BuildCommands(params object[] things)
        {
            return this.BuildCommandsWithWhitelist(null, things);
        }

        /// <summary>
        ///   Builds a set of Insert and Update commands based on the passed-on objects.
        ///   These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects With a PK property (whatever PrimaryKeyField is set to) will be created at UPDATEs
        /// </summary>
        public List<DynamicCommand> BuildCommandsWithWhitelist(object whitelist, params object[] things)
        {
            return
                things.Select(
                    item =>
                    this.HasPrimaryKey(item)
                        ? this.CreateUpdateCommand(item, this.GetPrimaryKey(item), whitelist)
                        : this.CreateInsertCommand(item, whitelist)).ToList();
        }

        /// <summary>
        ///   Executes a set of objects as Insert or Update commands based on their property settings, within a transaction.
        ///   These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects With a PK property (whatever PrimaryKeyField is set to) will be created at UPDATEs
        /// </summary>
        public virtual int Save(params object[] things)
        {
            return this.SaveWithWhitelist(null, things);
        }

        /// <summary>
        ///   Executes a set of objects as Insert or Update commands based on their property settings, within a transaction.
        ///   These objects can be POCOs, Anonymous, NameValueCollections, or Expandos. Objects With a PK property (whatever PrimaryKeyField is set to) will be created at UPDATEs
        /// </summary>
        public virtual int SaveWithWhitelist(object whitelist, params object[] things)
        {
            return this.Database.Execute(this.BuildCommandsWithWhitelist(whitelist, things), transaction: true);
        }

        /// <summary>
        ///   Conventionally introspects the object passed in for a field that  looks like a PK. If you've named your PrimaryKeyField, this becomes easy
        /// </summary>
        public bool HasPrimaryKey(object o)
        {
            return o.ToDictionary().ContainsKey(this.MapPrimaryKey(o));
        }

        /// <summary>
        ///   If the object passed in has a property with the same name as your PrimaryKeyField it is returned here.
        /// </summary>
        public object GetPrimaryKey(object o)
        {
            object result;
            o.ToDictionary().TryGetValue(this.MapPrimaryKey(o), out result);
            return result;
        }

        private string MapPrimaryKey(object o)
        {
            return o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(
                           prop =>
                           prop.GetCustomAttributes(typeof (ColumnNameAttribute), false).Cast<ColumnNameAttribute>().Any
                               (column => column.Name.Equals(this.PrimaryKeyField)))
                       .Select(prop => prop.Name).FirstOrDefault()
                   ?? this.PrimaryKeyField;
        }

        /// <summary>
        ///   Creates a command for use with transactions - internal stuff mostly, but here for you to play with
        /// </summary>
        protected virtual DynamicCommand CreateInsertCommand(object o, object whitelist = null)
        {
            const string stub = "INSERT INTO {0} ({1}) \r\n VALUES ({2}); SELECT @@IDENTITY AS NewID";
            var items = FilterItems(o, whitelist).ToList();
            if (items.Any())
            {
                var keys = string.Join(",", items.Select(item => item.Key));
                var vals = string.Join(",", items.Select((_, i) => "@" + i.ToString()));
                return new DynamicCommand
                           {
                               Sql = string.Format(stub, this.TableName, keys, vals),
                               Arguments = items.Select(item => item.Value),
                           };
            }
            throw new InvalidOperationException("Can't parse this object to the database - there are no properties set");
        }

        /// <summary>
        ///   Creates a command for use with transactions - internal stuff mostly, but here for you to play with
        /// </summary>
        protected virtual DynamicCommand CreateUpdateCommand(object o, object key, object whitelist = null)
        {
            const string stub = "UPDATE {0} SET {1} WHERE {2} = @{3}";
            var items =
                FilterItems(o, whitelist).Where(
                    item => !item.Key.Equals(this.MapPrimaryKey(o), StringComparison.CurrentCultureIgnoreCase)
                         && item.Value != null).ToList();
            if (items.Any())
            {
                var keys = string.Join(",", items.Select((item, i) => string.Format("{0} = @{1} \r\n", item.Key, i)));
                return new DynamicCommand
                           {
                               Sql = string.Format(stub, this.TableName, keys, this.PrimaryKeyField, items.Count),
                               Arguments = items.Select(item => item.Value).Concat(new[] {key}),
                           };
            }
            throw new InvalidOperationException("No parsable object was sent in - could not divine any name/value pairs");
        }

        private static IEnumerable<KeyValuePair<string, object>> FilterItems(object o, object whitelist)
        {
            IEnumerable<KeyValuePair<string, object>> settings = o.ToDictionary();
            var whitelistValues = GetColumns(whitelist)
                .Split(',')
                .Select(s => s.Split(new[] {' ', '[', ']'}, StringSplitOptions.RemoveEmptyEntries).Last());

            if (!string.Equals("*", whitelistValues.FirstOrDefault(), StringComparison.Ordinal))
            {
                settings = settings.Join(whitelistValues, 
                    s => s.Key.Trim(), w => w, (s, _) => s,StringComparer.OrdinalIgnoreCase);
            }
            return settings;
        }

        private static string GetColumn(string propertyName, string columnName)
        {
            return (columnName == null)
                       ? "[" + propertyName.Trim('[', ']') + "]"
                       : string.Format("[{0}] AS [{1}]", columnName.Trim('[', ']'), propertyName.Trim('[', ']'));
        }

        private static string GetColumns(object columns)
        {
            if (columns == null)
            {
                return "*";
            }
            var result =  (columns as string);
            if(result == null)
            {
                IEnumerable<string> value;
                if (columns is Type)
                {
                    var properties = ((Type) columns).GetProperties(BindingFlags.GetProperty |
                                                                       BindingFlags.Public | BindingFlags.Instance);
                    value = properties
                        .Where(prop =>!prop.GetCustomAttributes(typeof (ColumnIgnoreAttribute), false).Any())
                        .Select(property =>
                            GetColumn(property.Name,
                                      property.GetCustomAttributes(
                                          typeof (ColumnNameAttribute), false).Cast
                                          <ColumnNameAttribute>().Select(column => column.Name)
                                          .FirstOrDefault()));
                }
                else
                {
                    if ((columns as IEnumerable<string>) != null)
                    {
                        value = columns as IEnumerable<string>;
                    }
                    else
                    {
                        value = columns
                            .ToDictionary()
                            .Select(kvp => GetColumn(kvp.Key, kvp.Value as string));
                    }
                }
                result = string.Join(",", value);
            }

            return result;
        }

        private DynamicCommand BuildCommand(string sql, object key = null, object where = null, params object[] args)
        {
            var command = new DynamicCommand {Sql = sql};
            if (key != null)
            {
                where = new Dictionary<string, object> {{this.PrimaryKeyField, key}};
            }
            if (where == null)
            {
                return command;
            }
            var whereString = where as string;
            if (whereString != null)
            {
                var whereRegex = new Regex(@"^where ", RegexOptions.IgnoreCase);
                var keyword = whereRegex.IsMatch(sql.Trim()) ? " AND " : " WHERE ";
                command.Sql += keyword + whereString.Replace(whereString.Trim(), String.Empty);
                command.Arguments = (command.Arguments ?? Enumerable.Empty<object>()).Concat(args);
            }
            else
            {
                var dict = where.ToDictionary();
                command.Sql += " WHERE " +
                               String.Join(" AND ", dict.Select((kvp, i) => String.Format("{0} = @{1}", kvp.Key, i)));
                command.Arguments = dict.Select(kvp => kvp.Value).ToArray();
            }
            return command;
        }

        /// <summary>
        ///   Removes one or more records from the DB according to the passed-in WHERE
        /// </summary>
        protected virtual DynamicCommand CreateDeleteCommand(object key = null, object where = null, params object[] args)
        {
            return this.BuildCommand(string.Format("DELETE FROM {0}", this.TableName), key, where, args);
        }

        /// <summary>
        ///   Adds a record to the database. You can pass in an Anonymous object, an ExpandoObject, 
        ///   A regular old POCO, or a NameValueColletion from a Request.Form or Request.QueryString
        /// </summary>
        public virtual object Insert(object o, object whitelist = null)
        {
            return this.Database.Scalar(this.CreateInsertCommand(o, whitelist));
        }

        /// <summary>
        ///   Updates a record in the database. You can pass in an Anonymous object, an ExpandoObject,
        ///   A regular old POCO, or a NameValueCollection from a Request.Form or Request.QueryString
        /// </summary>
        public virtual int Update(object o, object key, object whitelist = null)
        {
            return this.Database.Execute(this.CreateUpdateCommand(o, key, whitelist));
        }

        /// <summary>
        ///   Removes one or more records from the DB according to the passed-in WHERE
        /// </summary>
        public virtual int Delete(object key = null, object where = null, params object[] args)
        {
            return this.Database.Execute(this.CreateDeleteCommand(key, where, args));
        }

        /// <summary>
        ///   Returns all records complying with the passed-in WHERE clause and arguments,  ordered as specified, limited (TOP) by limit.
        /// </summary>
        public virtual IEnumerable<dynamic> All(object where = null, string orderBy = "", int limit = 0, object columns = null,
                                        params object[] args)
        {
            return this.DoAll(where, orderBy, limit, columns, args, command => this.Database.Query(command));
        }

        /// <summary>
        ///   Returns a dynamic PagedResult. Result properties are Items, CurrentPage, TotalPages, and TotalRecords.
        /// </summary>
        public virtual dynamic Paged(object where = null, string orderBy = "", object columns = null, int pageSize = 20,
                             int currentPage = 1, params object[] args)
        {
            return this.DoPaged(where, orderBy, columns, pageSize, currentPage, args,
                                (count, query) => this.GetPagedResult(currentPage, pageSize,
                                    (int) this.Database.Scalar(count), this.Database.Query(query)));
        }

        /// <summary>
        ///   Returns a single row from the database
        /// </summary>
        public virtual dynamic Single(object key = null, object where = null, object columns = null)
        {
            return this.DoSingle(key, where, columns, command => this.Database.Query(command).FirstOrDefault());
        }

        /// <summary>
        ///   Does the work for fetching a single item.
        /// </summary>
        protected virtual T DoSingle<T>(object key, object where, object columns, Func<DynamicCommand, T> work)
        {
            var sql = string.Format("SELECT {0} FROM {1}", GetColumns(columns), this.TableName);
            return work(this.BuildCommand(sql, key, where));
        }

        /// <summary>
        ///   Does the work for getting all items.
        /// </summary>
        protected virtual T DoAll<T>(object where, string orderBy, int limit, object columns, object[] args, Func<DynamicCommand, T> work)
        {
            var sql = String.Format(limit > 0 ? "SELECT TOP " + limit + " {0} FROM {1}" : "SELECT {0} FROM {1}",
                                    GetColumns(columns), this.TableName);
            var command = this.BuildCommand(sql, where: where, args: args);
            if (!String.IsNullOrEmpty(orderBy))
            {
                command.Sql += (orderBy.Trim().StartsWith("order by", StringComparison.CurrentCultureIgnoreCase)
                                    ? " "
                                    : " ORDER BY ") + orderBy;
            }
            return work(command);
        }

        /// <summary>
        ///   Does the work for executing a paged result.
        /// </summary>
        protected virtual dynamic DoPaged(object where, string orderBy, object columns, int pageSize,
                         int currentPage, object[] args, Func<DynamicCommand, DynamicCommand, dynamic> work)
        {
            var countSql = string.Format("SELECT COUNT({0}) FROM {1}", this.PrimaryKeyField, this.TableName);
            if (String.IsNullOrEmpty(orderBy))
            {
                orderBy = this.PrimaryKeyField;
            }
            var sql =
                string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS Row, {0} FROM {2}) AS Paged",
                              GetColumns(columns), orderBy, this.TableName);
            var pageStart = (currentPage - 1) * pageSize;
            sql += string.Format(" WHERE Row >={0} AND Row <={1}", pageStart, (pageStart + pageSize));
            var queryCommand = this.BuildCommand(sql, where: where, args: args);
            var whereCommand = this.BuildCommand(countSql, where: where, args: args);
            return work(whereCommand, queryCommand);
        }

        /// <summary>
        ///   Gets the result for a paged query.
        /// </summary>
        protected virtual dynamic GetPagedResult(int currentPage, int pageSize, int totalRecords, IEnumerable<dynamic> items)
        {
            return new
            {
                CurrentPage = currentPage,
                TotalRecords = totalRecords,
                TotalPages = (totalRecords + (pageSize - 1)) / pageSize,
                Items = items
            }.ToExpando();
        }
    }
}