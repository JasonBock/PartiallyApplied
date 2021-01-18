using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	internal static class TooManyArgumentsDiagnostic
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(TooManyArgumentsDiagnostic.Id, TooManyArgumentsDiagnostic.Title,
				TooManyArgumentsDiagnostic.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					TooManyArgumentsDiagnostic.Id, TooManyArgumentsDiagnostic.Title)),
				node.GetLocation());

		internal const string Id = "PA5";
		internal const string Message = "Too many arguments are being passed to the target method";
		internal const string Title = "Too Many Arguments";
	}
}