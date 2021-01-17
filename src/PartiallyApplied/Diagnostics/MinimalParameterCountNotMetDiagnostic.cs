using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class MinimalParameterCountNotMetDiagnostic
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(MinimalParameterCountNotMetDiagnostic.Id, MinimalParameterCountNotMetDiagnostic.Title,
				MinimalParameterCountNotMetDiagnostic.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					MinimalParameterCountNotMetDiagnostic.Id, MinimalParameterCountNotMetDiagnostic.Title)),
				node.GetLocation());

		public const string Id = "PA4";
		public const string Message = "The target method needs at least 2 parameters for partial application";
		public const string Title = "Minimal Parameter Count Not Met";
	}
}