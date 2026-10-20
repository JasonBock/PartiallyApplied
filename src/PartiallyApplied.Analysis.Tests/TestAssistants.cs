using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NuGet.Frameworks;

namespace PartiallyApplied.Tests;

internal static class TestAssistants
{
	internal static async Task RunGeneratorAsync<TGenerator>(string code,
		IEnumerable<(string, string)> generatedSources,
		IEnumerable<DiagnosticResult> expectedDiagnostics,
		OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary,
		IEnumerable<MetadataReference>? additionalReferences = null,
		ReportDiagnostic generalDiagnosticOption = ReportDiagnostic.Default,
		List<string>? disabledDiagnostics = null)
		where TGenerator : IIncrementalGenerator, new()
	{
		var test = new IncrementalGeneratorTest<TGenerator>(generalDiagnosticOption)
		{
			ReferenceAssemblies = net10ReferenceAssemblies.Value,
			TestState =
			{
				Sources = { code },
				OutputKind = outputKind,
			},
		};

		if (disabledDiagnostics is not null)
		{
			test.DisabledDiagnostics.AddRange(disabledDiagnostics);
		}

		foreach (var (generatedFileName, generatedCode) in generatedSources)
		{
			test.TestState.GeneratedSources.Add((typeof(PartiallyAppliedGenerator), generatedFileName, generatedCode));
		}

		test.TestState.AdditionalReferences.Add(typeof(TGenerator).Assembly);

		if (additionalReferences is not null)
		{
			test.TestState.AdditionalReferences.AddRange(additionalReferences);
		}

		test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
		await test.RunAsync();
	}

	private static readonly Lazy<ReferenceAssemblies> net10ReferenceAssemblies = new(() =>
	{
		// Always look here for the latest version of a particular runtime:
		// https://www.nuget.org/packages/Microsoft.NETCore.App.Ref
		if (!NuGetFramework.Parse("net10.0").IsPackageBased)
		{
			// The NuGet version provided at runtime does not recognize the 'net10.0' target framework
			throw new NotSupportedException("The 'net10.0' target framework is not supported by this version of NuGet.");
		}

		return new ReferenceAssemblies(
			 "net10.0",
			 new PackageIdentity(
				  "Microsoft.NETCore.App.Ref",
				  "10.0.0"),
			 Path.Combine("ref", "net10.0"));
	}, LazyThreadSafetyMode.ExecutionAndPublication);
}