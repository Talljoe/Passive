// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Dialect
{
    using System;

    /// <summary>
    /// Class that describes a variant of the SQL Language.
    /// </summary>
    public class DatabaseDialect
    {
        /// <summary>
        /// Constant that describes the ROW_NUMBER() method of paging.
        /// </summary>
        public const string RowNumberPagingFormat = "SELECT {1} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {2}) AS [Row___], {1} FROM {0} {3}) AS Paged WHERE [Row___] > {4} AND [Row___] <= ({4} + {5})";
        /// <summary>
        /// Constant that describes the OFFSET/FETCH method of paging.
        /// </summary>
        public const string OffsetPagingFormat = "SELECT {1} FROM {0} {3} ORDER BY {2} OFFSET {4} ROWS FETCH NEXT {5} ROWS ONLY";

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
        public virtual string GetPagingSql(string tableName, string columns, string orderBy, string where, int pageSize, int currentPage)
        {
            throw new NotImplementedException("This database doesn't support paging.");
        }

        /// <summary>
        /// Gets the SQL for retrieving the just-inserted identity.
        /// </summary>
        /// <returns></returns>
        public virtual string GetIdentitySql()
        {
            return @"SELECT @@IDENTITY as NewId";
        }
    }
}