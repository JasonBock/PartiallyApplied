using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;
using System.Linq;

namespace PartiallyApplied.Tests
{
	public static class PartiallyAppliedInformationResultTests
	{
		[Test]
		public static void Create()
		{
			var symbol = PartiallyAppliedInformationResultTests.GetMethodSymbol(
				"public class Target { public void Foo() { } }");
			var result = new PartiallyAppliedInformationResult(symbol, 2);

			Assert.Multiple(() =>
			{
				Assert.That(result.Target, Is.SameAs(symbol));
				Assert.That(result.PartialArgumentCount, Is.EqualTo(2));
			});
		}

		private static IMethodSymbol GetMethodSymbol(string source)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			var model = compilation.GetSemanticModel(syntaxTree, true);

			var methodSyntax = syntaxTree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>().Where(_ => _.Identifier.Text == "Foo").Single();
			return model.GetDeclaredSymbol(methodSyntax)!;
		}
	}
}