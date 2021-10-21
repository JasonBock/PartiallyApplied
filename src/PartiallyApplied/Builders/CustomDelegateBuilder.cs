using Microsoft.CodeAnalysis;
using PartiallyApplied.Extensions;
using System.CodeDom.Compiler;
using System.Linq;
using System.Threading;

namespace PartiallyApplied.Builders
{
	internal static class CustomDelegateBuilder
	{
		internal static int id;

		internal static void Build(PartiallyAppliedInformationResult result,
			IndentedTextWriter writer, NamespaceGatherer gatherer)
		{
			var target = result.Target;
			gatherer.AddRange(target.Parameters.Select(_ => _.Type.ContainingNamespace));

			if (!target.ReturnsVoid)
			{
				gatherer.Add(target.ReturnType.ContainingNamespace);
			}

			var id = Interlocked.Increment(ref CustomDelegateBuilder.id);

			var applyParameters = string.Join(", ", target.Parameters.Take(result.PartialArgumentCount)
				.Select(_ => $"{_.Type.GetName()} {_.Name}"));
			var applyOpenGenerics = target.TypeParameters.Length > 0 ?
				$"<{string.Join(", ", target.TypeParameters.Select(_ => _.Name))}>" : string.Empty;
			var applyName = $"{result.ApplyName}{applyOpenGenerics}";

			var targetDelegateParameters = string.Join(", ", 
				target.Parameters.Select(_ => _.HasExplicitDefaultValue ? $"{_.Type.GetName()} {_.Name} = {_.ExplicitDefaultValue.GetDefaultValue()}" : $"{_.Type.GetName()} {_.Name}"));
			// NOTE: The number of open generics for the target delegate is the same as the apply method.
			var targetDelegateName = $"Target_{id}_Delegate{applyOpenGenerics}";

			var applyDelegateParameters = string.Join(", ", target.Parameters.Skip(result.PartialArgumentCount)
				.Select(_ => _.HasExplicitDefaultValue ? $"{_.Type.GetName()} {_.Name} = {_.ExplicitDefaultValue.GetDefaultValue()}" : $"{_.Type.GetName()} {_.Name}"));
			var applyDelegateGenerics = target.Parameters.Skip(result.PartialArgumentCount)
				.Where(_ => _.Type.Kind == SymbolKind.TypeParameter).ToArray();
			var applyDelegateOpenGenerics = applyDelegateGenerics.Length > 0 ?
				$"<{string.Join(", ", applyDelegateGenerics.Select(_ => _.Type.GetName()))}>" : string.Empty;
			var applyDelegateName = $"Apply_{id}_Delegate{applyDelegateOpenGenerics}";

			var refReturn = target.ReturnsByRef ? "ref " :
				target.ReturnsByRefReadonly ? "ref readonly " : string.Empty;
			var methodRefReturn = target.ReturnsByRef || target.ReturnsByRefReadonly ? "ref " : string.Empty;

			writer.WriteLine($"public delegate {refReturn}{target.ReturnType.GetName()} {targetDelegateName}({targetDelegateParameters});");
			writer.WriteLine($"public delegate {refReturn}{target.ReturnType.GetName()} {applyDelegateName}({applyDelegateParameters});");

			writer.WriteLine($"public static {applyDelegateName} {applyName}({targetDelegateName} method, {applyParameters}) =>");
			writer.Indent++;
			var remainingParameters = string.Join(", ", target.Parameters.Skip(result.PartialArgumentCount).Select(_ => _.Name));
			var allParameters = string.Join(", ", target.Parameters.Select(_ => _.Name));
			writer.WriteLine($"new(({remainingParameters}) => {methodRefReturn}method({allParameters}));");
			writer.Indent--;
		}
	}
}