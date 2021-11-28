using NUnit.Framework;

namespace PartiallyApplied.Tests;

public static class HelpUrlBuilderTests
{
	[Test]
	public static void Create() =>
		Assert.Multiple(() =>
		{
			Assert.That(HelpUrlBuilder.Build("a", "b"),
				Is.EqualTo("https://github.com/JasonBock/PartiallyApplied/tree/master/docs/a-b.md"));
		});
}