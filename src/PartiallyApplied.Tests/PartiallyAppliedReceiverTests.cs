using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Linq;

namespace PartiallyApplied.Tests
{
	public static class PartiallyAppliedReceiverTests
	{
		[Test]
		public static async Task FindCandidatesWhenInvocationIsPartiallyCreate()
		{
			var classDeclaration = (await SyntaxFactory.ParseSyntaxTree($"{Naming.PartiallyClassName}.{Naming.PartiallyMethodName}();")
				.GetRootAsync().ConfigureAwait(false)).DescendantNodes(_ => true).OfType<InvocationExpressionSyntax>().First();

			var receiver = new PartiallyAppliedReceiver();
			receiver.OnVisitSyntaxNode(classDeclaration);

			Assert.Multiple(() =>
			{
				Assert.That(receiver.Candidates.Count, Is.EqualTo(1));
			});
		}

		[Test]
		public static async Task FindCandidatesWhenInvocationIsNotPartiallyCreate()
		{
			var classDeclaration = (await SyntaxFactory.ParseSyntaxTree("string.IsNullOrEmpty(\"a\");")
				.GetRootAsync().ConfigureAwait(false)).DescendantNodes(_ => true).OfType<InvocationExpressionSyntax>().First();

			var receiver = new PartiallyAppliedReceiver();
			receiver.OnVisitSyntaxNode(classDeclaration);

			Assert.Multiple(() =>
			{
				Assert.That(receiver.Candidates.Count, Is.EqualTo(0));
			});
		}
	}
}