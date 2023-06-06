using Goc.Business.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Goc.Business;

public static class DependencyContainer
{
    public static IServiceCollection AddBussines(this IServiceCollection services)
    {
        services.AddScoped<ITeamBl, TeamBl>();
        services.AddScoped<IMissionBl, MissionBl>();
        return services;
    }
}