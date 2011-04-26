// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    using Passive.Dialect;

    /// <summary>
    /// Interface that describes a way of detecting the database
    /// </summary>
    public interface IDatabaseDetector
    {
        /// <summary>
        /// Probes the specified database.
        /// </summary>
        DatabaseDialect Probe(DynamicDatabase database, string providerName, string connectionString);
    }
}