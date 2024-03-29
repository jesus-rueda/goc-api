﻿using Goc.Business.Contracts;
using Goc.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Goc.Business;

public static class DependencyContainer
{
    public static IServiceCollection AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<INotificationSerive, NotificationSerive>();
        services.AddScoped<ICampaignBl, CampaignBl>();
        services.AddScoped<ICharacterBl, CharacterBl>();
        services.AddScoped<ITeamBl, TeamBl>();
        services.AddScoped<IUserBl, UsersBl>();
        services.AddScoped<IMissionBl, MissionBl>();
        services.AddScoped<IEvidenceBl, EvidenceBl>();
        services.AddScoped<IMessageBl, MessageBl>();

        return services;
    }
}