using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using PartiallyApplied.Diagnostics;
using System.Globalization;

namespace PartiallyApplied.Tests.Diagnostics
{
	public static class IncorrectApplyArgumentCountDiagnosticTests
	{
		[Test]
		public static void Create()
		{
			var node = SyntaxFactory.ClassDeclaration("Hi");
			var diagnostic = IncorrectApplyArgumentCountDiagnostic.Create(node);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostic.GetMessage(), Is.EqualTo(IncorrectApplyArgumentCountDiagnostic.Message));
				Assert.That(diagnostic.Descriptor.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(IncorrectApplyArgumentCountDiagnostic.Title));
				Assert.That(diagnostic.Id, Is.EqualTo(IncorrectApplyArgumentCountDiagnostic.Id));
				Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Error));
				Assert.That(diagnostic.Location, Is.EqualTo(node.GetLocation()));
			});
		}
	}
}