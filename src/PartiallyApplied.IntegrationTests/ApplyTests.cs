using NUnit.Framework;

namespace PartiallyApplied.IntegrationTests
{
	public static class Maths
	{
		public static int Add(int a, int b) => a + b;
		public static int Multiply(int a, int b) => a * b;
		public static int Sum(int a, int b, int c, int d) => a + b + c + d;
	}

	public static class ApplyTests
	{
		[Test]
		public static void UseForAdd()
		{
			var incrementBy3 = Partially.Apply(Maths.Add, 3);
			Assert.That(incrementBy3(4), Is.EqualTo(7));
		}

		[Test]
		public static void UseForMultiply()
		{
			var tripler = Partially.Apply(Maths.Multiply, 3);
			Assert.That(tripler(4), Is.EqualTo(12));
		}

		[Test]
		public static void UseForSum()
		{
			var incrementBy7 = Partially.Apply(Maths.Sum, 3, 4);
			Assert.That(incrementBy7(5, 6), Is.EqualTo(18));
		}
	}
}