using System.Reflection;

namespace RoomHub.Api.Common.Cqrs;

public static class HandlerRegistrationExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type[] handlerContracts = [typeof(ICommandHandler<,>)];

        var implementations = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (var implementation in implementations)
        {
            var contracts = implementation.GetInterfaces()
                .Where(i => i.IsGenericType && handlerContracts.Contains(i.GetGenericTypeDefinition()));

            foreach (var contract in contracts)
                services.AddScoped(contract, implementation);
        }

        return services;
    }
}
