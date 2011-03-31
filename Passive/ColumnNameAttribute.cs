// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnNameAttribute : Attribute
    {
        public ColumnNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}