using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class NoTargetMethodFoundDiagnostics
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(NoTargetMethodFoundDiagnostics.Id, NoTargetMethodFoundDiagnostics.Title,
				NoTargetMethodFoundDiagnostics.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					IncorrectApplyArgumentCountDiagnostics.Id, IncorrectApplyArgumentCountDiagnostics.Title)),
				node.GetLocation());

		public const string Id = "PA3";
		public const string Message = "The first argument count not be resolved as a method reference";
		public const string Title = "No Target Method Found";
	}
}