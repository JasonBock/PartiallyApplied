﻿using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System.Collections.Immutable;
using System;
using System.Linq;

namespace PartiallyApplied.Tests
{
	public static class PartiallyAppliedGeneratorTests
	{
		[Test]
		public static void x()
		{
			var (diagnostics, output) = PartiallyAppliedGeneratorTests.GetGeneratedOutput(
@"using System;

namespace MockTests
{
	public static class Maths
	{
		public static int Add(int a, Span<int> b) => b.Contains(a);
	}

	public static class Test
	{
		public static void Generate()
		{
			var incrementBy3 = Partially.Apply(Maths.Add, 3);
		}
	}
}");

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(0));
				Assert.That(output, Does.Contain("public static partial class Partially"));
			});
		}

		[Test]
		public static void GenerateWhenInvocationDoesNotExist()
		{
			var (diagnostics, output) = PartiallyAppliedGeneratorTests.GetGeneratedOutput(
@"namespace MockTests
{
	public static class Maths
	{
		public static int Add(int a, int b) => a + b;
	}

	public static class Test
	{
		public static void Generate()
		{
			var incrementBy3 = Partially.Apply(Maths.Add, 3);
		}
	}
}");

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(0));
				Assert.That(output, Does.Contain("public static partial class Partially"));
			});
		}

		[Test]
		public static void GenerateWhenInvocationExists()
		{
			var (diagnostics, output) = PartiallyAppliedGeneratorTests.GetGeneratedOutput(
@"using System;

namespace MockTests
{
	public static class Maths
	{
		public static int Add(int a, int b) => a + b;
	}

	public static class Test
	{
		public static void Generate()
		{
			var incrementBy3 = Partially.Apply(Maths.Add, 3);
		}
	}
}

public static partial class Partially
{
	public static Func<int, int> Apply(Func<int, int, int> method, int a) =>
		new((b) => method(a, b));
}");

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(0));
				Assert.That(output, Does.Not.Contain("public static partial class Partially"));
			});
		}

		[Test]
		public static void GenerateWhenDuplicatesExist()
		{
			var (diagnostics, output) = PartiallyAppliedGeneratorTests.GetGeneratedOutput(
@"namespace MockTests
{
	public static class Maths
	{
		public static int Add(int a, int b) => a + b;
		public static int Multiply(int a, int b) => a * b;
	}

	public static class Test
	{
		public static void Generate()
		{
			var incrementBy3 = Partially.Apply(Maths.Add, 3);
			var tripler = Partially.Apply(Maths.Multiply, 3);
		}
	}
}");

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(0));
				Assert.That(output, Does.Contain("public static partial class Partially"));
			});
		}

		private static (ImmutableArray<Diagnostic>, string) GetGeneratedOutput(string source, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location))
				.Concat(new[] { MetadataReference.CreateFromFile(typeof(PartiallyAppliedGenerator).Assembly.Location) });
			var compilation = CSharpCompilation.Create("apply", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(outputKind));
			var originalTreeCount = compilation.SyntaxTrees.Length;

			var generator = new PartiallyAppliedGenerator();

			var driver = CSharpGeneratorDriver.Create(ImmutableArray.Create<ISourceGenerator>(generator));
			driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

			var trees = outputCompilation.SyntaxTrees.ToList();

			return (diagnostics, trees.Count != originalTreeCount ? trees[^1].ToString() : string.Empty);
		}
	}
}