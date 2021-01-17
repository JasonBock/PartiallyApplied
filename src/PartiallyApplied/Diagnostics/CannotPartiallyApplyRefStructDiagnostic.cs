using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class CannotPartiallyApplyRefStructDiagnostic
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(CannotPartiallyApplyRefStructDiagnostic.Id, CannotPartiallyApplyRefStructDiagnostic.Title,
				CannotPartiallyApplyRefStructDiagnostic.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					CannotPartiallyApplyRefStructDiagnostic.Id, CannotPartiallyApplyRefStructDiagnostic.Title)),
				node.GetLocation());

		public const string Id = "PA7";
		public const string Message = "Cannot partially apply parameters that are ref struct types (e.g. Span<T>)";
		public const string Title = "Cannot Partially Apply Ref Struct";
	}
}