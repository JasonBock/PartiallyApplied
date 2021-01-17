# Cannot Partially Apply Ref Struct
The target method cannot have `ref struct` types as parameters (e.g. `Span<int>`) as targets for partial function application.
```
using System;

public static class Maths
{
  public static int Contains(Span<int> a, int b) => a.Contains(b);
}

public static class Runner
{
  public static void Run()
  {
    var buffer = new Span<int>(new [] { 3 });
    // This will generate PA7
    Partially.Apply(Maths.Contains, buffer);
  }
}
```