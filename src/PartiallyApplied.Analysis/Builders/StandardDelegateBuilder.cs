using Microsoft.CodeAnalysis;
using PartiallyApplied.Analysis.Models;
using System.CodeDom.Compiler;

namespace PartiallyApplied.Builders;

internal static class StandardDelegateBuilder
{
	internal static void Build(InvocationModel result, IndentedTextWriter writer)
	{
		var target = result.Target;

		var delegateType = target.ReturnsVoid ? "Action" : "Func";

		var targetDelegateParameters = new List<ITypeReferenceModel>(
			target.Parameters.Select(parameter => parameter.Type));

		if (!target.ReturnsVoid)
		{
			targetDelegateParameters.Add(target.ReturnType);
		}

		var targetDelegate = $"{delegateType}<{string.Join(", ", targetDelegateParameters.Select(_ => _.FullyQualifiedName))}>";
		var applyDelegate = $"{delegateType}<{string.Join(", ", targetDelegateParameters.Skip(result.PartialArgumentCount).Select(_ => _.FullyQualifiedName))}>";
		var applyParameters = string.Join(", ", target.Parameters.Take(result.PartialArgumentCount).Select(_ => $"{_.Type.FullyQualifiedName} {_.Name}"));
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