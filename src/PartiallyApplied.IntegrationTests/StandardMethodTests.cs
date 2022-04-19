using NUnit.Framework;

namespace PartiallyApplied.IntegrationTests;

public static class Maths
{
	public static int Add(int a, int b) => a + b;
	public static int Multiply(int a, int b) => a * b;
	public static int Sum(int a, int b, int c, int d) => a + b + c + d;
	public static string CombineBegin<T>(T a, double b, int c) =>
		$"{a}, {b}, {c}";
	public static string CombineEnd<T>(int a, int b, T c) =>
		$"{a}, {b}, {c}";
}

public static class MethodOverloads
{
	public static string Combine(string a, int b, int c, string d) =>
		$"{a}, {b}, {c}, {d}";
	public static string Combine(int a, string b, string c) =>
		$"{a}, {b}, {c}";
}

public static class StandardMethodTests
{
	[Test]
	public static void ApplyForOverloadStartingWithInt()
	{
		var overloadCombineInt = Partially.Apply(MethodOverloads.Combine, 3);
		Assert.That(overloadCombineInt("b", "c"), Is.EqualTo("3, b, c"));
	}

	[Test]
	public static void ApplyForOverloadStartingWithString()
	{
		var overloadCombineString = Partially.Apply(MethodOverloads.Combine, "3");
		Assert.That(overloadCombineString(2, 3, "4"), Is.EqualTo("3, 2, 3, 4"));
	}

	[Test]
	public static void ApplyForGenericEnd()
	{
		var combine3AtEndWithString = Partially.Apply<string>(Maths.CombineEnd, 3);
		Assert.That(combine3AtEndWithString(2, "c"), Is.EqualTo("3, 2, c"));
	}

	[Test]
	public static void ApplyForGenericBegin()
	{
		var combine3AtBeginWithInt = Partially.Apply(Maths.CombineBegin, 3);
		Assert.That(combine3AtBeginWithInt(4, 5), Is.EqualTo("3, 4, 5"));
	}

	[Test]
	public static void ApplyForAdd()
	{
		var incrementBy3 = Partially.Apply(Maths.Add, 3);
		Assert.That(incrementBy3(4), Is.EqualTo(7));
	}

	[Test]
	public static void ApplyForMultiply()
	{
		var tripler = Partially.Apply(Maths.Multiply, 3);
		Assert.That(tripler(4), Is.EqualTo(12));
	}

	[Test]
	public static void ApplyForSum()
	{
		var incrementBy7 = Partially.Apply(Maths.Sum, 3, 4);
		Assert.That(incrementBy7(5, 6), Is.EqualTo(18));
	}
}