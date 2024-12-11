// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Goc.Api.Controllers;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Goc.Business.Contracts;
using Goc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    #region Fields

    public const string aud = "https://parsdev.roseninspection.net/Auth";

    public static MemoryCache myCache = new(new MemoryCacheOptions() { });

    private readonly IUserService myUsers;

    #endregion

    public AuthController(IUserService users)
    {
        this.myUsers = users;
    }
    #region Methods

    [HttpDelete]    
    public async Task<ActionResult> logOut()
    {
        await this.HttpContext.SignOutAsync();
        return this.Ok();
    }
    

    [HttpPost]
    [Route("{challengeId}")]
    public async Task<LoginResponse> logIn(Guid challengeId)
    {
        var challenge = AuthController.myCache.Get<DeviceAuthResponse>(challengeId);
        if (challenge == null)
        {
            return new LoginResponse { IsAuthenticated = false, Message = "User not log in yet" };
        }

        var device_code = challenge.DeviceCode;
        var client_id = "pars-dev";
        var grant_type = "urn:ietf:params:oauth:grant-type:device_code";

        var idp = new Uri("https://sts.rosen-group.com/adfs/oauth2/token");
        var body = new Dictionary<string, string>() { { "client_id", client_id }, { "grant_type", grant_type }, { "device_code", device_code } };
        var http = new HttpClient();
        var response = await http.PostAsync(idp, new FormUrlEncodedContent(body));
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return new LoginResponse { IsAuthenticated = false, Message = "User not log in yet" };
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var ser = new JsonSerializer();
        var sr = new StringReader(responseBody);
        var jsonReader = new JsonTextReader(sr);
        var res = ser.Deserialize<AuthResponse>(jsonReader);

        //validate token
        var handler = new JwtSecurityTokenHandler();
        var validParams = new TokenValidationParameters
                          {
                              ValidAudience = AuthController.aud, RequireSignedTokens = false // for testing
                          };
        var jsonToken = handler.ReadJwtToken(res.AccessToken);
        var tokenS = jsonToken;

        // set the auth cookie
        var claims = tokenS.Claims;

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme, "upn", null);

        var authProperties = new AuthenticationProperties
                             {
                                 AllowRefresh = true,
                                 // Refreshing the authentication session should be allowed.

                                 ExpiresUtc = DateTimeOffset.UtcNow.AddDays(15),
                                 // The time at which the authentication ticket expires. A 
                                 // value set here overrides the ExpireTimeSpan option of 
                                 // CookieAuthenticationOptions set with AddCookie.

                                 IsPersistent = true
                                 // Whether the authentication session is persisted across 
                                 // multiple requests. When used with cookies, controls
                                 // whether the cookie's lifetime is absolute (matching the
                                 // lifetime of the authentication ticket) or session-based.

                                 //IssuedUtc = <DateTimeOffset>,
                                 // The time at which the authentication ticket was issued.

                                 //RedirectUri = <string>
                                 // The full path or absolute URI to be used as an http 
                                 // redirect response value.
                             };

        await this.myUsers.AutoRegisterUser(claimsIdentity.Name);
        await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        
        

        // remove cache info
        AuthController.myCache.Remove(challengeId);

        return new LoginResponse { IsAuthenticated = true, UserId = claimsIdentity.Name, UserName = claimsIdentity.Name, Message = "ok" };
    }

    [HttpPost]
    public async Task<ChallengeResponse> RequestLogInChallenge()
    {
        var client_id = "pars-dev";
        var response_type = "device_code";
        var resource = AuthController.aud;
        var idp = new Uri("https://sts.rosen-group.com/adfs/oauth2/devicecode");
        var body = new Dictionary<string, string>() { { "client_id", client_id }, { "response_type", response_type }, { "resource", resource } };
        var http = new HttpClient();
        var response = await http.PostAsync(idp, new FormUrlEncodedContent(body));
        var responseBody = await response.Content.ReadAsStringAsync();
        var ser = new JsonSerializer();
        var sr = new StringReader(responseBody);
        var jsonReader = new JsonTextReader(sr);
        var res = ser.Deserialize<DeviceAuthResponse>(jsonReader);

        var challenge = new ChallengeResponse
                        {
                            Code = res.UserCode,
                            VerificationUri = res.VerificationUri,
                            VerificationUriComplete = res.VerificationUriComplete,
                            Interval = res.Interval
                        };

        AuthController.myCache.Set(challenge.ChallengeId, res, TimeSpan.FromSeconds(res.ExpiresIn));

        return challenge;
    }

    [HttpGet]
    [Authorize]
    [Route("profile")]
    public async Task<ActionResult<ICampaingProfile>> GetProfile()
    {
        var user = this.User.GetGocUser();
        return this.Ok(user);
    }

    #endregion

}

public class UserProfile
{
}