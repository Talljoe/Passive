// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;

    /// <summary>
    /// Attribute that defines the name of the column in Passive operations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ColumnNameAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}