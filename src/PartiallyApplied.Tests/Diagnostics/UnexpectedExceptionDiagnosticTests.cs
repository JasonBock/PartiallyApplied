using Microsoft.CodeAnalysis;
using NUnit.Framework;
using PartiallyApplied.Diagnostics;
using System;
using System.Globalization;

namespace PartiallyApplied.Tests.Diagnostics
{
	public static class UnexpectedExceptionDiagnosticTests
	{
		[Test]
		public static void Create()
		{
			var exception = new Exception();
			var diagnostic = UnexpectedExceptionDiagnostic.Create(exception);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostic.GetMessage(), Is.EqualTo(exception.ToString().Replace(Environment.NewLine, " : ")));
				Assert.That(diagnostic.Descriptor.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(UnexpectedExceptionDiagnostic.Title));
				Assert.That(diagnostic.Id, Is.EqualTo(UnexpectedExceptionDiagnostic.Id));
				Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);
				Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Error));
			});
		}
	}
}