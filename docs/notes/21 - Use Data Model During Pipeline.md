So, `AppliedInvocationModel` does the validation, and it will have an `EquatableArray<InvocationModel> Invocations { get; }` when it's done. That will be either 0 elements with diagnostics, or at least 1 if code generation is needed.

An `InvocationModel` has three things:
* `MethodModel Invocation` - this is the delegate that is the first argument to "Partially.Apply()"
* `uint PartialArgumentCount` - this is the number of partial arguments
* `string ApplyName` - this is the name of the "apply" method

TODO:

* DONE - `PartiallyAppliedGeneratorTests`
    * DONE - `GenerateWhenDuplicatesExistAsync` - should only generate 1, not 2
    * DONE - `GenerateWhenOverloadsExistAsync` - the order of the generated `Apply` methods keeps switching, need to make it consistent
        * DONE - Comparables
            * DONE - `MethodModel` 
            * DONE - `ParameterModel`
            * DONE - `TypeReferenceModel`
* DONE (deleted) - `NamespaceGatherer` - is this needed? Why?
* DONE (deleted) - `IMethodSymbolEqualityComparer` - may be able to delete
* `ExpressionSyntaxExtensions.TryGetMethodSymbols()` - do I actually have a case where there will be more than 1 candidate symbol? Update tests as well to verify count.
* `PartiallyAppliedInformationResult` - can probably be just a simple `record`
