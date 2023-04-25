namespace Kuzaine.Services;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public interface IKuzaineService
{

}

public static class KuzaineServiceRegistration
{
    public static IServiceCollection AddKuzaineServices(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
    {
        var assemblies = handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly);

        if (!assemblies.Any())
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");
        }

        foreach (var assembly in assemblies)
        {
            var rules = assembly.GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(IKuzaineService)) == typeof(IKuzaineService));

            foreach (var rule in rules)
            {
                foreach (var @interface in rule.GetInterfaces())
                {
                    services.Add(new ServiceDescriptor(@interface, rule, ServiceLifetime.Scoped));
                }
            }
        }

        return services;
    }
}