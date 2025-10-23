namespace eMechanic.Architecture.Tests;

using System;
using System.Collections.Generic;
using System.Reflection;
public abstract class ArchitectureTestBase
{
    protected static readonly string API_NAMESPACE = typeof(API.AssemblyReference).Namespace!;
    protected static readonly string APPLICATION_NAMESPACE = typeof(Application.AssemblyReference).Namespace!;
    protected static readonly string DOMAIN_NAMESPACE = typeof(Domain.AssemblyReference).Namespace!;
    protected static readonly string INFRASTRUCTURE_NAMESPACE = typeof(Infrastructure.AssemblyReference).Namespace!;
    protected static readonly string COMMON_NAMESPACE = typeof(Common.AssemblyReference).Namespace!;
    protected static readonly string FEATURES_NAMESPACE = typeof(API.Features.IFeature).Namespace!;

    protected static readonly Assembly[] SolutionAssemblies = LoadSolutionAssemblies();

    private static Assembly[] LoadSolutionAssemblies()
    {
        var assemblies = new List<Assembly>
        {
            typeof(API.AssemblyReference).Assembly,
            typeof(Application.AssemblyReference).Assembly,
            typeof(Common.AssemblyReference).Assembly,
            typeof(ArchitectureTestBase).Assembly,
            typeof(Domain.AssemblyReference).Assembly,
            typeof(Infrastructure.AssemblyReference).Assembly,
        };

        return assemblies.Distinct().ToArray();
    }

    protected static Assembly GetAssembly(string namespacePrefix)
    {
        var assembly = SolutionAssemblies
            .FirstOrDefault(a => a.FullName != null && a.FullName.StartsWith(namespacePrefix, StringComparison.Ordinal));

        if (assembly is null)
        {
            var loadedAssemblies = string.Join("\n", SolutionAssemblies.Select(a => a.FullName));
            throw new InvalidOperationException(
                $"Assembly with namespace prefix '{namespacePrefix}' not found. Loaded assemblies:\n{loadedAssemblies}");
        }
        return assembly;
    }
}
