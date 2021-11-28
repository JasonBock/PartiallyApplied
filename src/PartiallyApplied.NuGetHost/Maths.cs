namespace PartiallyApplied.NuGetHost;

public static class Maths
{
	public static int Add(int a, int b) => a + b;
	public static int AddOptionals(int a = 3, int b = 4, int c = 5) => a + b + c;
	public static int Multiply(int a, int b) => a * b;
	public static int Sum(int a, int b, int c, int d) => a + b + c + d;
}