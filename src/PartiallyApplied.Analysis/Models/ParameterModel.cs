using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Analysis.Models;

internal sealed record ParameterModel
{
	internal ParameterModel(IParameterSymbol parameterSymbol, TypeReferenceModelContext typeReferenceModelContext)
	{
		this.Type = typeReferenceModelContext.CreateTypeReference(parameterSymbol.Type);
		this.Name = parameterSymbol.Name;
	}

	internal string Name { get; }
	internal ITypeReferenceModel Type { get; }
}