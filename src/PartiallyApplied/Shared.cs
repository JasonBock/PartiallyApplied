using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PartiallyApplied.Tests")]

namespace PartiallyApplied;

#pragma warning disable CA1716 // Identifiers should not match keywords
public static class Shared
#pragma warning restore CA1716 // Identifiers should not match keywords
{
	public const string GeneratedFileName = "Partially.g.cs";
}