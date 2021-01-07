using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace PartiallyApplied.Tests
{
	public static class PartiallyAppliedReceiverTests
	{
		private static IEnumerable<TestCaseData> GetMethodCalls()
		{
			yield return new($"{Naming.PartiallyClassName}.{Naming.ApplyMethodName}();", 1);
			yield return new($"{Naming.PartiallyClassName}.{Naming.ApplyWithRefReturnMethodName}();", 1);
			yield return new($"{Naming.PartiallyClassName}.{Naming.ApplyWithRefReadonlyReturnMethodName}();", 1);
			yield return new("string.IsNullOrEmpty(\"a\");", 0);
		}

		[TestCaseSource(nameof(PartiallyAppliedReceiverTests.GetMethodCalls))]
		public static async Task FindCandidate(string code, int expectedCount)
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
}