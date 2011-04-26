// Copyright (c) 2011 Tall Ambitions, LLC
// See included LICENSE for details.
namespace Passive.Test.Utility
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Assertions;

    public static class ShouldExtensions
    {
        public static AndConstraint<GenericCollectionAssertions<T>> BeEmptyOrSubsetOf<T>(
            this GenericCollectionAssertions<T> source, IEnumerable<T> expected, string reason = null, params object[] reasonArgs)
        {
            return source.Subject.Any()
                ? source.BeSubsetOf(expected, reason, reasonArgs)
                : source.BeEmpty(reason, reasonArgs);
        }

        public static AndConstraint<GenericCollectionAssertions<T>> HaveEquivalencyTo<T>(
            this GenericCollectionAssertions<T> source, IEnumerable<T> expected, string reason = null, params object[] reasonArgs)
        {
            return expected.Any()
                ? source.BeEquivalentTo(expected, reason, reasonArgs)
                : source.BeEmpty(reason, reasonArgs);
        }
    }
}