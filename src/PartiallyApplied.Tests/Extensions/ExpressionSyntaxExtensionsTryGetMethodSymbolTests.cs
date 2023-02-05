using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Tests.Extensions;

public static class ExpressionSyntaxExtensionsTryGetMethodSymbolTests
{
	[Test]
	public static void TryGetMethodSymbolWhenMethodResolves()
	{
		var code =
		"""
		public static class Targets
		{
			public static void Foo() { }
		}

		public static class Runner
		{
			public static void Run()
			{
				Targets.Foo();
			}
		}
		""";

		var (expression, model) = ExpressionSyntaxExtensionsTryGetMethodSymbolTests.GetExpression<InvocationExpressionSyntax>(code);
		var (method, wasFound) = expression.TryGetMethodSymbols(model)[0];

		Assert.Multiple(() =>
		{
			Assert.That(method!.Name, Is.EqualTo("Foo"));
			Assert.That(wasFound, Is.True);
		});
	}

	[Test]
	public static void TryGetMethodSymbolWhenMethodIsACandidate()
	{
		var code =
			"""
			public static class Targets
			{
				public static void Foo() { }
			}

			public static class Runner
			{
				public static void Run()
				{
					Targets.Foo(3);
				}
			}
			""";

		var (expression, model) = ExpressionSyntaxExtensionsTryGetMethodSymbolTests.GetExpression<InvocationExpressionSyntax>(code);
		var (method, wasFound) = expression.TryGetMethodSymbols(model)[0];

		Assert.Multiple(() =>
		{
			Assert.That(method!.Name, Is.EqualTo("Foo"));
			Assert.That(wasFound, Is.False);
		});
	}

	[Test]
	public static void TryGetMethodSymbolWhenExpressionIsNotAMethod()
	{
		var code =
		"""
		public static class Targets
		{
			public static int Foo { get; }
		}

		public static class Runner
		{
			public static void Run()
			{
				var value = Targets.Foo;
			}
		}
		""";

		var (expression, model) = ExpressionSyntaxExtensionsTryGetMethodSymbolTests.GetExpression<MemberAccessExpressionSyntax>(code);
		var (method, wasFound) = expression.TryGetMethodSymbols(model)[0];

		Assert.Multiple(() =>
		{
			Assert.That(method, Is.Null);
			Assert.That(wasFound, Is.False);
		});
	}

	private static (ExpressionSyntax, SemanticModel) GetExpression<T>(string source)
		where T : ExpressionSyntax
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);
		var references = AppDomain.CurrentDomain.GetAssemblies()
			.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
			.Select(_ => MetadataReference.CreateFromFile(_.Location));
		var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { syntaxTree },
			references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

		var model = compilation.GetSemanticModel(syntaxTree, true);
		var expression = syntaxTree.GetRoot().DescendantNodes(_ => true)
			.OfType<T>().Single();

		return (expression, model);
	}
}