using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PartiallyApplied.Analysis.Models;
using PartiallyApplied.Builders;
using System.Collections.Immutable;

namespace PartiallyApplied;

[Generator]
public sealed class PartiallyAppliedGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken token) =>
			// We're looking for invocations where the containing member name is 
			// Naming.PartiallyClassName and the method name starts with Naming.ApplyMethodName
			node is InvocationExpressionSyntax invocation &&
				invocation.Expression is MemberAccessExpressionSyntax access &&
				access.Expression is IdentifierNameSyntax accessIdentifier &&
				accessIdentifier.Identifier.Text == Naming.PartiallyClassName &&
				access.Name.Identifier.Text.StartsWith(Naming.ApplyMethodName, StringComparison.InvariantCulture);

		static List<AppliedInvocationModel> TransformTargets(GeneratorSyntaxContext context, CancellationToken token)
		{
			var model = AppliedInvocationModel.Create((InvocationExpressionSyntax)context.Node, context.SemanticModel);

			return model.Diagnostics.IsEmpty ?
				[model] :
				[];
		}

		var provider = context.SyntaxProvider
			.CreateSyntaxProvider(IsSyntaxTargetForGeneration, TransformTargets)
			.SelectMany((models, _) => models);
		var output = context.CompilationProvider.Combine(provider.Collect());

		context.RegisterSourceOutput(output,
			(context, source) => CreateOutput(source.Right, context));
	}

	private static void CreateOutput(ImmutableArray<AppliedInvocationModel> targets, SourceProductionContext context)
	{
		if (targets.Length > 0)
		{
			var invocations = new SortedSet<InvocationModel>(new InvocationModelComparer());

			foreach (var target in targets.Distinct())
			{
				// TODO: This will change with https://github.com/JasonBock/PartiallyApplied/issues/25
				foreach (var diagnostic in target.Diagnostics)
				{
					context.ReportDiagnostic(diagnostic);
				}

				if (!target.Diagnostics.Any(_ => _.Severity == DiagnosticSeverity.Error) &&
					target.Invocations.Length > 0)
				{
					foreach (var invocation in target.Invocations)
					{
						_ = invocations.Add(invocation);
					}
				}
			}

			if (invocations.Count > 0)
			{
				var builder = new PartiallyAppliedBuilder([.. invocations]);
				context.AddSource(Shared.GeneratedFileName, builder.Code);
			}
		}
	}

	private sealed class InvocationModelComparer
		: IComparer<InvocationModel>
	{
		public int Compare(InvocationModel x, InvocationModel y)
		{
			var compareTo = x.ApplyName.CompareTo(y.ApplyName);

			if (compareTo != 0)
			{
				return compareTo;
			}
			else
			{
				compareTo = x.PartialArgumentCount.CompareTo(y.PartialArgumentCount);

				return 
					compareTo != 0 ? 
						compareTo : 
						x.Target.CompareTo(y.Target);
			}
		}
	}
}