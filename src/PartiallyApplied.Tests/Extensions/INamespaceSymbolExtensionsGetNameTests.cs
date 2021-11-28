﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Tests.Extensions;

public static class INamespaceSymbolExtensionsGetNameTests
{
	[Test]
	public static void GetName()
	{
		var namespaceValue = "MyNamespace.Stuff";
		var code = $"namespace {namespaceValue} {{ }}";
		var @namespace = INamespaceSymbolExtensionsGetNameTests.GetNamespaceSymbol(code);
		Assert.That(@namespace.GetName(), Is.EqualTo(namespaceValue));
	}

	[Test]
	public static void GetNameWhenNamespaceIsNull() =>
		Assert.That((null as INamespaceSymbol).GetName(), Is.EqualTo(string.Empty));

	private static INamespaceSymbol GetNamespaceSymbol(string source)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);
		var references = AppDomain.CurrentDomain.GetAssemblies()
			.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
			.Select(_ => MetadataReference.CreateFromFile(_.Location));
		var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { syntaxTree },
			references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		var model = compilation.GetSemanticModel(syntaxTree, true);

		var typeSyntax = syntaxTree.GetRoot().DescendantNodes(_ => true)
			.OfType<NamespaceDeclarationSyntax>().Single();
		return model.GetDeclaredSymbol(typeSyntax)!;
	}
}