﻿using System.Security.Claims;

namespace Jalpan.Auth.Jwt;

public class JsonWebTokenPayload
{
    public string Subject { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime Expiry { get; set; }
    public IEnumerable<string>? Roles { get; set; }
    public IEnumerable<Claim> Claims { get; set; } = [];
}
