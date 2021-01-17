using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class IncorrectApplyArgumentCountDiagnostic
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(IncorrectApplyArgumentCountDiagnostic.Id, IncorrectApplyArgumentCountDiagnostic.Title,
				IncorrectApplyArgumentCountDiagnostic.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					IncorrectApplyArgumentCountDiagnostic.Id, IncorrectApplyArgumentCountDiagnostic.Title)),
				node.GetLocation());

		public const string Id = "PA2";
		public const string Message = "The Apply() method needs the target method and at least one parameter to partially apply";
		public const string Title = "Incorrect Apply Argument Count";
	}
}