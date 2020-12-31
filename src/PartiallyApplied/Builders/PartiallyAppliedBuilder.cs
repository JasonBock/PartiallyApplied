using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using PartiallyApplied.Extensions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PartiallyApplied.Builders
{
	public sealed class PartiallyAppliedBuilder
	{
		private readonly PartiallyAppliedInformation information;

		public PartiallyAppliedBuilder(PartiallyAppliedInformation information)
		{
			this.information = information;
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

			StandardDelegateBuilder.Build(this.information, writer, gatherer);
			CustomDelegateBuilder.Build(this.information, writer, gatherer);

			writer.Indent--;
			writer.WriteLine("}");

			return string.Join(Environment.NewLine,
				string.Join(Environment.NewLine, gatherer.Values.Select(_ => $"using {_};")), string.Empty, "#nullable enable", textWriter.ToString());
		}

		public SourceText Code { get; private set; }

		private sealed class IMethodSymbolEqualityComparer
			: IEqualityComparer<IMethodSymbol>
		{
			private static readonly Lazy<IMethodSymbolEqualityComparer> defaultValue = new(() => new());

			private IMethodSymbolEqualityComparer()
				: base() { }

			public bool Equals(IMethodSymbol x, IMethodSymbol y) => x.AreEqual(y);

			public int GetHashCode(IMethodSymbol obj) => obj.GetHashCode();

			public static IMethodSymbolEqualityComparer Default { get; } = IMethodSymbolEqualityComparer.defaultValue.Value;
		}
	}
}