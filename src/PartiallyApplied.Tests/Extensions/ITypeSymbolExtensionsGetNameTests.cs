using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System;
using System.Linq;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Tests.Extensions
{
	public static class ITypeSymbolExtensionsGetNameTests
	{
		[Test]
		public static void GetName()
		{
			var typeSymbol = ITypeSymbolExtensionsGetNameTests.GetTypeSymbol("public class Target { }");
			var name = typeSymbol.GetName();

			Assert.That(name, Is.EqualTo("Target"));
		}

		private static ITypeSymbol GetTypeSymbol(string source)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			var model = compilation.GetSemanticModel(syntaxTree, true);

			var typeSyntax = syntaxTree.GetRoot().DescendantNodes(_ => true)
				.OfType<TypeDeclarationSyntax>().Where(_ => _.Identifier.Text == "Target").Single();
			return model.GetDeclaredSymbol(typeSyntax)!;
		}
	}
}