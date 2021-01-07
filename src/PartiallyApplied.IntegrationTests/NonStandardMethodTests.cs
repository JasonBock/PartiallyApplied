using NUnit.Framework;
using System;

namespace PartiallyApplied.IntegrationTests
{
	public static class NonStandardMethods
	{
		private static int refReadonly;
		private static int refRefurn;

		public static int Sum(int a, int b, int c, int d, int e, int f, int g, int h,
			int i, int j, int k, int l, int m, int n, int o, int p, int q) =>
			a + b + c + d + e + f + g + h + i + j + k + l + m + n + o + p + q;

		public static bool Contains(int value, Span<int> buffer) => buffer.Contains(value);

		public static ref int RefReturn(int a, int b)
		{
			NonStandardMethods.refRefurn = a + b + NonStandardMethods.refRefurn;
			return ref NonStandardMethods.refRefurn;
		}

		public static ref readonly int RefReadonlyReturn(int a, int b)
		{
			NonStandardMethods.refReadonly = a + b + NonStandardMethods.refReadonly;
			return ref NonStandardMethods.refReadonly;
		}
	}

	public static class NonStandardMethodTests
	{
		[Test]
		public static void ApplyWithLotsOfParameters()
		{
			var applyAboutHalf = Partially.Apply(NonStandardMethods.Sum, 1, 2, 3, 4, 5, 6, 7, 8);
			Assert.That(applyAboutHalf(9, 10, 11, 12, 13, 14, 15, 16, 17), Is.EqualTo(153));
		}

		[Test]
		public static void ApplyWithRefStructParameter()
		{
			var doesBufferContain3 = Partially.Apply(NonStandardMethods.Contains, 3);
			var buffer = new Span<int>(new[] { 3 });
			Assert.That(doesBufferContain3(buffer), Is.True);
		}

		[Test]
		public static void ApplyWithRefReturn()
		{
			var refReturn = Partially.ApplyWithRefReturn(NonStandardMethods.RefReturn, 3);
			ref var result = ref refReturn(4);
			result = 12;
			result = ref refReturn(4);
			Assert.That(result, Is.EqualTo(19));
		}

		[Test]
		public static void ApplyWithRefReadonlyReturn()
		{
			var refReadonlyReturn = Partially.ApplyWithRefReadonlyReturn(NonStandardMethods.RefReadonlyReturn, 3);
			ref readonly var result = ref refReadonlyReturn(4);
			result = ref refReadonlyReturn(4);
			Assert.That(result, Is.EqualTo(14));
		}
	}
}