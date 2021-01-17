using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Diagnostics
{
	public static class NoTargetMethodFoundDiagnostic
	{
		internal static Diagnostic Create(SyntaxNode node) =>
			Diagnostic.Create(new(NoTargetMethodFoundDiagnostic.Id, NoTargetMethodFoundDiagnostic.Title,
				NoTargetMethodFoundDiagnostic.Message,
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					IncorrectApplyArgumentCountDiagnostic.Id, IncorrectApplyArgumentCountDiagnostic.Title)),
				node.GetLocation());

		public const string Id = "PA3";
		public const string Message = "The first argument count not be resolved as a method reference";
		public const string Title = "No Target Method Found";
	}
}