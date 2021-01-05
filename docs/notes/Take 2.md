# Round 2 of PartiallyApplied

The main thing to look for is:
```
Partially.Apply(Maths.Multiply, 3)
```
The intent is that there isn't a `Partially.Apply` to start. The call must also have at least argument as well as we're going to partially apply. The source generator would then create all the code to do partial application, so the result would be this:
```
using System;

public static partial class Partially
{
	public delegate int MultiplyDelegate(int a, int b);
	public delegate int MultiplyApplyDelegate(int b);

	public static MultiplyApplyDelegate Apply(MultiplyDelegate method, int a)
	{
		int MultiplyApply(int b) => Maths.Multiply(a, b);
		return MultiplyApply;
	}
}

public static class Maths
{
	public static int Multiply(int a, int b) => a * b;

	public static void TripleValue(int b)
	{
		var tripler = Partially.Apply(Maths.Multiply, 3);
		Console.Out.WriteLine(tripler(b));
	}
}
```
Doing this in an app:
```
Maths.TripleValue(5);
```
Would print "15" to the console.

The problem seems to be when the first `Apply()` is generated. Then the compiler starts getting confused.
```
model.GetSymbolInfo(delegateArgument.Expression).Symbol
null

model.GetSymbolInfo(delegateArgument.Expression).CandidateSymbols[0]
{Maths.Multiply(int, int)}
```
In other words, if getting symbol info doesn't return anything, use `CandidateSymbols`. If there's at least one, use that.

**Remember**:
```
public static class Maths
{
	public static int Add(int a, int b) => a + b;
	public static int Multiply(int a, int b) => a * b;
	
	public static void RunApply()
	{
		var increaseByThree = Partially.Apply(Maths.Add, 3);
	}
}
```
This should only create **one** delegate for the intake. If the `GetMethodSymbol()` call I did in the experiment finds it "normally", then that means the binding is already in place, and we don't need to make another delegate, so guess what? It just ends up "working" (I think).

So, here's the thing. I need to create an "BaseDelegate" for every unique function I see. Then a "BaseApplyDelegate" for every unique partial application for a given unique function. That's it. It doesn't matter what class/object it comes from or what the name is. So this would be fine:
```
using System;

public static partial class Partially
{
	public delegate int Base374839Delegate(int a, int b);
	public delegate int Base374839_5739190ApplyDelegate(int b);

	public static Base374839_5739190ApplyDelegate Apply(Base374839Delegate method, int a)
	{
		int Base374839_5739190Apply(int b) => method(a, b);
		return Base374839_5739190Apply;
	}
}

public static class Maths
{
	public static int Add(int a, int b) => a + b;
	public static int Multiply(int a, int b) => a * b;

	public static void DoValues(int b)
	{
		var tripler = Partially.Apply(Maths.Multiply, 3);
		var incrementBy3 = Partially.Apply(Maths.Add, 3);
		Console.Out.WriteLine(tripler(b));
		Console.Out.WriteLine(incrementBy3(b));
	}
}
```
This would work just fine.

Wow.

Now all I need is a deck of cards.

Wait. What about the case where you stated one of the arguments, but now you want to state two, and you already stated the first one? You already have the delegate definition correct, so you don't want to create a new one. Hmmmmm....
```
public static partial class Partially
{
	public delegate int SumDelegate(int a, int b, int c);
	public delegate int Sum_a_ApplyDelegate(int c);

	public static Sum_a_ApplyDelegate Apply(SumDelegate method, int a)
	{
		int Sum_a_Apply(int b, int c) => method(a, b, c);
		return Sum_a_Apply;
	}
}

public static class Maths
{
	public static int Sum(int a, int b, int c) => a + b + c;

	public static void DoValues(int b)
	{
		//var incrementBy3 = Partially.Apply(Maths.Add, 3);
		var incrementBy3And4 = Partially.Apply(Maths.Sum, 3, 4);
	}
}
```
I might have to keep a static list of all generated delegates for the first argument and the return argument (yuck!). Not ideal, but I don't know how else to prevent identical delegates being created.

If I use hash code, I may be able to keep a hash set of "int" for the argument delegate, and "(int, int)" for the return delegate, and not store the whole string.

Tasks:
* Need a way to find out how to find what functions are "equal" (probably use the MethodMatch thing in Rocks)
* If an Apply() function already exists, don't do anything
* Additional file to allow the developer to state what to look for. "Partially.Apply" is the default, but they could set "Quux.Flux".

For **most** cases, using a `Func<>` or `Action<>` would satify the need. So, I need a way to determine if the delegate can be done with a `Func<>` or `Action<>`. This means:
* 16 arguments or less
* Nothing "ref" or "out" or "ref readonly"
* No pointers or ref structs

If that's the case, then just generate something like `Func<int, string, Guid>`. We don't need to generate the target and apply delegates, we just use them in `Apply()` and the internal function. In fact, in that case, I don't think I even need a local function.
```
public static partial class Partially
{
	public static Func<int, int> Apply(Func<int, int, int, int> method, int a, int b) =>
		new Func<int, int>(c => method(a, b, c));
}

public static class Maths
{
	public static int Sum(int a, int b, int c) => a + b + c;

	public static void DoValues(int b)
	{
		var incrementBy3And4 = Partially.Apply(Maths.Sum, 3, 4);
	}
}
```
That's **much** cleaner!

Post-MVP:
* DONE - Need to support projecting delegates when methods are not "standard" (i.e. pointers and ref structs, or ref/out/in parameters and/or return types)
* DONE - Need to support open generics
* DONE - Need to include default values for functions