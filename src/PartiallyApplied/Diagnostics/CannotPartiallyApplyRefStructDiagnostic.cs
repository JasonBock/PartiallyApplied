using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics;

internal static class CannotPartiallyApplyRefStructDiagnostic
{
	internal static Diagnostic Create(SyntaxNode node) =>
		Diagnostic.Create(new(CannotPartiallyApplyRefStructDiagnostic.Id, CannotPartiallyApplyRefStructDiagnostic.Title,
			CannotPartiallyApplyRefStructDiagnostic.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				CannotPartiallyApplyRefStructDiagnostic.Id, CannotPartiallyApplyRefStructDiagnostic.Title)),
			node.GetLocation());

	internal const string Id = "PA7";
	internal const string Message = "Cannot partially apply parameters that are ref struct types (e.g. Span<T>)";
	internal const string Title = "Cannot Partially Apply Ref Struct";
}