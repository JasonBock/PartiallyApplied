using NUnit.Framework;

namespace PartiallyApplied.Tests;

internal static class PartiallyAppliedGeneratorTests
{
	[Test]
	public static async Task GenerateAsync()
	{
		var code =
			"""
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
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public static global::System.Func<int, int> Apply(global::System.Func<int, int, int> method, int a) =>
					new((b) => method(a, b));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithParameterTypeInNamespaceAsync()
	{
		var code =
			"""
			using System;

			namespace SomeNamespace
			{
				public sealed record Identity(Guid Id);
			}

			namespace PartiallyTests
			{
				public static class Worker
				{
					public static void Work(int a, SomeNamespace.Identity identity) { }
				}

				public static class Test
				{
					public static void Generate()
					{
						var workWith3 = Partially.Apply(Worker.Work, 3);
					}
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public static global::System.Action<global::SomeNamespace.Identity> Apply(global::System.Action<int, global::SomeNamespace.Identity> method, int a) =>
					new((identity) => method(a, identity));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenGenericsExistForStandardMethodAsync()
	{
		var code =
			"""
			namespace PartiallyTests
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
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public static global::System.Action<T> Apply<T>(global::System.Action<int, T> method, int a) =>
					new((b) => method(a, b));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenGenericsExistForNonStandardMethodAsync()
	{
		var code =
			"""
			using System;

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
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public delegate void Target_D6F274AC_Delegate<T>(int value, global::System.Span<int> buffer, T value2);
				public delegate void Apply_D6F274AC_Delegate<T>(global::System.Span<int> buffer, T value2);
				public static Apply_D6F274AC_Delegate<T> Apply<T>(Target_D6F274AC_Delegate<T> method, int value) =>
					new((buffer, value2) => method(value, buffer, value2));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateUsingApplyRefReturnAsync()
	{
		var code =
			"""
			namespace PartiallyTests
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
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public delegate ref int Target_6B76E192_Delegate(int a, int b);
				public delegate ref int Apply_6B76E192_Delegate(int b);
				public static Apply_6B76E192_Delegate ApplyWithRefReturn(Target_6B76E192_Delegate method, int a) =>
					new((b) => ref method(a, b));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateUsingApplyRefReadonlyReturnAsync()
	{
		var code =
			"""
			namespace PartiallyTests
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
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public delegate ref readonly int Target_11D24770_Delegate(int a, int b);
				public delegate ref readonly int Apply_11D24770_Delegate(int b);
				public static Apply_11D24770_Delegate ApplyWithRefReadonlyReturn(Target_11D24770_Delegate method, int a) =>
					new((b) => ref method(a, b));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenInvocationExistsAsync()
	{
		var code =
			"""
			using System;

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
			}
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[],
			[]);
	}

	[Test]
	public static async Task GenerateWhenDuplicatesExistAsync()
	{
		var code =
			"""
			namespace PartiallyTests
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
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public static global::System.Func<int, int> Apply(global::System.Func<int, int, int> method, int a) =>
					new((b) => method(a, b));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenOverloadsExistAsync()
	{
		var code =
			"""
			using System;

			namespace PartiallyTests
			{
				public static class Overloads
				{
					public static void Foo(Guid a, int b, int c, Guid d) { }
					public static void Foo(int a, string b, Guid c) { }
				}

				public static class Test
				{
					public static void Generate()
					{
						var method = Partially.Apply(Overloads.Foo, 3);
					}
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public static global::System.Action<string, global::System.Guid> Apply(global::System.Action<int, string, global::System.Guid> method, int a) =>
					new((b, c) => method(a, b, c));
				public static global::System.Action<int, int, global::System.Guid> Apply(global::System.Action<global::System.Guid, int, int, global::System.Guid> method, global::System.Guid a) =>
					new((b, c, d) => method(a, b, c, d));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenAnonymousMethodExistsAsync()
	{
		var code =
			"""
			namespace PartiallyTests
			{
				public static class Test
				{
					public static void Generate()
					{
						var func = (int a, int b) => a + b;
						var combineWith3 = Partially.Apply(func, 3);
					}
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public static global::System.Func<int, int> Apply(global::System.Func<int, int, int> method, int arg1) =>
					new((arg2) => method(arg1, arg2));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenMethodIsSystemDelegateTypeAsync()
	{
		var code =
			"""
			using System;

			namespace PartiallyTests
			{
			   public class Functions
			   {
			      public static int Add(int a, int b) => a + b;
			   }

			   public static class Test
			   {
			      public static void Generate()
			      {
			         Func<int, int, int> func = Functions.Add;
			         var combineWith3 = Partially.Apply(func, 3);
			      }
			   }
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public static global::System.Func<int, int> Apply(global::System.Func<int, int, int> method, int arg1) =>
					new((arg2) => method(arg1, arg2));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenMethodIsSystemActionDelegateTypeAsync()
	{
		var code =
			"""
			using System;

			namespace PartiallyTests
			{
				public class Functions
			   {
					public static void PrintSum(int a, int b) => Console.WriteLine(a + b);
				}

				public static class Test
				{
					public static void Generate()
					{
						Action<int, int> func = Functions.PrintSum;
						var combineWith3 = Partially.Apply(func, 3);
					}
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			public static partial class Partially
			{
				public static global::System.Action<int> Apply(global::System.Action<int, int> method, int arg1) =>
					new((arg2) => method(arg1, arg2));
			}
			
			""";

		await TestAssistants.RunGeneratorAsync<PartiallyAppliedGenerator>(code,
			[(Shared.GeneratedFileName, generatedCode)],
			[]);
	}
}