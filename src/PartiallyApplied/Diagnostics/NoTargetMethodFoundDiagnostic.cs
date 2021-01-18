using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	internal static class NoTargetMethodFoundDiagnostic
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(NoTargetMethodFoundDiagnostic.Id, NoTargetMethodFoundDiagnostic.Title,
				NoTargetMethodFoundDiagnostic.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					IncorrectApplyArgumentCountDiagnostic.Id, IncorrectApplyArgumentCountDiagnostic.Title)),
				node.GetLocation());

		internal const string Id = "PA3";
		internal const string Message = "The first argument count not be resolved as a method reference";
		internal const string Title = "No Target Method Found";
	}
}