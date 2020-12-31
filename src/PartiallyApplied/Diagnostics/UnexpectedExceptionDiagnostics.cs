using Microsoft.CodeAnalysis;
using System;

namespace PartiallyApplied.Diagnostics
{
	public static class UnexpectedExceptionDiagnostics
	{
		internal static Diagnostic Create(Exception e) =>
			Diagnostic.Create(new(UnexpectedExceptionDiagnostics.Id, UnexpectedExceptionDiagnostics.Title,
				e.ToString().Replace(Environment.NewLine, " : "),
				DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					UnexpectedExceptionDiagnostics.Id, UnexpectedExceptionDiagnostics.Title)), null);

		public const string Id = "PA1";
		public const string Message = "An unexpected exception has occurred";
		public const string Title = "Unexpected Exception";
	}
}