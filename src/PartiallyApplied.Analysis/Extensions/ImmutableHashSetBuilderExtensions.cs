using System.Collections.Immutable;

namespace PartiallyApplied.Extensions;

internal static class ImmutableHashSetBuilderExtensions
{
	extension<T>(ImmutableHashSet<T>.Builder self)
	{
		internal void AddRange(IEnumerable<T> values)
		{
			foreach (var value in values)
			{
				_ = self.Add(value);
			}
		}
	}
}