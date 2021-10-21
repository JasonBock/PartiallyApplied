using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System.Collections.Immutable;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;

namespace PartiallyApplied.Tests
{
	[NonParallelizable]
	public static class PartiallyAppliedGeneratorTests
	{
		[Test]
		public static async Task GenerateWhenGenericsExistForStandardMethodAsync()
		{
			var code =
@"namespace PartiallyTests
{
	public static class Maths
	{
		public static void Combine<T>(int a, T b) { } 
	}

	public static class Test
	{
		public static void Generate()
		{
			var combineWith3 = Partially.Apply<int>(Maths.Combine, 3);
		}
	}
}";

			var generatedCode =
@"using PartiallyTests;
using System;

#nullable enable
public static partial class Partially
{
	public static Action<T> Apply<T>(Action<int, T> method, int a) =>
		new((b) => method(a, b));
}
";

			await TestAssistants.RunAsync<PartiallyAppliedGenerator>(code,
				new[] { (typeof(PartiallyAppliedGenerator), Shared.GeneratedFileName, generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		[Order(1)]
		public static async Task GenerateWhenGenericsExistForNonStandardMethodAsync()
		{
			var code =
@"using System;

namespace PartiallyTests
{
	public static class Maths
	{
		public static void Contains<T>(int value, Span<int> buffer, T value2) { }
	}

	public static class Test
	{
		public static void Generate()
		{
			var combineWith3 = Partially.Apply<int>(Maths.Contains, 3);
		}
	}
}";

			var generatedCode =
@"using PartiallyTests;
using System;

#nullable enable
public static partial class Partially
{
	public delegate void Target_1_Delegate<T>(int value, Span<int> buffer, T value2);
	public delegate void Apply_1_Delegate<T>(Span<int> buffer, T value2);
	public static Apply_1_Delegate<T> Apply<T>(Target_1_Delegate<T> method, int value) =>
		new((buffer, value2) => method(value, buffer, value2));
}
";

			await TestAssistants.RunAsync<PartiallyAppliedGenerator>(code,
				new[] { (typeof(PartiallyAppliedGenerator), Shared.GeneratedFileName, generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		[Order(2)]
		public static async Task GenerateUsingApplyRefReturnAsync()
		{
			var code =
@"namespace PartiallyTests
{
	public static class Maths
	{
		private static int refReturn;

		public static ref int Add(int a, int b)
		{
			Maths.refReturn = a + b + Maths.refReturn;
			return ref Maths.refReturn;
		}
	}

	public static class Test
	{
		public static void Generate()
		{
			var incrementBy3 = Partially.ApplyWithRefReturn(Maths.Add, 3);
		}
	}
}";

			var generatedCode =
@"using System;

#nullable enable
public static partial class Partially
{
	public delegate ref int Target_2_Delegate(int a, int b);
	public delegate ref int Apply_2_Delegate(int b);
	public static Apply_1_Delegate ApplyWithRefReturn(Target_2_Delegate method, int a) =>
		new((b) => ref method(a, b));
}
";

			await TestAssistants.RunAsync<PartiallyAppliedGenerator>(code,
				new[] { (typeof(PartiallyAppliedGenerator), Shared.GeneratedFileName, generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		[Order(3)]
		public static async Task GenerateUsingApplyRefReadonlyReturnAsync()
		{
			var code =
@"namespace PartiallTests
{
	public static class Maths
	{
		private static int refReturn;

		public static ref readonly int Add(int a, int b)
		{
			Maths.refReturn = a + b + Maths.refReturn;
			return ref Maths.refReturn;
		}
	}

	public static class Test
	{
		public static void Generate()
		{
			var incrementBy3 = Partially.ApplyWithRefReadonlyReturn(Maths.Add, 3);
		}
	}
}";

			var generatedCode =
@"using System;

#nullable enable
public static partial class Partially
{
	public delegate ref readonly int Target_3_Delegate(int a, int b);
	public delegate ref readonly int Apply_3_Delegate(int b);
	public static Apply_1_Delegate ApplyWithRefReadonlyReturn(Target_3_Delegate method, int a) =>
		new((b) => ref method(a, b));
}
";

			await TestAssistants.RunAsync<PartiallyAppliedGenerator>(code,
				new[] { (typeof(PartiallyAppliedGenerator), Shared.GeneratedFileName, generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		public static async Task GenerateWhenInvocationDoesNotExistAsync()
		{
			var code =
@"namespace PartiallyTests
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
}";

			var generatedCode =
@"using System;

#nullable enable
public static partial class Partially
{
	public static Func<int, int> Apply(Func<int, int, int> method, int a) =>
		new((b) => method(a, b));
}
";

			await TestAssistants.RunAsync<PartiallyAppliedGenerator>(code,
				new[] { (typeof(PartiallyAppliedGenerator), Shared.GeneratedFileName, generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		public static async Task GenerateWhenInvocationExistsAsync()
		{
			var code =
@"using System;

namespace PartiallyTests
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
}";

			await TestAssistants.RunAsync<PartiallyAppliedGenerator>(code, 
				Array.Empty<(Type, string, string)>(),
				Enumerable.Empty<DiagnosticResult>());
		}

		[Test]
		public static async Task GenerateWhenDuplicatesExistAsync()
		{
			var code =
@"namespace PartiallyTests
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
}";

			var generatedCode =
@"using System;

#nullable enable
public static partial class Partially
{
	public static Func<int, int> Apply(Func<int, int, int> method, int a) =>
		new((b) => method(a, b));
}
";

			await TestAssistants.RunAsync<PartiallyAppliedGenerator>(code,
				new[] { (typeof(PartiallyAppliedGenerator), Shared.GeneratedFileName, generatedCode) },
				Enumerable.Empty<DiagnosticResult>());
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