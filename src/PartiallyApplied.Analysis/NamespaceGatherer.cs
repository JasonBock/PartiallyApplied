using System.Collections.Immutable;

namespace PartiallyApplied;

internal sealed class NamespaceGatherer
{
	private readonly ImmutableHashSet<string>.Builder builder =
		ImmutableHashSet.CreateBuilder<string>();

	public void Add(string? @namespace)
	{
		if (@namespace is not null)
		{
			_ = this.builder.Add(@namespace);
		}
	}

	public IImmutableSet<string> Values => this.builder.ToImmutableSortedSet();
}