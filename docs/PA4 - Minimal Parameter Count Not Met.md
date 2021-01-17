# Minimal Parameter Count Not Met
The target method needs at least two parameters to do partial function application.
```
public static class Targets
{
  public static void NotEnoughParameters(int a) { }
}

// This will generate PA4
var paf = Partially.Apply(Targets.NotEnoughParameters, 3);
```