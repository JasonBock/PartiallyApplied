using Microsoft.CodeAnalysis;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Analysis.Models;

internal sealed class TypeReferenceModelContext
{
	private readonly Dictionary<ITypeSymbol, TypeReferenceModel> typeMap =
		new(SymbolEqualityComparer.IncludeNullability);

	internal TypeReferenceModelContext(SemanticModel semanticModel) =>
		this.SemanticModel = semanticModel;

	internal ITypeReferenceModel CreateTypeReference(ITypeSymbol typeSymbol)
	{
		if (this.typeMap.TryGetValue(typeSymbol, out var model))
		{
			return model;
		}
		else
		{
			var newModel = new TypeReferenceModel(typeSymbol, this);
			this.typeMap.Add(typeSymbol, newModel);
			return newModel;
		}
	}

	internal SemanticModel SemanticModel { get; }

	private sealed record TypeReferenceModel
		: ITypeReferenceModel
	{
		public TypeReferenceModel(ITypeSymbol type, TypeReferenceModelContext modelContext)
		{
			this.Namespace =
				type.ContainingNamespace is not null ?
					!type.ContainingNamespace.IsGlobalNamespace ?
						type.ContainingNamespace.ToDisplayString() :
						null :
					null;
			this.Name = type.Name;
			this.FullyQualifiedName = type.GetFullyQualifiedName(modelContext.SemanticModel.Compilation);
			this.SymbolKind = type.Kind;
		}

		public override string ToString() => this.FullyQualifiedName;

		public bool Equals(ITypeReferenceModel other) =>
			this.Equals(other as TypeReferenceModel);

		public bool Equals(TypeReferenceModel? other) =>
			this.FullyQualifiedName == other?.FullyQualifiedName;

		public override int GetHashCode() => this.FullyQualifiedName.GetHashCode();

		public string FullyQualifiedName { get; }
		public string Name { get; }
		public string? Namespace { get; }
		public SymbolKind SymbolKind { get; }
	}
}