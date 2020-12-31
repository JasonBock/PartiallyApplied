using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using PartiallyApplied.Extensions;
using System;

namespace PartiallyApplied
{
	internal sealed class NamespaceGatherer
	{
		private readonly ImmutableHashSet<string>.Builder builder =
			ImmutableHashSet.CreateBuilder<string>();

		public void Add(INamespaceSymbol @namespace) =>
			this.builder.Add(@namespace.GetName());

		public void Add(Type type) =>
			this.builder.Add(type.Namespace);

		public ImmutableHashSet<string> Values => this.builder.ToImmutable();
	}
}