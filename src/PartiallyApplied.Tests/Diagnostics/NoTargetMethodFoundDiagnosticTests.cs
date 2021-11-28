using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using PartiallyApplied.Diagnostics;
using System.Globalization;

namespace PartiallyApplied.Tests.Diagnostics;

public static class NoTargetMethodFoundDiagnosticTests
{
	[Test]
	public static void Create()
	{
		var node = SyntaxFactory.ClassDeclaration("Hi");
		var diagnostic = NoTargetMethodFoundDiagnostic.Create(node);

		Assert.Multiple(() =>
		{
			Assert.That(diagnostic.GetMessage(), Is.EqualTo(NoTargetMethodFoundDiagnostic.Message));
			Assert.That(diagnostic.Descriptor.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(NoTargetMethodFoundDiagnostic.Title));
			Assert.That(diagnostic.Id, Is.EqualTo(NoTargetMethodFoundDiagnostic.Id));
			Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Error));
			Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);
			Assert.That(diagnostic.Location, Is.EqualTo(node.GetLocation()));
		});
	}
}