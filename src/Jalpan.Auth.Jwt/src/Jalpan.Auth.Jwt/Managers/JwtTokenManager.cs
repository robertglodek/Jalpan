﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Jalpan.Time;
using Microsoft.IdentityModel.Tokens;

namespace Jalpan.Auth.Jwt.Managers;

internal sealed class JwtTokenManager : IJwtTokenManager
{
    private static readonly Dictionary<string, IEnumerable<string>> EmptyClaims = [];
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    private readonly string? _issuer;
    private readonly TimeSpan _expiry;
    private readonly IDateTime _dateTime;
    private readonly SigningCredentials _signingCredentials;
    private readonly string? _audience;

    public JwtTokenManager(IOptions<AuthOptions> options, SecurityKeyDetails securityKeyDetails, IDateTime dateTime)
    {
        var jwtOptions = options.Value.Jwt;

        _audience = jwtOptions.Audience;
        _issuer = jwtOptions.Issuer;
        _expiry = jwtOptions.Expiry;
        _dateTime = dateTime;
        _signingCredentials = new SigningCredentials(securityKeyDetails.Key, securityKeyDetails.Algorithm);
    }

    public JsonWebToken CreateToken(
        string userId,
        string? email = null,
        string? role = null,
        IDictionary<string, IEnumerable<string>>? claims = null)
    {
        if (_signingCredentials.Key is X509SecurityKey { PrivateKeyStatus: PrivateKeyStatus.DoesNotExist })
        {
            throw new InvalidOperationException(
                "Cannot create JWT: The X509SecurityKey does not contain a private key required for signing.");
        }

        var now = _dateTime.Now;

        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.UniqueName, userId)
        };

        if (!string.IsNullOrWhiteSpace(email))
        {
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            jwtClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (!string.IsNullOrWhiteSpace(_audience))
        {
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, _audience));
        }

        if (claims?.Any() is true)
        {
            foreach (var (claim, values) in claims)
            {
                jwtClaims.AddRange(values.Select(value => new Claim(claim, value)));
            }
        }

        var expires = now.Add(_expiry);

        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            claims: jwtClaims,
            notBefore: now,
            expires: expires,
            signingCredentials: _signingCredentials
        );

        var token = _jwtSecurityTokenHandler.WriteToken(jwt);

        return new JsonWebToken
        {
            AccessToken = token,
            Expiry = new DateTimeOffset(expires).ToUnixTimeMilliseconds(),
            UserId = userId,
            Email = email ?? string.Empty,
            Role = role ?? string.Empty,
            Claims = claims ?? EmptyClaims
        };
    }
}