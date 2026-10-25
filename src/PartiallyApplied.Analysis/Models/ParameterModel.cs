using Microsoft.CodeAnalysis;

namespace PartiallyApplied.Analysis.Models;

internal sealed record ParameterModel
	: IComparable<ParameterModel>
{
	internal ParameterModel(IParameterSymbol parameterSymbol, TypeReferenceModelContext typeReferenceModelContext)
	{
		this.Type = typeReferenceModelContext.CreateTypeReference(parameterSymbol.Type);
		this.Name = parameterSymbol.Name;
		this.HasExplicitDefaultValue = parameterSymbol.HasExplicitDefaultValue;

		if (parameterSymbol.HasExplicitDefaultValue)
		{
			this.ExplicitDefaultValue = parameterSymbol.ExplicitDefaultValue;
		}
	}

	public int CompareTo(ParameterModel other) =>
		this.Name.CompareTo(other.Name);

	internal object? ExplicitDefaultValue { get; }
	internal bool HasExplicitDefaultValue { get; }
	internal string Name { get; }
	internal ITypeReferenceModel Type { get; }
}