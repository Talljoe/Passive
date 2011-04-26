// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Dialect
{
    using System;

    /// <summary>
    /// Class that describes the SQL CE 4.0 dialect.
    /// </summary>
    public class SqlCompact4Dialect : DatabaseDialect
    {
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
        public override string GetPagingSql(string tableName, string columns, string orderBy, string where, int pageSize,
                                            int currentPage)
        {
            var pageStart = (currentPage - 1) * pageSize;
            return String.Format(OffsetPagingFormat, tableName, columns, orderBy, where, pageStart, pageSize);
        }
    }
}