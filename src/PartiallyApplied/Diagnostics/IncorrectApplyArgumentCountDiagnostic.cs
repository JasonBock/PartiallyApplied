using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics;

internal static class IncorrectApplyArgumentCountDiagnostic
{
	internal static Diagnostic Create(SyntaxNode node) =>
		Diagnostic.Create(new(IncorrectApplyArgumentCountDiagnostic.Id, IncorrectApplyArgumentCountDiagnostic.Title,
			IncorrectApplyArgumentCountDiagnostic.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				IncorrectApplyArgumentCountDiagnostic.Id, IncorrectApplyArgumentCountDiagnostic.Title)),
			node.GetLocation());

	internal const string Id = "PA2";
	internal const string Message = "The Apply() method needs the target method and at least one parameter to partially apply";
	internal const string Title = "Incorrect Apply Argument Count";
}