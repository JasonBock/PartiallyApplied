# Introduction

New to PartiallyApplied? In this page, we'll cover what PartiallyApplied can do so you can get up to speed on the API with little effort. We'll cover what partially applied functions are first. Then, we'll discuss how PartiallyApplied works. Special cases will also be addressed.

Remember that this is just a quickstart. You can always browse the tests in source to see specific examples of a case that may not be covered in detail here.

## What is Partial Function Application?

Before we start using PartiallyApplied, let's cover what "partial function application", or "PAF", is. A fairly formal definition [exists](https://en.wikipedia.org/wiki/Partial_application), and we encourage you to read this, but at the end of the day, PAF generates a new method with some of the parameters bound to specific values. For example, let's say you had this simple mathematical function:
```
f(x, y) -> (3 * (x ^ 2)) + (4 * y) + 3
```
If we always wanted `x` to be 3, we could do this:
```
g(y) -> f(3, y) -> (3 * (3 ^ 2)) + (4 * y) + 3
```
Essentially, we now have a function, `g(y)`, that does what `f(x, y)` would do when `x` will **always** be equal to 3.

## Using PartiallyApplied

PartiallyApplied works using the [source generator feature](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) in C# 9. The generator looks for calls to a method that starts with the word "Apply", from a class called "Partially". In actuality, these members don't exist when you start to use PartiallyApplied. Once you start creating PAFs, the generator will build a class called `Partially` with the appropriate `Apply()` methods.

In the next section, let's go through an example to illustrate how this works.

### Targeting Methods

Let's start by defining a simple `Add()` method:
```
public static class Maths
{
  public static int Add(int a, int b) => a + b;
}
```
If we want to create a PAF such that `a` is set to 3, we'd write this:
```
var incrementBy3 = Partially.Apply(Maths.Add, 3);
```
Note that the first time you do this, there is no `Partially` class with an `Apply` method. PartiallyApplied is wired up to look for cases where you write code to do this kind of invocation. It then looks at the arguments passed to `Apply()`, and determines how to create the code to create the PAF. For the vast majority of cases, this can be done with either `Action` or `Func` delegates. For example, once the code generation completes, you'll be able to "go to definition" with your favorite IDE on `Apply()`, and you should see something like this:
```
using System;

#nullable enable
public static partial class Partially
{
	public static Func<int, int> Apply(Func<int, int, int> method, int a) =>
		new Func<int, int>((b) => method(a, b));
}
```
PartiallyApplied doesn't care **where** the target method is defined. It will generate an `Apply()` method for each distinct method signature that you are targeting. Therefore, if you defined a method like this:
```
public static class Maths
{
  public static void Multiply(int x, int y) => x * y;
}
```
And you would use PartiallyApplied to create a PAF:
```
var triple = Partially.Apply(Maths.Multiply, 3);
```
PartiallyApplied would use the same `Apply()` method that was generated for `Maths.Add`.

### Special Cases

There are a couple of cases where PartiallyApplied will create custom delegates to support the targeted method. These cases are:
* If the method has over 16 parameters
* If the method has a `ref` or `ret return` return values
* If the method has a `ref struct` type for a parameter that is **not** used in partial application.
* If the method has any default values for a parameter
For example, if you use PartiallyApplied to create a PAF for 

TODO: Finish

# Conclusion

You've now seen how PartiallyApplied works. Remember to peruse the tests within `PartiallyApplied.IntegrationTests` in case you get stuck. If you'd like, feel free to submit a PR to update this document to improve its contents. If you run into any issues, please submit an issue. Happy coding!