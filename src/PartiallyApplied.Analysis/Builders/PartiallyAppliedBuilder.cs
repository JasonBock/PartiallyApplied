using Microsoft.CodeAnalysis.Text;
using PartiallyApplied.Analysis.Extensions;
using PartiallyApplied.Analysis.Models;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

namespace PartiallyApplied.Builders;

internal sealed class PartiallyAppliedBuilder
{
	private readonly ImmutableArray<InvocationModel> invocations;

	internal PartiallyAppliedBuilder(ImmutableArray<InvocationModel> invocations)
	{
		this.invocations = invocations;
		this.Code = SourceText.From(this.Build(), Encoding.UTF8);
	}

	private string Build()
	{
		using var textWriter = new StringWriter();
		using var writer = new IndentedTextWriter(textWriter, "\t");

		writer.WriteLines(
			$$"""
			#nullable enable
			
			public static partial class {{Naming.PartiallyClassName}}
			{
			""");
			
		writer.Indent++;

		foreach (var result in this.invocations)
		{
			if (result.Target.IsStandard)
			{
				StandardDelegateBuilder.Build(result, writer);
			}
			else
			{
				CustomDelegateBuilder.Build(result, writer);
			}
		}

		writer.Indent--;
		writer.WriteLine("}");

		return textWriter.ToString();
	}

	public SourceText Code { get; private set; }
}