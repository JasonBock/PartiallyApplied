using Microsoft.CodeAnalysis;
using System;

namespace PartiallyApplied.Diagnostics
{
	internal static class UnexpectedExceptionDiagnostic
	{
		internal static Diagnostic Create(Exception e) =>
			Diagnostic.Create(new(UnexpectedExceptionDiagnostic.Id, UnexpectedExceptionDiagnostic.Title,
				e.ToString().Replace(Environment.NewLine, " : "),
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					UnexpectedExceptionDiagnostic.Id, UnexpectedExceptionDiagnostic.Title)), null);

		internal const string Id = "PA1";
		internal const string Title = "Unexpected Exception";
	}
}