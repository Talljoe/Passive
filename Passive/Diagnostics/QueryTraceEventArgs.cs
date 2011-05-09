namespace Passive.Diagnostics
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class that describes the arguemnts to a query trace event.
    /// </summary>
    public class QueryTraceEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryTraceEventArgs"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">The context.</param>
        public QueryTraceEventArgs(string sql, IEnumerable<object> arguments, string context)
        {
            this.Token = new Guid();
            this.Sql = sql;
            this.Arguments = arguments;
            this.Context = context;
        }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public IEnumerable<object> Arguments { get; private set; }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        public string Sql { get; private set; }

        /// <summary>
        /// Gets the Context.
        /// </summary>
        public string Context { get; private set; }

        /// <summary>
        /// Gets the token.
        /// </summary>
        public Guid Token { get; private set; }
    }
}