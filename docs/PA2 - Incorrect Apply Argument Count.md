# Incorrect Apply Argument Count
The `Apply` method needs at least two parameters to do partial function application.
```
public static class Targets
{
  public static int Add(int a, int b) => a + b;
}

public static class Runner
{
  // This will generate PA2
  public static void Run() => Partially.Apply(Targets.Add);
}
```