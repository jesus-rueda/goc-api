using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Goc.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.DotNet.Scaffolding.Shared.Messaging;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public const string aud = "https://parsdev.roseninspection.net/Auth";

        public static MemoryCache myCache = new(new MemoryCacheOptions()
                                                            {
                                                                  
                                                            });

        [HttpPost]
        [Route("{challengeId}")]
        public async Task<LoginResponse> logIn(Guid challengeId)
        {
            var challenge = AuthController.myCache.Get<DeviceAuthResponse>(challengeId);
            var device_code = challenge.DeviceCode;
            var client_id = "pars-dev";
            var grant_type = "urn:ietf:params:oauth:grant-type:device_code";
            
            var idp = new Uri("https://sts.rosen-group.com/adfs/oauth2/token");
            var body = new Dictionary<string, string>()
                       {
                             { "client_id", client_id }
                           , { "grant_type", grant_type }
                           , { "device_code", device_code }
                       };
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
                                      ValidAudience = aud, RequireSignedTokens = false // for testing
                                  };
                var jsonToken = handler.ReadJwtToken(res.AccessToken);
                var tokenS = jsonToken;


                // set the auth cookie
                var claims = tokenS.Claims;

                var claimsIdentity = new ClaimsIdentity(
                    claims, 
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    "upn",
                    null);

                var authProperties = new AuthenticationProperties
                                     {
                                         AllowRefresh = true,
                                         // Refreshing the authentication session should be allowed.

                                         ExpiresUtc = DateTimeOffset.UtcNow.AddDays(15),
                                         // The time at which the authentication ticket expires. A 
                                         // value set here overrides the ExpireTimeSpan option of 
                                         // CookieAuthenticationOptions set with AddCookie.

                                         IsPersistent = true,
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


                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                
                // remove cache info
                AuthController.myCache.Remove(challengeId);

                return new LoginResponse { IsAuthenticated = true, UserId = claimsIdentity.Name, UserName = claimsIdentity.Name, Message = "ok" };
        }
        
        [HttpPost]
        public async Task<ChallengeResponse> RequestLogInChallenge()
        {
            var client_id = "pars-dev";
            var response_type = "device_code";
            var resource = aud;
            var idp = new Uri("https://sts.rosen-group.com/adfs/oauth2/devicecode");
            var body = new Dictionary<string, string>()
                       {
                              { "client_id", client_id }
                           , { "response_type", response_type }
                           , { "resource", resource }
                       };
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

            myCache.Set(challenge.ChallengeId, res, TimeSpan.FromSeconds(res.ExpiresIn));

            return challenge;
        }




        //// GET api/<AuthController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<AuthController>
       
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<AuthController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<AuthController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }


    public class DeviceAuthResponse
    {
        [JsonProperty("device_code")]
        public string DeviceCode { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("interval")]
        public int Interval { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("user_code")]
        public string UserCode { get; set; }

        [JsonProperty("verification_uri")]
        public string VerificationUri { get; set; }

        [JsonProperty("verification_uri_complete")]
        public string VerificationUriComplete { get; set; }

        [JsonProperty("verification_url")]
        public string VerificationUrl { get; set; }
    }

    public class ChallengeResponse
    {
        public Guid ChallengeId { get; set; } = Guid.NewGuid();

        public string Code { get; set; }

        public string VerificationUri { get; set; }

        public string VerificationUriComplete { get; set; }

        public int Interval { get; set; }
    }
}
