using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class MinimalParameterCountNotMetDiagnostics
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(MinimalParameterCountNotMetDiagnostics.Id, MinimalParameterCountNotMetDiagnostics.Title,
				MinimalParameterCountNotMetDiagnostics.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					MinimalParameterCountNotMetDiagnostics.Id, MinimalParameterCountNotMetDiagnostics.Title)),
				node.GetLocation());

		public const string Id = "PA4";
		public const string Message = "The target method needs at least 2 parameters for partial application";
		public const string Title = "Minimal Parameter Count Not Met";
	}
}