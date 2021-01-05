using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class CannotPartiallyApplyRefStructDiagnostics
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(CannotPartiallyApplyRefStructDiagnostics.Id, CannotPartiallyApplyRefStructDiagnostics.Title,
				CannotPartiallyApplyRefStructDiagnostics.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					CannotPartiallyApplyRefStructDiagnostics.Id, CannotPartiallyApplyRefStructDiagnostics.Title)),
				node.GetLocation());

		public const string Id = "PA7";
		public const string Message = "Cannot partially apply parameters that are ref struct types (e.g. Span<T>)";
		public const string Title = "Cannot Partially Apply Ref Struct";
	}
}