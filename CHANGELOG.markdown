vNext
-----
* DBNull values are now returned as nulls.

0.2.8.0
-------
* Added support for tracing of SQL commands.
* Built [Glimpse plugin][Passive.Glimpse] for viewing tracing data

0.2.0.0
--------
* Fixed error in SQL paging where it would fetch an extra row at the beginning.
* Removed DatabaseCapabilities and replaced with new dialect classes.
* Added overload to DynamicDatabase to take the connection string explicitly (thank you jgeurts).

0.1.3.0
-------
* Added missing "args" parameter to DynamicModel.Single() to fully support the where clause.
* Added DatabaseCapabilities class to track known capabilities.
* Added paging support for SQL CE 4.0.

0.1.2.0
-------
* Fixed a bug in the "where" clause handling.

0.1.1.0
-------
* Work on the build system.
* Update the README.

0.1.0.0
-------
* Initial release.

[Passive.Glimpse]: https://github.com/Talljoe/Passive.Glimpse "Passive.Glimpse project"