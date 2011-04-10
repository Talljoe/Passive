// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System.Collections.Generic;

    /// <summary>
    ///   A class that represents a SQL command.
    /// </summary>
    public class DynamicCommand
    {
        /// <summary>
        /// Gets or sets the SQL.
        /// </summary>
        /// <value>
        /// The SQL.
        /// </value>
        public string Sql { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public IEnumerable<object> Arguments { get; set; }
    }
}