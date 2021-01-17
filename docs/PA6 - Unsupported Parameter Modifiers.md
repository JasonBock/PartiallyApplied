# Unsupported Parameter Modifiers
The target method cannot have parameters with `ref`, `out`, or `in` modifiers.
```
public static class Maths
{
  public static int AddByRef(int a, ref int b) => a + b;
}

public static class Runner
{
  // This will generate PA6
  public static void Run() => Partially.Apply(Maths.AddByRef, 3);
}
```