# No Target Method Found
If the target method doesn't exist, this diagnostic will be created.
```
public static class Maths
{
  public static int Add(int a, int b) => a + b;
}

public static class Runner
{
  // This will generate PA3
  public static void Run() => Partially.Apply(Maths.Subtract, 3);
}
```