using PartiallyApplied.NuGetHost;

var incrementBy3 = Partially.Apply(Maths.Add, 3);
Console.Out.WriteLine(incrementBy3(7));

var tripler = Partially.Apply(Maths.Multiply, 3);
Console.Out.WriteLine(tripler(4));

var incrementBy7 = Partially.Apply(Maths.Sum, 3, 4);
Console.Out.WriteLine(incrementBy7(5, 6));

var addWith3 = Partially.ApplyWithOptionals(Maths.AddOptionals, 3);
Console.Out.WriteLine(addWith3());
Console.Out.WriteLine(addWith3(10));
Console.Out.WriteLine(addWith3(10, 20));