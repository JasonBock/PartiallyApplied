using Microsoft.CodeAnalysis;
using PartiallyApplied.Builders;
using PartiallyApplied.Diagnostics;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PartiallyApplied
{
	[Generator]
	public sealed class PartiallyAppliedGenerator
		: ISourceGenerator
	{
		/// <summary>
		/// I'm following the lead I got from StrongInject, though this 
		/// was recently taken out there. It's because of this: https://github.com/dotnet/roslyn/pull/46804
		/// I'm getting an exception and I'm getting no information in VS, so I have to keep this in
		/// until I <b>know</b> that full exception information will be shown.
		/// </summary>
		public void Execute(GeneratorExecutionContext context)
		{
			try
			{
				PartiallyAppliedGenerator.PrivateExecute(context);
			}
			catch (Exception e)
			{
				context.ReportDiagnostic(UnexpectedExceptionDiagnostic.Create(e));
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void PrivateExecute(GeneratorExecutionContext context)
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