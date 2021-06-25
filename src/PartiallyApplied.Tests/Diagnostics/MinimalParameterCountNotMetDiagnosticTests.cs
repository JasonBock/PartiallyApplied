using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using PartiallyApplied.Diagnostics;
using System.Globalization;

namespace PartiallyApplied.Tests.Diagnostics
{
	public static class MinimalParameterCountNotMetDiagnosticTests
	{
		[Test]
		public static void Create()
		{
			var node = SyntaxFactory.ClassDeclaration("Hi");
			var diagnostic = MinimalParameterCountNotMetDiagnostic.Create(node);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostic.GetMessage(), Is.EqualTo(MinimalParameterCountNotMetDiagnostic.Message));
				Assert.That(diagnostic.Descriptor.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(MinimalParameterCountNotMetDiagnostic.Title));
				Assert.That(diagnostic.Id, Is.EqualTo(MinimalParameterCountNotMetDiagnostic.Id));
				Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Error));
				Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);
				Assert.That(diagnostic.Location, Is.EqualTo(node.GetLocation()));
			});
		}
	}
}