using System.Reflection;
using AutoMapper;

namespace MyExpenses.Models.AutoMapper;

public static class Mapping
{
    public static IMapper Mapper { get; }

    static Mapping()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
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