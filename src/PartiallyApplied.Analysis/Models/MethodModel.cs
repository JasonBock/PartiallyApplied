using Microsoft.CodeAnalysis;
using PartiallyApplied.Extensions;

namespace PartiallyApplied.Analysis.Models;

internal sealed record MethodModel
	: IComparable<MethodModel>
{
	internal MethodModel(IMethodSymbol method, TypeReferenceModelContext typeReferenceModelContext)
	{
		this.IsStandard = method.IsStandard();
		this.Parameters = [.. method.Parameters.Select(_ => new ParameterModel(_, typeReferenceModelContext))];
		this.ReturnsVoid = method.ReturnsVoid;
		this.ReturnType = typeReferenceModelContext.CreateTypeReference(method.ReturnType);
		this.ReturnsByRef = method.ReturnsByRef;
		this.ReturnsByRefReadonly = method.ReturnsByRefReadonly;
		this.TypeParameters = [.. method.TypeParameters.Select(_ => typeReferenceModelContext.CreateTypeReference(_))];
	}

	internal bool IsStandard { get; }
	internal EquatableArray<ParameterModel> Parameters { get; }
	internal bool ReturnsByRef { get; }
	internal bool ReturnsByRefReadonly { get; }
	internal bool ReturnsVoid { get; }
	internal ITypeReferenceModel ReturnType { get; }
	internal EquatableArray<ITypeReferenceModel> TypeParameters { get; }

	public int CompareTo(MethodModel other)
	{
		var compareTo = this.IsStandard.CompareTo(other.IsStandard);

		if (compareTo != 0)
		{
			return compareTo;
		}
		else
		{
			compareTo = this.ReturnsByRef.CompareTo(other.ReturnsByRef);

			if (compareTo != 0)
			{
				return compareTo;
			}
			else
			{
				compareTo = this.ReturnsByRefReadonly.CompareTo(other.ReturnsByRefReadonly);

				if (compareTo != 0)
				{
					return compareTo;
				}
				else
				{
					compareTo = this.ReturnsVoid.CompareTo(other.ReturnsVoid);

					if (compareTo != 0)
					{
						return compareTo;
					}
					else
					{
						compareTo = this.Parameters.Length.CompareTo(other.Parameters.Length);

						if (compareTo != 0)
						{
							return compareTo;
						}
						else
						{
							for (var i = 0; i < this.Parameters.Length; i++)
							{
								compareTo = this.Parameters[i].CompareTo(other.Parameters[i]);

								if (compareTo != 0)
								{
									return compareTo;
								}
							}

							compareTo = this.ReturnType.CompareTo(other.ReturnType);

							if (compareTo != 0)
							{
								return compareTo;
							}
							else
							{
								compareTo = this.TypeParameters.Length.CompareTo(other.TypeParameters.Length);

								if (compareTo != 0)
								{
									return compareTo;
								}
								else
								{
									for (var i = 0; i < this.TypeParameters.Length; i++)
									{
										compareTo = this.TypeParameters[i].CompareTo(other.TypeParameters[i]);

										if (compareTo != 0)
										{
											return compareTo;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		return 0;
	}
}