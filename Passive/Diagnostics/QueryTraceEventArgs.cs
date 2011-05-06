namespace Passive.Diagnostics
{
    using System;

    /// <summary>
    /// Class that describes the arguemnts to a query trace event.
    /// </summary>
    public class QueryTraceEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTraceEventArgs"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public QueryTraceEventArgs(string sql)
        {
            this.Sql = sql;
        }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        public string Sql { get; private set; }
    }
}