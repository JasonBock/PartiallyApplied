using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics;

internal static class UnsupportedParameterModifiersDiagnostic
{
	internal static Diagnostic Create(SyntaxNode node) =>
		Diagnostic.Create(new(UnsupportedParameterModifiersDiagnostic.Id, UnsupportedParameterModifiersDiagnostic.Title,
			UnsupportedParameterModifiersDiagnostic.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				UnsupportedParameterModifiersDiagnostic.Id, UnsupportedParameterModifiersDiagnostic.Title)),
			node.GetLocation());

	internal const string Id = "PA6";
	internal const string Message = "The target method cannot have `ref`, `out`, or `in` parameter modifiers";
	internal const string Title = "Unsupported Parameter Modifiers";
}