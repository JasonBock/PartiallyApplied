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
			const string methodName = "Foo";
			var symbol = PartiallyAppliedInformationResultTests.GetMethodSymbol(
				$"public class Target {{ public void {methodName}() {{ }} }}", methodName);
			var result = new PartiallyAppliedInformationResult(symbol, 2, Naming.ApplyMethodName);

			Assert.Multiple(() =>
			{
				Assert.That(result.ApplyName, Is.EqualTo(Naming.ApplyMethodName));
				Assert.That(result.Target, Is.SameAs(symbol));
				Assert.That(result.PartialArgumentCount, Is.EqualTo(2));
			});
		}

		[Test]
		public static void CheckEqualityWithSymbolDifferences()
		{
			const string methodNameA = "Foo";
			const string methodNameB = "Bar";
			const string methodNameC = "Quux";
			var symbolA = PartiallyAppliedInformationResultTests.GetMethodSymbol(
				$"public class Target {{ public void {methodNameA}() {{ }} }}", methodNameA);
			var symbolB = PartiallyAppliedInformationResultTests.GetMethodSymbol(
				$"public class Target {{ public void {methodNameB}(int a) {{ }} }}", methodNameB);
			var symbolC = PartiallyAppliedInformationResultTests.GetMethodSymbol(
				$"public class Target {{ public void {methodNameC}() {{ }} }}", methodNameC);
			var resultA = new PartiallyAppliedInformationResult(symbolA, 2, Naming.ApplyMethodName);
			var resultB = new PartiallyAppliedInformationResult(symbolB, 2, Naming.ApplyMethodName);
			var resultC = new PartiallyAppliedInformationResult(symbolC, 2, Naming.ApplyMethodName);

			Assert.Multiple(() =>
			{
				Assert.That(resultA, Is.Not.EqualTo(resultB), "resultA == resultB");
				Assert.That(resultA, Is.EqualTo(resultC), "resultA != resultC");
				Assert.That(resultB, Is.Not.EqualTo(resultC), "resultB == resultC");
			});
		}

		[Test]
		public static void CheckEqualityWithCountDifferences()
		{
			const string methodName = "Foo";
			var symbol = PartiallyAppliedInformationResultTests.GetMethodSymbol(
				$"public class Target {{ public void {methodName}() {{ }} }}", methodName);
			var resultA = new PartiallyAppliedInformationResult(symbol, 2, Naming.ApplyMethodName);
			var resultB = new PartiallyAppliedInformationResult(symbol, 3, Naming.ApplyMethodName);
			var resultC = new PartiallyAppliedInformationResult(symbol, 2, Naming.ApplyMethodName);

			Assert.Multiple(() =>
			{
				Assert.That(resultA, Is.Not.EqualTo(resultB), "resultA == resultB");
				Assert.That(resultA, Is.EqualTo(resultC), "resultA != resultC");
				Assert.That(resultB, Is.Not.EqualTo(resultC), "resultB == resultC");
			});
		}

		[Test]
		public static void CheckEqualityWithNameDifferences()
		{
			const string methodName = "Foo";
			var symbol = PartiallyAppliedInformationResultTests.GetMethodSymbol(
				$"public class Target {{ public void {methodName}() {{ }} }}", methodName);
			var resultA = new PartiallyAppliedInformationResult(symbol, 2, Naming.ApplyMethodName);
			var resultB = new PartiallyAppliedInformationResult(symbol, 2, $"{Naming.ApplyMethodName}WithRefReturn");
			var resultC = new PartiallyAppliedInformationResult(symbol, 2, Naming.ApplyMethodName);

			Assert.Multiple(() =>
			{
				Assert.That(resultA, Is.Not.EqualTo(resultB), "resultA == resultB");
				Assert.That(resultA, Is.EqualTo(resultC), "resultA != resultC");
				Assert.That(resultB, Is.Not.EqualTo(resultC), "resultB == resultC");
			});
		}

		private static IMethodSymbol GetMethodSymbol(string source, string methodName)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			var model = compilation.GetSemanticModel(syntaxTree, true);

			var methodSyntax = syntaxTree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>().Where(_ => _.Identifier.Text == methodName).Single();
			return model.GetDeclaredSymbol(methodSyntax)!;
		}
	}
}