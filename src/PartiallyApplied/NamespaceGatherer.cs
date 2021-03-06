﻿using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using PartiallyApplied.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public void AddRange(IEnumerable<INamespaceSymbol> namespaces) => 
			this.builder.AddRange(namespaces.Select(_ => _.GetName()));

		public ImmutableHashSet<string> Values => this.builder.ToImmutable();
	}
}