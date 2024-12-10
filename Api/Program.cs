// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Goc.Business;
using Goc.Business.Contracts;
using Goc.Business.Hubs;
using Goc.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
Func<CookieValidatePrincipalContext, Task> authValidator = null;
// Add services to the container.

builder.Services.AddSignalR();

builder.Services.AddCors(
    options => options.AddPolicy(
        "CorsPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://purple-cliff-03edabc0f.3.azurestaticapps.net").AllowAnyHeader().AllowAnyMethod()
                .AllowCredentials();
        }));

builder.Services.AddDbContext<GocContext>(option =>
{
    var connection = builder.Configuration.GetConnectionString("db");
    option.UseSqlServer(connection);
});
builder.Services.AddBusiness();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
options =>
{
    options.Events.OnValidatePrincipal = async context =>
    {
        if (authValidator != null)
        {
            await authValidator(context);
        }
    };
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.Events.OnRedirectToAccessDenied = options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnSigningOut = context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return Task.CompletedTask;
    };

    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = true;
});

var app = builder.Build();

authValidator = async context =>
{
    await using var scope = app.Services.CreateAsyncScope();
    var users = scope.ServiceProvider.GetService<IUserService>();
    var user = await users.GetByUpn(context.Principal.Identity.Name);
    if (user == null)
    {
        return;
    }

    var claims = new List<Claim>();
    if (user.IsAdmin)
    {
        claims.Add(new Claim(ClaimTypes.Role, "admin"));
    }

    if (user.IsLeader)
    {
        claims.Add(new Claim(ClaimTypes.Role, "leader"));
    }

    if (user.TeamId.HasValue)
    {
        claims.Add(new Claim("TeamId", user.TeamId?.ToString()));
    }

    if (user.TeamId.HasValue)
    {
        claims.Add(new Claim("CharacterId", user.CharacterId?.ToString()));
    }

    var authIdentity = new GocIdentity(user, claims);
    context.Principal.AddIdentity(authIdentity);
};

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseCookiePolicy(
    new CookiePolicyOptions()
    {
        //HttpOnly = HttpOnlyPolicy.Always,
        //Secure = CookieSecurePolicy.Always,
        MinimumSameSitePolicy = SameSiteMode.None
    });

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/Notification");

app.Run();