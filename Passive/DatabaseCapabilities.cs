// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive
{
    /// <summary>
    /// Class that describes the capabilities of the database.
    /// </summary>
    public class DatabaseCapabilities
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseCapabilities"/> class.
        /// </summary>
        public DatabaseCapabilities(bool? supportsRowNumber = null,
                                    bool? supportsOffset = null,
                                    DatabaseCapabilities prototype = null)
        {
            prototype = prototype ?? this;
            this.SupportsRowNumber = supportsRowNumber ?? prototype.SupportsRowNumber;
            this.SupportsOffset = supportsOffset ?? prototype.SupportsOffset;
        }

        /// <summary>
        /// Gets a value indicating whether supports ROW_NUMBER for paging.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the database supports ROW_NUMBER; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsRowNumber { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the database OFFSET/FETCH for paging.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the database supports OFFSET/FETCH; otherwise, <c>false</c>.
        /// </value>
        public bool SupportsOffset { get; private set; }
    }
}