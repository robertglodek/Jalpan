﻿using Jalpan.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Jalpan.Auth;

public static class Extensions
{
    private const string SectionName = "jwt";
    private const string RegistryName = "auth";

    public static IJalpanBuilder AddJwt(this IJalpanBuilder builder, string sectionName = SectionName,  Action<JwtBearerOptions>? optionsFactory = null)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<AuthOptions>();
        builder.Services.Configure<AppOptions>(section);

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication();

        if (options.Jwt is null)
        {
            throw new InvalidOperationException("JWT options cannot be null.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = options.Jwt.RequireAudience,
            ValidIssuer = options.Jwt.ValidIssuer,
            ValidIssuers = options.Jwt.ValidIssuers,
            ValidateActor = options.Jwt.ValidateActor,
            ValidAudience = options.Jwt.ValidAudience,
            ValidAudiences = options.Jwt.ValidAudiences,
            ValidateAudience = options.Jwt.ValidateAudience,
            ValidateIssuer = options.Jwt.ValidateIssuer,
            ValidateLifetime = options.Jwt.ValidateLifetime,
            ValidateTokenReplay = options.Jwt.ValidateTokenReplay,
            ValidateIssuerSigningKey = options.Jwt.ValidateIssuerSigningKey,
            SaveSigninToken = options.Jwt.SaveSigninToken,
            RequireExpirationTime = options.Jwt.RequireExpirationTime,
            RequireSignedTokens = options.Jwt.RequireSignedTokens,
            ClockSkew = TimeSpan.Zero
        };

        var hasCertificate = false;
        var algorithm = options.Algorithm;
        SecurityKey? securityKey = null;
        if (options.Certificate is not null)
        {
            X509Certificate2? certificate = null;
            var password = options.Certificate.Password;
            var hasPassword = !string.IsNullOrWhiteSpace(password);
            if (!string.IsNullOrWhiteSpace(options.Certificate.Location))
            {
                certificate = hasPassword
                    ? new X509Certificate2(options.Certificate.Location, password)
                    : new X509Certificate2(options.Certificate.Location);
                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                Console.WriteLine($"Loaded X.509 certificate from location: '{options.Certificate.Location}' {keyType}.");
            }

            if (!string.IsNullOrWhiteSpace(options.Certificate.RawData))
            {
                var rawData = Convert.FromBase64String(options.Certificate.RawData);
                certificate = hasPassword
                    ? new X509Certificate2(rawData, password)
                    : new X509Certificate2(rawData);
                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
                Console.WriteLine($"Loaded X.509 certificate from raw data {keyType}.");
            }

            if (certificate is not null)
            {
                if (string.IsNullOrWhiteSpace(options.Algorithm))
                {
                    algorithm = SecurityAlgorithms.RsaSha256;
                }

                hasCertificate = true;
                securityKey = new X509SecurityKey(certificate);
                tokenValidationParameters.IssuerSigningKey = securityKey;
                var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
                Console.WriteLine($"Using X.509 certificate for {actionType} tokens.");
            }
        }

        if (!hasCertificate)
        {
            if (string.IsNullOrWhiteSpace(options.Jwt.IssuerSigningKey))
            {
                throw new InvalidOperationException("Missing issuer signing key.");
            }

            if (string.IsNullOrWhiteSpace(options.Algorithm))
            {
                algorithm = SecurityAlgorithms.HmacSha256;
            }

            var rawKey = Encoding.UTF8.GetBytes(options.Jwt.IssuerSigningKey);
            securityKey = new SymmetricSecurityKey(rawKey);
            tokenValidationParameters.IssuerSigningKey = securityKey;
            Console.WriteLine("Using symmetric encryption for issuing tokens.");
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.AuthenticationType))
        {
            tokenValidationParameters.AuthenticationType = options.Jwt.AuthenticationType;
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.NameClaimType))
        {
            tokenValidationParameters.NameClaimType = options.Jwt.NameClaimType;
        }

        if (!string.IsNullOrWhiteSpace(options.Jwt.RoleClaimType))
        {
            tokenValidationParameters.RoleClaimType = options.Jwt.RoleClaimType;
        }
   
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwtBearerOptions => 
        {
            jwtBearerOptions.Authority = options.Jwt.Authority;
            jwtBearerOptions.Audience = options.Jwt.Audience;
            jwtBearerOptions.MetadataAddress = options.Jwt.MetadataAddress ?? string.Empty;
            jwtBearerOptions.SaveToken = options.Jwt.SaveToken;
            jwtBearerOptions.RefreshOnIssuerKeyNotFound = options.Jwt.RefreshOnIssuerKeyNotFound;
            jwtBearerOptions.RequireHttpsMetadata = options.Jwt.RequireHttpsMetadata;
            jwtBearerOptions.IncludeErrorDetails = options.Jwt.IncludeErrorDetails;
            jwtBearerOptions.TokenValidationParameters = tokenValidationParameters;
            if (!string.IsNullOrWhiteSpace(options.Jwt.Challenge))
            {
                jwtBearerOptions.Challenge = options.Jwt.Challenge;
            }

            optionsFactory?.Invoke(jwtBearerOptions);
        });

        if (securityKey is not null)
        {
            builder.Services.AddSingleton(new SecurityKeyDetails(securityKey, algorithm));
            builder.Services.AddSingleton<IJwtTokenManager, JwtTokenManager>();
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton(tokenValidationParameters);

        return builder;
    }
}