using Goc.Business.Contracts;
using Goc.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Goc.Business;

public static class DependencyContainer
{
    public static IServiceCollection AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<INotificationSerive, NotificationSerive>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<ICharacterBl, CharacterBl>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IUserService, UsersService>();
        services.AddScoped<IMissionBl, MissionBl>();
        services.AddScoped<IEvidenceService, EvidenceService>();
        services.AddScoped<IMessageService, MessageService>();

        return services;
    }
}