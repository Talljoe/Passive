// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Dialect
{
    using System;

    /// <summary>
    /// Class that describes the dialect for Sql Server.
    /// </summary>
    public class SqlServerDialect : DatabaseDialect
    {
        private readonly string format;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDialect"/> class.
        /// </summary>
        public SqlServerDialect() : this(0) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerDialect"/> class.
        /// </summary>
        /// <param name="version">The version of the SQL server.</param>
        public SqlServerDialect(int version)
        {
            this.format = (version >= 11) // Denali
                              ? OffsetPagingFormat
                              : RowNumberPagingFormat;
        }

        /// <summary>
        /// Gets the SQL statement for paging given the supplied information.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="where">The where.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="currentPage">The current page.</param>
        /// <returns></returns>
        public override string GetPagingSql(string tableName, string columns, string orderBy, string where,
                                            int pageSize, int currentPage)
        {
            var pageStart = (currentPage - 1) * pageSize;
            return String.Format(this.format, tableName, columns, orderBy, where, pageStart, pageSize);
        }

        /// <summary>
        /// Gets the SQL for retrieving the just-inserted identity.
        /// </summary>
        /// <returns></returns>
        public override string GetIdentitySql()
        {
            return @"SELECT SCOPE_IDENTITY() AS [NewId]";
        }
    }
}