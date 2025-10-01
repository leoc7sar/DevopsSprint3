namespace Mottu.Api.Contracts;

public record UsuarioResponse(Guid Id, string Nome, string Email);
public record UsuarioCreateRequest(string Nome, string Email, string Senha);
public record UsuarioUpdateRequest(string Nome, string? Senha); 
public record UsuarioDeleteRequest(string Email);

public record LoginRequest(string Email, string Senha);
public record AuthResponse(string Token, UsuarioResponse Usuario);
