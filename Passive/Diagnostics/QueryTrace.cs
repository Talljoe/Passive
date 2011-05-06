// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Diagnostics
{
    using System;

    /// <summary>
    /// Static class to enable tracing.
    /// </summary>
    public static class QueryTrace
    {
        /// <summary>
        /// Occurs when a query is started.
        /// </summary>
        public static event EventHandler<QueryTraceEventArgs> QueryBegin;

        /// <summary>
        /// Occurs when a query ends.
        /// </summary>
        public static event EventHandler<QueryTraceEventArgs> QueryEnd;

        /// <summary>
        /// Invokes the query begin event.
        /// </summary>
        /// <param name="e">The <see cref="Passive.Diagnostics.QueryTraceEventArgs"/> instance containing the event data.</param>
        internal static void InvokeQueryBegin(QueryTraceEventArgs e)
        {
            var handler = QueryBegin;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        /// <summary>
        /// Invokes the query end event.
        /// </summary>
        /// <param name="e">The <see cref="Passive.Diagnostics.QueryTraceEventArgs"/> instance containing the event data.</param>
        internal static void InvokeQueryEnd(QueryTraceEventArgs e)
        {
            var handler = QueryEnd;
            if (handler != null)
            {
                handler(null, e);
            }
        }
    }
}