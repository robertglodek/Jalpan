using Jalpan.Auth.Jwt.Managers;
using Jalpan.Auth.Jwt.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Jalpan.Auth.Jwt;

public static class Extensions
{
    private const string DefaultSectionName = "jwt";
    private const string RegistryKey = "auth";

    public static IJalpanBuilder AddJwt(this IJalpanBuilder builder, string sectionName = DefaultSectionName)
    {
        sectionName = string.IsNullOrWhiteSpace(sectionName) ? DefaultSectionName : sectionName;

        if (!builder.TryRegister(RegistryKey))
        {
            return builder;
        }

        var section = builder.Configuration.GetSection(sectionName);
        var options = section.BindOptions<AuthOptions>();
        builder.Services.Configure<AppOptions>(section);

        var tokenValidationParameters = CreateTokenValidationParameters(options);

        var securityKey = GetSecurityKey(options, out var algorithm);

        ConfigureJwtBearerAuthentication(builder.Services, options, tokenValidationParameters);

        builder.Services.AddAuthorization();

        builder.Services.AddSingleton(new SecurityKeyDetails(securityKey, algorithm));
        builder.Services.AddSingleton<IJwtTokenManager, JwtTokenManager>();
        builder.Services.AddSingleton(tokenValidationParameters);
        builder.Services.AddSingleton<IAccessTokenManager, InMemoryAccessTokenManager>();
        builder.Services.AddTransient<AccessTokenValidationMiddleware>();

        return builder;
    }

    private static SecurityKey GetSecurityKey(AuthOptions options, out string algorithm)
    {
        algorithm = options.Algorithm;
        
        SecurityKey? securityKey = null;

        if (options.Certificate != null)
        {
            securityKey = LoadCertificate(options);
        }

        if (securityKey != null)
        {
            if (string.IsNullOrWhiteSpace(algorithm))
            {
                algorithm = SecurityAlgorithms.RsaSha256;
            }
            
            return securityKey;
        }
        
        if (string.IsNullOrWhiteSpace(options.Jwt.IssuerSigningKey))
        {
            throw new InvalidOperationException("Missing issuer signing key.");
        }

        var rawKey = Encoding.UTF8.GetBytes(options.Jwt.IssuerSigningKey);
        securityKey = new SymmetricSecurityKey(rawKey);
            
        if (string.IsNullOrWhiteSpace(algorithm))
        {
            algorithm = SecurityAlgorithms.HmacSha256;
        }

        return securityKey;
    }

    private static X509SecurityKey? LoadCertificate(AuthOptions options)
    {
        var certificate = TryLoadCertificateFromLocation(options) ?? TryLoadCertificateFromRawData(options);

        if (certificate == null) return null;
        var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
        Console.WriteLine($"Using X.509 certificate for {actionType} tokens.");
        return new X509SecurityKey(certificate);

    }

    private static X509Certificate2? TryLoadCertificateFromLocation(AuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Certificate!.Location))
        {
            return null;
        }

        var password = options.Certificate.Password;
        var certificate = string.IsNullOrWhiteSpace(password)
            ? new X509Certificate2(options.Certificate.Location)
            : new X509Certificate2(options.Certificate.Location, password);

        var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
        Console.WriteLine($"Loaded X.509 certificate from location: '{options.Certificate.Location}' {keyType}.");
        return certificate;
    }

    private static X509Certificate2? TryLoadCertificateFromRawData(AuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Certificate!.RawData))
        {
            return null;
        }

        var rawData = Convert.FromBase64String(options.Certificate.RawData);
        var password = options.Certificate.Password;
        var certificate = string.IsNullOrWhiteSpace(password)
            ? new X509Certificate2(rawData)
            : new X509Certificate2(rawData, password);

        var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
        Console.WriteLine($"Loaded X.509 certificate from raw data {keyType}.");
        return certificate;
    }

    private static TokenValidationParameters CreateTokenValidationParameters(AuthOptions options)
    {
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

        return tokenValidationParameters;
    }

    private static void ConfigureJwtBearerAuthentication(IServiceCollection services, AuthOptions options, TokenValidationParameters tokenValidationParameters)
    {
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwtBearerOptions =>
        {
            jwtBearerOptions.Authority = options.Jwt.Authority;
            jwtBearerOptions.Audience = options.Jwt.Audience;
            jwtBearerOptions.MetadataAddress = options.Jwt.MetadataAddress;
            jwtBearerOptions.SaveToken = options.Jwt.SaveToken;
            jwtBearerOptions.RefreshOnIssuerKeyNotFound = options.Jwt.RefreshOnIssuerKeyNotFound;
            jwtBearerOptions.RequireHttpsMetadata = options.Jwt.RequireHttpsMetadata;
            jwtBearerOptions.IncludeErrorDetails = options.Jwt.IncludeErrorDetails;
            jwtBearerOptions.TokenValidationParameters = tokenValidationParameters;
            if (!string.IsNullOrWhiteSpace(options.Jwt.Challenge))
            {
                jwtBearerOptions.Challenge = options.Jwt.Challenge;
            }
        });
    }

    public static IJalpanBuilder AddDistributedAccessTokenValidator(this IJalpanBuilder builder)
    {
        builder.Services.AddSingleton<IAccessTokenManager, DistributedAccessTokenManager>();
        return builder;
    }

    public static IApplicationBuilder UseAccessTokenValidator(this IApplicationBuilder app) 
        => app.UseMiddleware<AccessTokenValidationMiddleware>();
}