using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using PartiallyApplied.Diagnostics;
using System.Collections.Immutable;

namespace PartiallyApplied.Tests;

public static class PartiallyAppliedInformationTests
{
	[Test]
	public static void CreateWhenTargetHasRefParameters()
	{
		var code =
 @"public static class Maths
{
	public static int AddByRef(int a, ref int b) => a + b;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.AddByRef, 3);
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Any(_ => _.Id == UnsupportedParameterModifiersDiagnostic.Id), Is.True);
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenTargetHasOutParameters()
	{
		var code =
 @"public static class Maths
{
	public static int AddByOut(int a, out int b) => a + b;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.AddByOut, 2);
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Any(_ => _.Id == UnsupportedParameterModifiersDiagnostic.Id), Is.True);
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenTargetHasInParameters()
	{
		var code =
 @"public static class Maths
{
	public static int AddByIn(int a, in int b) => a + b;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.AddByIn, 3);
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Any(_ => _.Id == UnsupportedParameterModifiersDiagnostic.Id), Is.True);
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenTargetHasRefStructParametersBeingPartiallyApplied()
	{
		var code =
 @"using System;

public static class Maths
{
	public static int Contains(Span<int> a, int b) => a.Contains(b);
}

public static class Runner
{
	public static void Run()
	{
		var buffer = new Span<int>(new [] { 3 });
		Partially.Apply(Maths.Contains, buffer);
	}
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Any(_ => _.Id == CannotPartiallyApplyRefStructDiagnostic.Id), Is.True);
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenTargetHasRefStructParametersNotBeingPartiallyApplied()
	{
		var code =
 @"using System;

public static class Maths
{
	public static int Contains(int a, Span<int> b) => b.Contains(a);
}

public static class Runner
{
	public static void Run()
	{
		var buffer = new Span<int>(new [] { 3 });
		Partially.Apply(Maths.Contains, 3);
	}
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Length, Is.EqualTo(0));
			Assert.That(information.Results.Length, Is.GreaterThan(0));
			var result = information.Results[0];
			Assert.That(result.Target.Name, Is.EqualTo("Contains"));
			Assert.That(result.PartialArgumentCount, Is.EqualTo(1));
		});
	}

	[Test]
	public static void CreateWhenApplyHasLessThan2Parameters()
	{
		var code =
 @"public static class Maths
{
	public static int Add(int a, int b) => a + b;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.Add);
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Any(_ => _.Id == IncorrectApplyArgumentCountDiagnostic.Id), Is.True);
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenMethodDoesNotExist()
	{
		var code =
 @"public static class Maths
{
	public static int Add(int a, int b) => a + b;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.Subtract, 3);
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Any(_ => _.Id == NoTargetMethodFoundDiagnostic.Id), Is.True);
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenMethodOnlyHas1Parameter()
	{
		var code =
 @"public static class Maths
{
	public static int Reflect(int a) => a;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.Reflect, 3);
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Any(_ => _.Id == MinimalParameterCountNotMetDiagnostic.Id), Is.True);
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenTooManyArgumentsArePassed()
	{
		var code =
 @"public static class Maths
{
	public static int Add(int a, int b) => a + b;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.Add, 3, 4);
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);
		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Any(_ => _.Id == TooManyArgumentsDiagnostic.Id), Is.True);
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenApplyMethodExists()
	{
		var code =
 @"public static class Maths
{
	public static int Add(int a, int b) => a + b;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.Add, 3);
}

public static class Partially
{
	public delegate int Base_1_Delegate(int a, int b);
	public delegate int Base_1_Apply_1_Delegate(int b);

	public static Base_1_Apply_1_Delegate Apply(Base_1_Delegate method, int a)
	{
		int Base_1_Apply_1_Delegate_Local(int b) => method(a, b);
		return Base_1_Apply_1_Delegate_Local;
	}
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);

		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Length, Is.EqualTo(0));
			Assert.That(information.Results.Length, Is.EqualTo(0));
		});
	}

	[Test]
	public static void CreateWhenApplyMethodDoesNotExist()
	{
		var code =
 @"public static class Maths
{
	public static int Add(int a, int b) => a + b;
}

public static class Runner
{
	public static void Run() => Partially.Apply(Maths.Add, 3);
}";
		var information = PartiallyAppliedInformationTests.GetInformation(code);

		Assert.Multiple(() =>
		{
			Assert.That(information.Diagnostics.Length, Is.EqualTo(0));
			Assert.That(information.Results.Length, Is.GreaterThan(0));
			var result = information.Results[0];
			Assert.That(result.Target.Name, Is.EqualTo("Add"));
			Assert.That(result.PartialArgumentCount, Is.EqualTo(1));
		});
	}

	private static PartiallyAppliedInformation GetInformation(string source)
	{
		var syntaxTree = CSharpSyntaxTree.ParseText(source);
		var references = AppDomain.CurrentDomain.GetAssemblies()
			.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
			.Select(_ => MetadataReference.CreateFromFile(_.Location))
			.Concat(new[] { MetadataReference.CreateFromFile(typeof(PartiallyAppliedGenerator).Assembly.Location) });
		var compilation = CSharpCompilation.Create("apply", new SyntaxTree[] { syntaxTree },
			references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

		var invocationSyntax = syntaxTree.GetRoot().DescendantNodes(_ => true)
			.OfType<InvocationExpressionSyntax>().Single(_ =>
				_.Expression is MemberAccessExpressionSyntax access &&
					access.Expression is IdentifierNameSyntax accessIdentifier &&
					accessIdentifier.Identifier.Text == Naming.PartiallyClassName &&
					access.Name is IdentifierNameSyntax accessName &&
					accessName.Identifier.Text == Naming.ApplyMethodName);
		return new PartiallyAppliedInformation(invocationSyntax, compilation);
	}
}