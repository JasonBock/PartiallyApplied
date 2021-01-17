# PartiallyApplied

A way to do partial function application in C#.

## Overview

You can find this code as a package in [NuGet](https://www.nuget.org/packages/PartiallyApplied/). Once installed, you can use it to do partial function application:
```
public static class Maths
{
  public static int Add(int a, int b) => a + b;
}

public static class Runner
{
  public static void Run()
  {
    var incrementBy3 = Partially.Apply(Maths.Add, 3);
    var value = incrementBy3(4);
    // value is now equal to 7.
  }
}
```
More details can be found on the [Quickstart page](https://github.com/JasonBock/PartiallyApplied/blob/master/docs/Quickstart.md).