using System.Reflection;
using AutoMapper;
using MyExpenses.SharedUtils;
using Serilog;

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
        var mapperConfiguration = Configure();

        Mapper = mapperConfiguration.CreateMapper();
    }

    private static MapperConfiguration Configure()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var profiles = executingAssembly
            .GetTypes()
            .Where(t => typeof(Profile).IsAssignableFrom(t));

        var loggerFactory = LoggerConfig.LoggerFactory;
        var configuration = new MapperConfiguration(cfg =>
        {
            foreach (var profile in profiles)
            {
                cfg.AddProfile(profile);
            }
        }, loggerFactory);

        return configuration;
    }
}