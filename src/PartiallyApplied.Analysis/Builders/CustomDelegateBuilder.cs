using Microsoft.CodeAnalysis;
using PartiallyApplied.Analysis.Models;
using PartiallyApplied.Extensions;
using System.CodeDom.Compiler;

namespace PartiallyApplied.Builders;

internal static class CustomDelegateBuilder
{
	internal static void Build(InvocationModel result, IndentedTextWriter writer)
	{
		var target = result.Target;

		var applyParameters = string.Join(", ", target.Parameters.Take(result.PartialArgumentCount)
			.Select(parameter => $"{parameter.Type.FullyQualifiedName} {parameter.Name}"));
		var applyOpenGenerics = target.TypeParameters.Length > 0 ?
			$"<{string.Join(", ", target.TypeParameters.Select(_ => _.Name))}>" : string.Empty;
		var applyName = $"{result.ApplyName}{applyOpenGenerics}";

		var targetDelegateParameters = string.Join(", ",
			target.Parameters.Select(
				parameter => parameter.HasExplicitDefaultValue ? 
					$"{parameter.Type.FullyQualifiedName} {parameter.Name} = {parameter.ExplicitDefaultValue.GetDefaultValue()}" : 
					$"{parameter.Type.FullyQualifiedName} {parameter.Name}"));
		// NOTE: The number of open generics for the target delegate is the same as the apply method.
		var targetDelegateName = $"Target_{target.Hash}_Delegate{applyOpenGenerics}";

		var applyDelegateParameters = string.Join(", ", target.Parameters.Skip(result.PartialArgumentCount)
			.Select(_ => _.HasExplicitDefaultValue ? 
				$"{_.Type.FullyQualifiedName} {_.Name} = {_.ExplicitDefaultValue.GetDefaultValue()}" : 
				$"{_.Type.FullyQualifiedName} {_.Name}"));
		var applyDelegateGenerics = target.Parameters.Skip(result.PartialArgumentCount)
			.Where(_ => _.Type.SymbolKind == SymbolKind.TypeParameter).ToArray();
		var applyDelegateOpenGenerics = applyDelegateGenerics.Length > 0 ?
			$"<{string.Join(", ", applyDelegateGenerics.Select(_ => _.Type.FullyQualifiedName))}>" : string.Empty;
		var applyDelegateName = $"Apply_{target.Hash}_Delegate{applyDelegateOpenGenerics}";

		var refReturn = 
			target.ReturnsByRef ? 
				"ref " :
				target.ReturnsByRefReadonly ? 
					"ref readonly " : 
					string.Empty;
		var methodRefReturn = target.ReturnsByRef || target.ReturnsByRefReadonly ? "ref " : string.Empty;

		writer.WriteLine($"public delegate {refReturn}{target.ReturnType.FullyQualifiedName} {targetDelegateName}({targetDelegateParameters});");
		writer.WriteLine($"public delegate {refReturn}{target.ReturnType.FullyQualifiedName} {applyDelegateName}({applyDelegateParameters});");

		writer.WriteLine($"public static {applyDelegateName} {applyName}({targetDelegateName} method, {applyParameters}) =>");
		writer.Indent++;
		var remainingParameters = string.Join(", ", target.Parameters.Skip(result.PartialArgumentCount).Select(_ => _.Name));
		var allParameters = string.Join(", ", target.Parameters.Select(_ => _.Name));
		writer.WriteLine($"new(({remainingParameters}) => {methodRefReturn}method({allParameters}));");
		writer.Indent--;
	}
}