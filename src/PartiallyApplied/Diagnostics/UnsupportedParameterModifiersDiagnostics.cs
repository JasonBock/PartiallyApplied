using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class UnsupportedParameterModifiersDiagnostics
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(UnsupportedParameterModifiersDiagnostics.Id, UnsupportedParameterModifiersDiagnostics.Title,
				UnsupportedParameterModifiersDiagnostics.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					UnsupportedParameterModifiersDiagnostics.Id, UnsupportedParameterModifiersDiagnostics.Title)),
				node.GetLocation());

		public const string Id = "PA6";
		public const string Message = "The target method cannot have `ref`, `out`, or `in` parameter modifiers";
		public const string Title = "Unsupported Parameter Modifiers";
	}
}