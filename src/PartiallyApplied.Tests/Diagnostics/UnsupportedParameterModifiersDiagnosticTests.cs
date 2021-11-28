using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using PartiallyApplied.Diagnostics;
using System.Globalization;

namespace PartiallyApplied.Tests.Diagnostics;

public static class UnsupportedParameterModifiersDiagnosticTests
{
	[Test]
	public static void Create()
	{
		var node = SyntaxFactory.ClassDeclaration("Hi");
		var diagnostic = UnsupportedParameterModifiersDiagnostic.Create(node);

		Assert.Multiple(() =>
		{
			Assert.That(diagnostic.GetMessage(), Is.EqualTo(UnsupportedParameterModifiersDiagnostic.Message));
			Assert.That(diagnostic.Descriptor.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(UnsupportedParameterModifiersDiagnostic.Title));
			Assert.That(diagnostic.Id, Is.EqualTo(UnsupportedParameterModifiersDiagnostic.Id));
			Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Error));
			Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);
			Assert.That(diagnostic.Location, Is.EqualTo(node.GetLocation()));
		});
	}
}