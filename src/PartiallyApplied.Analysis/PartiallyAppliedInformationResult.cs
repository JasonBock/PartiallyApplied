using Microsoft.CodeAnalysis;
using PartiallyApplied.Extensions;

namespace PartiallyApplied;

public sealed class PartiallyAppliedInformationResult
	 : IEquatable<PartiallyAppliedInformationResult?>
{
	public PartiallyAppliedInformationResult(IMethodSymbol target, int partialArgumentCount, string applyName) =>
		(this.Target, this.PartialArgumentCount, this.ApplyName) = (target, partialArgumentCount, applyName);

	public static bool operator ==(PartiallyAppliedInformationResult left, PartiallyAppliedInformationResult right) =>
		EqualityComparer<PartiallyAppliedInformationResult>.Default.Equals(left, right);

	public static bool operator !=(PartiallyAppliedInformationResult left, PartiallyAppliedInformationResult right) =>
		!(left == right);

	public override bool Equals(object? obj) =>
		this.Equals(obj as PartiallyAppliedInformationResult);

	public bool Equals(PartiallyAppliedInformationResult? other) =>
		other is not null &&
			this.PartialArgumentCount == other.PartialArgumentCount &&
			this.Target.AreEqual(other.Target) &&
			this.ApplyName == other.ApplyName;

	public override int GetHashCode() =>
		(this.GetTargetHashCode(), this.ApplyName, this.PartialArgumentCount).GetHashCode();

	private int GetTargetHashCode() =>
		$"{this.Target.ReturnType.Name}{this.Target.Parameters.Select(_ => _.Type.GetName())}".GetHashCode();

	public string ApplyName { get; }
	public int PartialArgumentCount { get; }
	public IMethodSymbol Target { get; }
}