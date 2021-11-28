using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace PartiallyApplied.Tests;

public static class PartiallyAppliedReceiverTests
{
	private static IEnumerable<TestCaseData> GetMethodCalls()
	{
		yield return new($"{Naming.PartiallyClassName}.{Naming.ApplyMethodName}();", 1);
		yield return new($"{Naming.PartiallyClassName}.{Naming.ApplyMethodName}WithSpecialText();", 1);
		yield return new("string.IsNullOrEmpty(\"a\");", 0);
	}

	[TestCaseSource(nameof(PartiallyAppliedReceiverTests.GetMethodCalls))]
	public static async Task FindCandidateAsync(string code, int expectedCount)
	{
		var classDeclaration = (await SyntaxFactory.ParseSyntaxTree(code)
			.GetRootAsync().ConfigureAwait(false)).DescendantNodes(_ => true).OfType<InvocationExpressionSyntax>().First();

		var receiver = new PartiallyAppliedReceiver();
		receiver.OnVisitSyntaxNode(classDeclaration);

		Assert.Multiple(() =>
		{
			Assert.That(receiver.Candidates.Count, Is.EqualTo(expectedCount));
		});
	}
}