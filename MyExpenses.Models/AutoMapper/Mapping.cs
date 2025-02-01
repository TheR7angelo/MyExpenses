using System.Reflection;
using AutoMapper;

namespace MyExpenses.Models.AutoMapper;

/// <summary>
/// Provides mappings for objects used within the application.
/// </summary>
public static class Mapping
{
    /// <summary>
    /// Provides access to a shared instance of the AutoMapper IMapper.
    /// </summary>
    /// <remarks>
    /// This property allows mapping between object models using AutoMapper.
    /// It is configured to be accessed as a static singleton for convenience.
    /// </remarks>
    public static IMapper Mapper { get; }

    static Mapping()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This hint is disabled because the allocation of MapperConfiguration is intentional and required to set up AutoMapper mappings.
        // The configuration object needs to be created to define how objects are mapped, and its allocation cannot be avoided.
        // The performance impact is minimal as this is only executed during the static constructor, making it acceptable.
        var mapperConfiguration = new MapperConfiguration(Configure);

        Mapper = mapperConfiguration.CreateMapper();
    }

    /// <summary>
    /// Configures the AutoMapper with the necessary profiles from the executing assembly.
    /// </summary>
    /// <param name="cfg">The configuration expression used to define mapping profiles.</param>
    private static void Configure(IMapperConfigurationExpression cfg)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var profiles = executingAssembly
            .GetTypes()
            .Where(t => typeof(Profile).IsAssignableFrom(t));

        foreach (var profile in profiles)
        {
            cfg.AddProfile(profile);
        }
    }
}