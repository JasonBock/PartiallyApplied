using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class IncorrectApplyArgumentCountDiagnostics
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(IncorrectApplyArgumentCountDiagnostics.Id, IncorrectApplyArgumentCountDiagnostics.Title,
				IncorrectApplyArgumentCountDiagnostics.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					IncorrectApplyArgumentCountDiagnostics.Id, IncorrectApplyArgumentCountDiagnostics.Title)),
				node.GetLocation());

		public const string Id = "PA2";
		public const string Message = "The Apply() method needs the target method and at least one parameter to partially apply";
		public const string Title = "Incorrect Apply Argument Count";
	}
}