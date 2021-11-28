using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;

namespace PartiallyApplied.Tests;
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
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
			Assert.That(resultA, Is.Not.EqualTo(resultB), "resultA.Equals(resultB)");
			Assert.That(resultA == resultB, Is.False, "resultA == resultB");
			Assert.That(resultA != resultB, Is.True, "resultA != resultB");
			Assert.That(resultA.GetHashCode(), Is.Not.EqualTo(resultB.GetHashCode()), "resultA.GetHashCode() == resultB.GetHashCode()");

			Assert.That(resultA, Is.EqualTo(resultC), "resultA != resultC");
			Assert.That(resultA == resultC, Is.True, "resultA == resultC");
			Assert.That(resultA != resultC, Is.False, "resultA != resultC");
			Assert.That(resultA.GetHashCode(), Is.EqualTo(resultC.GetHashCode()), "resultA.GetHashCode() == resultC.GetHashCode()");

			Assert.That(resultB, Is.Not.EqualTo(resultC), "resultB == resultC");
			Assert.That(resultB == resultC, Is.False, "resultB == resultC");
			Assert.That(resultB != resultC, Is.True, "resultB != resultC");
			Assert.That(resultB.GetHashCode(), Is.Not.EqualTo(resultC.GetHashCode()), "resultB.GetHashCode() == resultC.GetHashCode()");
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
			Assert.That(resultA, Is.Not.EqualTo(resultB), "resultA.Equals(resultB)");
			Assert.That(resultA == resultB, Is.False, "resultA == resultB");
			Assert.That(resultA != resultB, Is.True, "resultA != resultB");
			Assert.That(resultA.GetHashCode(), Is.Not.EqualTo(resultB.GetHashCode()), "resultA.GetHashCode() == resultB.GetHashCode()");

			Assert.That(resultA, Is.EqualTo(resultC), "resultA != resultC");
			Assert.That(resultA == resultC, Is.True, "resultA == resultC");
			Assert.That(resultA != resultC, Is.False, "resultA != resultC");
			Assert.That(resultA.GetHashCode(), Is.EqualTo(resultC.GetHashCode()), "resultA.GetHashCode() == resultC.GetHashCode()");

			Assert.That(resultB, Is.Not.EqualTo(resultC), "resultB == resultC");
			Assert.That(resultB == resultC, Is.False, "resultB == resultC");
			Assert.That(resultB != resultC, Is.True, "resultB != resultC");
			Assert.That(resultB.GetHashCode(), Is.Not.EqualTo(resultC.GetHashCode()), "resultB.GetHashCode() == resultC.GetHashCode()");
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
			Assert.That(resultA, Is.Not.EqualTo(resultB), "resultA.Equals(resultB)");
			Assert.That(resultA == resultB, Is.False, "resultA == resultB");
			Assert.That(resultA != resultB, Is.True, "resultA != resultB");
			Assert.That(resultA.GetHashCode(), Is.Not.EqualTo(resultB.GetHashCode()), "resultA.GetHashCode() == resultB.GetHashCode()");

			Assert.That(resultA, Is.EqualTo(resultC), "resultA != resultC");
			Assert.That(resultA == resultC, Is.True, "resultA == resultC");
			Assert.That(resultA != resultC, Is.False, "resultA != resultC");
			Assert.That(resultA.GetHashCode(), Is.EqualTo(resultC.GetHashCode()), "resultA.GetHashCode() == resultC.GetHashCode()");

			Assert.That(resultB, Is.Not.EqualTo(resultC), "resultB == resultC");
			Assert.That(resultB == resultC, Is.False, "resultB == resultC");
			Assert.That(resultB != resultC, Is.True, "resultB != resultC");
			Assert.That(resultB.GetHashCode(), Is.Not.EqualTo(resultC.GetHashCode()), "resultB.GetHashCode() == resultC.GetHashCode()");
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