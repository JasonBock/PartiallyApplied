using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

		static InvocationExpressionSyntax TransformTargets(GeneratorSyntaxContext context, CancellationToken token) =>
			// Typically within the transform function for other incremental generators I've written,
			// I've done further analysis on the invocation node, 
			// but here, it's "good enough" just to return that node, and then within CreateOutput(),
			// I'll use PartiallyAppliedInformation to determine all the diagnostic issues with a given
			// invocation node.
			(InvocationExpressionSyntax)context.Node;

		var provider = context.SyntaxProvider
			.CreateSyntaxProvider(IsSyntaxTargetForGeneration, TransformTargets);
		var output = context.CompilationProvider.Combine(provider.Collect());

		context.RegisterSourceOutput(output,
			(context, source) => CreateOutput(source.Left, source.Right, context));
	}

	private static void CreateOutput(Compilation compilation,
		ImmutableArray<InvocationExpressionSyntax> targets, SourceProductionContext context)
	{
		if (targets.Length > 0)
		{
			var results = new HashSet<PartiallyAppliedInformationResult>();

			foreach (var target in targets.Distinct())
			{
				var information = new PartiallyAppliedInformation(target, compilation);

				foreach (var diagnostic in information.Diagnostics)
				{
					context.ReportDiagnostic(diagnostic);
				}

				if (!information.Diagnostics.Any(_ => _.Severity == DiagnosticSeverity.Error) &&
					information.Result is not null)
				{
					results.Add(information.Result);
				}
			}

			if (results.Count > 0)
			{
				var builder = new PartiallyAppliedBuilder(results.ToImmutableHashSet());
				context.AddSource(Shared.GeneratedFileName, builder.Code);
			}
		}
	}
}

//[Generator]
//public sealed class PartiallyAppliedGenerator
//	: ISourceGenerator
//{
//	public void Execute(GeneratorExecutionContext context)
//	{
//		if (context.SyntaxReceiver is PartiallyAppliedReceiver receiver)
//		{
//			var compilation = context.Compilation;
//			context.CancellationToken.ThrowIfCancellationRequested();
//			var information = new PartiallyAppliedInformation(receiver.Candidates.ToImmutableArray(), compilation);

//			foreach (var diagnostic in information.Diagnostics)
//			{
//				context.ReportDiagnostic(diagnostic);
//			}

//			if (!information.Diagnostics.Any(_ => _.Severity == DiagnosticSeverity.Error) &&
//				information.Results.Length > 0)
//			{
//				var builder = new PartiallyAppliedBuilder(information);
//				context.AddSource(Shared.GeneratedFileName, builder.Code);
//			}
//		}
//	}

//	public void Initialize(GeneratorInitializationContext context) =>
//		context.RegisterForSyntaxNotifications(() => new PartiallyAppliedReceiver());
//}