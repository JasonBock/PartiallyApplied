using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;
using System.Linq;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Tests.Extensions
{
	public static class IMethodSymbolExtensionsAreEqualTests
	{
		[TestCase("public class Target { public void Foo() { } public void Bar() { } }", true)]
		[TestCase("public class Target { public int Foo() => 3; public void Bar() { } }", false)]
		[TestCase("public class Target { public void Foo() { }; public int Bar() => 3; }", false)]
		[TestCase("public class Target { public int Foo() => 3; public double Bar() => 3.2; }", false)]
		[TestCase("public class Target { public void Foo(int a) { } public void Bar(int b, string c) { } }", false)]
		[TestCase("public class Target { public void Foo(int a) { } public void Bar(string c) { } }", false)]
		public static void IsStandard(string code, bool expectedResult)
		{
			var (methodSymbol1, methodSymbol2) = IMethodSymbolExtensionsAreEqualTests.GetMethodSymbols(code);
			Assert.That(methodSymbol1.AreEqual(methodSymbol2), Is.EqualTo(expectedResult));
		}

		private static (IMethodSymbol, IMethodSymbol) GetMethodSymbols(string source)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			var model = compilation.GetSemanticModel(syntaxTree, true);

			var methodSyntax1 = syntaxTree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>().Where(_ => _.Identifier.Text == "Foo").Single();
			var methodSyntax2 = syntaxTree.GetRoot().DescendantNodes(_ => true)
				.OfType<MethodDeclarationSyntax>().Where(_ => _.Identifier.Text == "Bar").Single();
			var methodSymbol1 = model.GetDeclaredSymbol(methodSyntax1)!;
			var methodSymbol2 = model.GetDeclaredSymbol(methodSyntax2)!;

			return (methodSymbol1, methodSymbol2);
		}
	}
}