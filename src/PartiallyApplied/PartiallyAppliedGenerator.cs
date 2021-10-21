using Microsoft.CodeAnalysis;
using PartiallyApplied.Builders;
using System.Linq;

namespace PartiallyApplied
{
	[Generator]
	public sealed class PartiallyAppliedGenerator
		: ISourceGenerator
	{
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.SyntaxReceiver is PartiallyAppliedReceiver receiver)
			{
				var compilation = context.Compilation;
				context.CancellationToken.ThrowIfCancellationRequested();
				var information = new PartiallyAppliedInformation(receiver.Candidates, compilation);

				foreach (var diagnostic in information.Diagnostics)
				{
					context.ReportDiagnostic(diagnostic);
				}

				if (!information.Diagnostics.Any(_ => _.Severity == DiagnosticSeverity.Error) &&
					information.Results.Length > 0)
				{
					var builder = new PartiallyAppliedBuilder(information);
					context.AddSource(Shared.GeneratedFileName, builder.Code);
				}
			}
		}

		public void Initialize(GeneratorInitializationContext context) =>
			context.RegisterForSyntaxNotifications(() => new PartiallyAppliedReceiver());
	}
}