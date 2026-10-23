namespace PartiallyApplied.Analysis.Models;

internal interface ITypeReferenceModel
	: IEquatable<ITypeReferenceModel>
{
	internal string FullyQualifiedName { get; }
	internal string Name { get; }
	internal string? Namespace { get; }
}