using NUnit.Framework;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Tests.Extensions;

public static class ObjectExtensionsGetDefaultValueTests
{
	[TestCase("b", "\"b\"")]
	[TestCase(true, "true")]
	[TestCase(false, "false")]
	[TestCase(null, "null")]
	public static void GetDefaultValue(object value, string expectedResult) =>
		Assert.That(value.GetDefaultValue(), Is.EqualTo(expectedResult));
}