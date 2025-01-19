using System.Reflection;
using AutoMapper;

namespace MyExpenses.Models.AutoMapper;

public static class Mapping
{
    public static IMapper Mapper { get; }

    static Mapping()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // This hint is disabled because the allocation of MapperConfiguration is intentional and required to set up AutoMapper mappings.
        // The configuration object needs to be created to define how objects are mapped, and its allocation cannot be avoided.
        // The performance impact is minimal as this is only executed during the static constructor, making it acceptable.
        var config = new MapperConfiguration(cfg =>
        {
            var profiles = executingAssembly
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t));

            foreach (var profile in profiles)
            {
                cfg.AddProfile(profile);
            }
        });

        Mapper = config.CreateMapper();
    }
}