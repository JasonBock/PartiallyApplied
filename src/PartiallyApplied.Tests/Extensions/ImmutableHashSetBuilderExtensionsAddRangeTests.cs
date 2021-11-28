using NUnit.Framework;
using PartiallyApplied.Extensions;
using System.Collections.Immutable;

namespace PartiallyApplied.Tests.Extensions;

public static class ImmutableHashSetBuilderExtensionsAddRangeTests
{
	[Test]
	public static void AddRange()
	{
		var builder = ImmutableHashSet.CreateBuilder<int>();
		builder.AddRange(Enumerable.Range(0, 3));

		Assert.That(builder.Count, Is.EqualTo(3));
	}
}