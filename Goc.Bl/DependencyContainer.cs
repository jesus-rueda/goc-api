using Goc.Business.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Goc.Business;

public static class DependencyContainer
{
    public static IServiceCollection AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<ICampaignBl, CampaignBl>();
        services.AddScoped<ICharacterBl, CharacterBl>();
        services.AddScoped<ITeamBl, TeamBl>();
        services.AddScoped<IMissionBl, MissionBl>();
        services.AddScoped<IEvidenceBl, EvidenceBl>();
        return services;
    }
}