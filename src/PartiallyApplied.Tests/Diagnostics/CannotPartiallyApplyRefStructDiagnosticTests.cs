using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using PartiallyApplied.Diagnostics;
using System.Globalization;

namespace PartiallyApplied.Tests.Diagnostics
{
	public static class CannotPartiallyApplyRefStructDiagnosticTests
	{
		[Test]
		public static void Create() 
		{
			var node = SyntaxFactory.ClassDeclaration("Hi");
			var diagnostic = CannotPartiallyApplyRefStructDiagnostic.Create(node);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostic.GetMessage(), Is.EqualTo(CannotPartiallyApplyRefStructDiagnostic.Message));
				Assert.That(diagnostic.Descriptor.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(CannotPartiallyApplyRefStructDiagnostic.Title));
				Assert.That(diagnostic.Id, Is.EqualTo(CannotPartiallyApplyRefStructDiagnostic.Id));
				Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Error));
				Assert.That(diagnostic.Location, Is.EqualTo(node.GetLocation()));
			});
		}
	}
}