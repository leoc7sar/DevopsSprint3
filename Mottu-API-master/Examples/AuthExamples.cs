using Swashbuckle.AspNetCore.Filters;
using Mottu.Api.Contracts;
using System;

namespace Mottu.Api.Examples;

public class LoginRequestExample : IExamplesProvider<LoginRequest>
{
  public LoginRequest GetExamples() => new("admin@mottu.com","123456");
}

public class AuthResponseExample : IExamplesProvider<AuthResponse>
{
  public AuthResponse GetExamples() =>
    new("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.fake.token",
      new UsuarioResponse(Guid.Parse("99999999-9999-9999-9999-999999999999"), "Admin Demo", "admin@mottu.com"));
}

public class UsuarioCreateRequestExample : IExamplesProvider<UsuarioCreateRequest>
{
  public UsuarioCreateRequest GetExamples() => new("Gustavo Camargo", "gus@mottu.com", "minhaSenha123");
}

public class UsuarioResponseExample : IExamplesProvider<UsuarioResponse>
{
  public UsuarioResponse GetExamples() =>
    new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Gustavo Camargo","gus@mottu.com");
}
