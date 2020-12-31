using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class TooManyArgumentsDiagnostics
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(TooManyArgumentsDiagnostics.Id, TooManyArgumentsDiagnostics.Title,
				TooManyArgumentsDiagnostics.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					TooManyArgumentsDiagnostics.Id, TooManyArgumentsDiagnostics.Title)),
				node.GetLocation());

		public const string Id = "PA5";
		public const string Message = "Too many arguments are being passed to the target method";
		public const string Title = "Too Many Arguments";
	}
}