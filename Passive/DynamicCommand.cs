// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using System.Collections.Generic;

    /// <summary>
    ///   A class that represents a sql command.
    /// </summary>
    public class DynamicCommand
    {
        public string Sql { get; set; }
        public IEnumerable<object> Args { get; set; }
    }
}