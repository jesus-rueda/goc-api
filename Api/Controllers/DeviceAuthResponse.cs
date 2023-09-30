// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Goc.Api.Controllers;

using Newtonsoft.Json;

public class DeviceAuthResponse
{
    #region Properties

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

    #endregion
}