﻿using System;

var incrementBy3 = Partially.Apply(Maths.Add, 3);
Console.Out.WriteLine(incrementBy3(7));

var tripler = Partially.Apply(Maths.Multiply, 3);
Console.Out.WriteLine(tripler(4));

var incrementBy7 = Partially.Apply(Maths.Sum, 3, 4);
Console.Out.WriteLine(incrementBy7(5, 6));

public static class Maths
{
	public static int Add(int a, int b) => a + b;
	public static int Multiply(int a, int b) => a * b;
	public static int Sum(int a, int b, int c, int d) => a + b + c + d;
}