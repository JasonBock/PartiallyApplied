# PartiallyApplied

Fancy definition: https://en.wikipedia.org/wiki/Partial_application

Other languages:
* Pony - https://tutorial.ponylang.io/expressions/partial-application.html
* Python - https://docs.python.org/3/library/functools.html?highlight=partial#functools.partial
* F# - https://fsharpforfunandprofit.com/posts/partial-application/

C# attempts
* https://dev.to/kelsonball/an-example-of-partial-function-application-in-c-2bed
* https://mikehadlow.blogspot.com/2015/09/partial-application-in-c.html
* NOT THIS: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/partial-method

Something like this:
```
public static class Maths
{
	public static int Multiply(int a, int b) => a * b;

	public static void Foo()
	{
		var tripler = Partially.Apply(Maths.Multiply, 3);
	}
}
```
We need to detect that an invocation to `Partially.Apply` has been called. I can have a method like this (maybe):
```
public static class Partially
{
	public static void Apply(Delegate target) { }
}
```
Once the generator kicks in, what I need to do is:
* The first argument must be a `IMethodSymbol`. If it isn't, diagnostic
* There has to be at least one argument to the target. If there aren't, diagnostic
* There has to be "rest" arguments of length up to, but not exceeding, the number of arguments to the target. If this isn't the case, diagnostic
* The rest arguments have to "fit" into the target. For example, if I had this:
```
public static void Foo(int a, string b, Guid c)...
```
Then I could do this:
```
Partially.Apply(Foo, 3, "b");
```
If they don't "fit", diagnostic

If all these "apply" (hahaha)...

* I need to generate this:
```
public partial static class Partially
{
	public delegate FooApplyDelegate(Guid c);
	private static void FooApply(Guid c) => Foo(3, "b", c);
	
	public static FooApplyDelegate Apply(Delegate target, int a, string b) = FooApply;
}
```
The `Apply()` method is purely to satisfy the compiler so the apparent call which is initially in error goes away.
The returned delgate is to match the signature of the partial application function
The actual implementation calls the original supplied method, passing in the given values and the parameter values.

This sucks because there will quickly be collisions. If there's a "Bar(int a, string b, Guid c)" in the same type, and "a" and "b" were given, I'd generate two Apply() methods with the exact same signature. What I could do is this:
```
namespace WhateverFooAndBarAreIn
{
	public partial static class Partially
	{
		public delegate void FooDelegate(int a, string b, Guid c);
		private static void FooApply(Guid c) => Foo(3, "b", c);
		public static FooApplyDelegate Apply(FooDelegate target, int a, string b) = FooApply;

		public delegate void BarDelegate(int a, string b, Guid c);
		private static void BarApply(Guid c) => Foo(3, "b", c);
		public static BarApplyDelegate Apply(BarDelegate target, int a, string b) = BarApply;	
	}
}
```
This is better, but...I could later call `Apply()` like this:
```
Partially.Apply(Foo, 4, "c");
```
Now I have to generate **another** applied delegate for that condition. Furthermore, I can't tell which "Apply" to call.

Rapid thought, I could generate a dictionary of tuples IF I have more than one condition for the same target method. Meaning, if I had this:
```
Partially.Apply(Foo, 3);
Partially.Apply(Bar, 3, "b");
Partially.Apply(Bar, 4);
```
I'm good, I can generate what I have above, because all the conditions are unique. **But**, if I do this:
```
Partially.Apply(Foo, 3);
Partially.Apply(Foo, 4);
```
They're not. I'm referencing a method, `Foo`, with the first value specified twice with different values.

Now I need to do this:
```
public delegate void FooDelegate(int a, string b, Guid c);
private static void FooHashCodeForA3Apply(string b, Guid c) => Foo(3, b, c);
private static void FooHashCodeForA4Apply(string b, Guid c) => Foo(3, b, c);
public static FooApplyDelegate Apply(FooDelegate target, int a) =>
	a == 3 ? FooHashCodeForA3Apply : FooHashCodeForA4Apply;
```
OK, scratch all that..."I had a revelation" Matrix style :)
```
public static partial class Partially
{
	public delegate int MultiplyDelegate(int a, int b);
	public delegate int MultiplyApplyDelegate(int b);

	public static MultiplyApplyDelegate Apply(this MultiplyDelegate self, int a)
	{
		int MultiplyApply(int b) => Maths.Multiply(a, b);
		return MultiplyApply;
	}
}
```
The applied function is a local method that closes over "a". So no matter what I pass in:
```
var doubler = Partially.Apply(Maths.Multiply, 2);
Console.Out.WriteLine(doubler(7));

var tripler = Partially.Apply(Maths.Multiply, 3);
Console.Out.WriteLine(tripler(7));
```
It works! I don't have to special case **anything**. The crux now falls on being able to figure out **at compile time** the "shape" of the delegate being passed in. If I can, I'm golden.
``
argument.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression
``
This should be a `IMethodSymbol: (model.GetSymbolInfo(argument.Expression).Symbol as IMethodSymbol)!`
```
argument.Expression.Kind() == SyntaxKind.IdentifierName
```
This is a bit harder. It's identifying...something. Either a variable declaration (or field or property declaration?) or a parameter.

So, start by looking for something declared within the body that this call exists in (or, if we're in a top-level statement, a global statement, which maybe we can get up to the compilation unit, look for the `MethodBodyOperation`, or ... not).

VariableDeclaration: Go up to the "root" (either a "method" body or the `CompilationUnitSyntax` if there's a parent that's a global statement) and find a variable declaration that 
```
Parameter
VariableDeclaration
```
But it might be easier than this:
* https://www.nuget.org/packages/Microsoft.CodeAnalysis.AnalyzerUtilities/
* https://github.com/dotnet/roslyn-analyzers/blob/master/src/Utilities/FlowAnalysis/FlowAnalysis/Analysis/PointsToAnalysis/PointsToAnalysis.cs

I may push that off until later.

Also, I've noticed that having `Partially.Apply(...)` confused the compiler. Maybe the "default" is this?
```
Apply(Delegate target, params object[] values).
```
Now if I generate something like this:
```
Apply(MultiplyDelegate method, int a)
```
I think C# will pick the more specific one.

Dynamic types?? Partial types/methods??