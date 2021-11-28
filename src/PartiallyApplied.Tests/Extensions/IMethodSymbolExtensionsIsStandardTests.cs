using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Tests.Extensions;

public static class IMethodSymbolExtensionsIsStandardTests
{
	[TestCase("public class Target { public void Foo() { } }", true)]
	[TestCase("public class Target { public void Foo(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15, int a16, int a17) { } }", false)]
	[TestCase("using System; public class Target { public void Foo(Span<int> buffer) { } }", false)]
	[TestCase("public class Target { public void Foo(int a = 3) { } }", false)]
	[TestCase("public class Target { public ref int Foo() { } }", false)]
	[TestCase("public class Target { public ref readonly int Foo() { } }", false)]
	public static void IsStandard(string code, bool expectedResult)
	{
		var methodSymbol = IMethodSymbolExtensionsIsStandardTests.GetMethodSymbol(code);
		Assert.That(methodSymbol.IsStandard(), Is.EqualTo(expectedResult));
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