using Microsoft.CodeAnalysis;
using PartiallyApplied.Extensions;
using System.CodeDom.Compiler;

namespace PartiallyApplied.Builders;

internal static class StandardDelegateBuilder
{
	internal static void Build(PartiallyAppliedInformationResult result,
		IndentedTextWriter writer, NamespaceGatherer gatherer)
	{
		var target = result.Target;
		gatherer.Add(typeof(Action));

		var delegateType = target.ReturnsVoid ? "Action" : "Func";

		var targetDelegateParameters = new List<ITypeSymbol>(
			target.Parameters.Select(_ =>
			{
				gatherer.Add(_.Type.ContainingNamespace);
				return _.Type;
			}));

		if (!target.ReturnsVoid)
		{
			gatherer.Add(target.ReturnType.ContainingNamespace);
			targetDelegateParameters.Add(target.ReturnType);
		}

		var targetDelegate = $"{delegateType}<{string.Join(", ", targetDelegateParameters.Select(_ => _.GetName()))}>";
		var applyDelegate = $"{delegateType}<{string.Join(", ", targetDelegateParameters.Skip(result.PartialArgumentCount).Select(_ => _.GetName()))}>";
		var applyParameters = string.Join(", ", target.Parameters.Take(result.PartialArgumentCount).Select(_ => $"{_.Type.GetName()} {_.Name}"));
		var applyOpenGenerics = target.TypeParameters.Length > 0 ?
			$"<{string.Join(", ", target.TypeParameters.Select(_ => _.Name))}>" : string.Empty;
		var applyName = $"{result.ApplyName}{applyOpenGenerics}";

		writer.WriteLine($"public static {applyDelegate} {applyName}({targetDelegate} method, {applyParameters}) =>");
		writer.Indent++;
		var remainingParameters = string.Join(", ", target.Parameters.Skip(result.PartialArgumentCount).Select(_ => _.Name));
		var allParameters = string.Join(", ", target.Parameters.Select(_ => _.Name));
		writer.WriteLine($"new(({remainingParameters}) => method({allParameters}));");
		writer.Indent--;
	}
}