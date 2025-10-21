namespace eMechanic.Architecture.Tests;

using System;
using System.Collections.Generic;
using System.Reflection;

public abstract class ArchitectureTestBase
{
    protected const string API_NAMESPACE = "eMechanic.API";
    protected const string APPLICATION_NAMESPACE = "eMechanic.Application";
    protected const string DOMAIN_NAMESPACE = "eMechanic.Domain";
    protected const string INFRASTRUCTURE_NAMESPACE = "eMechanic.Infrastructure";
    protected const string COMMON_NAMESPACE = "eMechanic.Common";

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
