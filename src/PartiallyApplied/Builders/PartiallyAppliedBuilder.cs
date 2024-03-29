﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using PartiallyApplied.Extensions;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

namespace PartiallyApplied.Builders;

public sealed class PartiallyAppliedBuilder
{
	private readonly ImmutableHashSet<PartiallyAppliedInformationResult> results;

	public PartiallyAppliedBuilder(ImmutableHashSet<PartiallyAppliedInformationResult> results)
	{
		this.results = results;
		this.Code = SourceText.From(this.Build(), Encoding.UTF8);
	}

	private string Build()
	{
		using var textWriter = new StringWriter();
		using var writer = new IndentedTextWriter(textWriter, "\t");

		var gatherer = new NamespaceGatherer();

		writer.WriteLine($"public static partial class {Naming.PartiallyClassName}");
		writer.WriteLine("{");
		writer.Indent++;

		foreach (var result in this.results)
		{
			if (result.Target.IsStandard())
			{
				StandardDelegateBuilder.Build(result, writer, gatherer);
			}
			else
			{
				CustomDelegateBuilder.Build(result, writer, gatherer);
			}
		}

		writer.Indent--;
		writer.WriteLine("}");

#pragma warning disable RS1035 // Do not use APIs banned for analyzers
		return string.Join(Environment.NewLine,
			string.Join(Environment.NewLine, gatherer.Values.Select(_ => $"using {_};")), string.Empty, "#nullable enable", textWriter.ToString());
#pragma warning restore RS1035 // Do not use APIs banned for analyzers
	}

	public SourceText Code { get; private set; }

	private sealed class IMethodSymbolEqualityComparer
		: IEqualityComparer<IMethodSymbol>
	{
		private static readonly Lazy<IMethodSymbolEqualityComparer> defaultValue = new(() => new());

		private IMethodSymbolEqualityComparer()
			: base() { }

		public bool Equals(IMethodSymbol x, IMethodSymbol y) => x.AreEqual(y);

		// TODO: I don't know why RS1024 is firing here. There are GitHub issues 
		// related to this that say this is "fixed", but ... I have no idea
		// why this is firing here.
#pragma warning disable RS1024 // Symbols should be compared for equality
		public int GetHashCode(IMethodSymbol obj) => obj.GetHashCode();
#pragma warning restore RS1024 // Symbols should be compared for equality

		public static IMethodSymbolEqualityComparer Default { get; } = IMethodSymbolEqualityComparer.defaultValue.Value;
	}
}