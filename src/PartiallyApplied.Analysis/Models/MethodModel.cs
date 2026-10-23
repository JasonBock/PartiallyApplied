using Microsoft.CodeAnalysis;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Analysis.Models;

internal sealed record MethodModel
{
	internal MethodModel(IMethodSymbol method, TypeReferenceModelContext typeReferenceModelContext)
	{
		this.IsStandard = method.IsStandard();
		this.Parameters = [.. method.Parameters.Select(_ => new ParameterModel(_, typeReferenceModelContext))];
		this.ReturnsVoid = method.ReturnsVoid;
		this.ReturnType = typeReferenceModelContext.CreateTypeReference(method.ReturnType);
		this.TypeParameters = [.. method.TypeParameters.Select(_ => typeReferenceModelContext.CreateTypeReference(_))];
	}

	internal bool IsStandard { get; }
	internal EquatableArray<ParameterModel> Parameters { get; }
	internal bool ReturnsVoid { get; }
	internal ITypeReferenceModel ReturnType { get; }
	internal EquatableArray<ITypeReferenceModel> TypeParameters { get; }
}