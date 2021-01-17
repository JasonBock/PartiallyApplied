# Too Many Arguments
If `Apply()` is given too many arguments, this diagnostic will be created.
```
public static class Maths
{
  public static int Add(int a, int b) => a + b;
}

public static class Runner
{
  // This will generate PA5
  public static void Run() => Partially.Apply(Maths.Add, 3, 4);
}
```