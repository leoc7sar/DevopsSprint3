using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Mottu.Api.Contracts;
using Mottu.Api.Data;
using Mottu.Api.Domain;
using Mottu.Api.Examples;
using Mottu.Api.Services;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;


namespace Mottu.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _token;
    public AuthController(AppDbContext db, ITokenService token) { _db = db; _token = token; }

    [SwaggerOperation(Summary = "Registrar usuário", Description = "Cria um usuário novo e já retorna token para uso no app.")]
    [SwaggerRequestExample(typeof(UsuarioCreateRequest), typeof(UsuarioCreateRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(AuthResponseExample))]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UsuarioCreateRequest req)
    {
        if (!req.Email?.Contains('@') ?? true) return BadRequest(new { message = "Email inválido" });
        if ((req.Senha?.Length ?? 0) < 5) return BadRequest(new { message = "Senha mínima de 5 caracteres" });

        var exists = await _db.Usuarios.AnyAsync(x => x.Email == req.Email);
        if (exists) return Conflict(new { message = "Email já cadastrado" });

        var u = new Usuario { Nome = req.Nome, Email = req.Email, SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha) };
        _db.Usuarios.Add(u);
        await _db.SaveChangesAsync();

        var token = _token.Generate(u);
        var resp = new AuthResponse(token, new UsuarioResponse(u.Id, u.Nome, u.Email));
        return Created($"/api/v1/usuarios/{u.Email}", resp);
    }

    [SwaggerOperation(Summary = "Fazer login", Description = "Valida e-mail e senha e retorna um token JWT.")]
    [SwaggerRequestExample(typeof(LoginRequest), typeof(LoginRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AuthResponseExample))]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.Email == req.Email);
        if (u == null || !BCrypt.Net.BCrypt.Verify(req.Senha, u.SenhaHash))
            return Unauthorized(new { message = "Credenciais inválidas" });

        var token = _token.Generate(u);
        return Ok(new AuthResponse(token, new UsuarioResponse(u.Id, u.Nome, u.Email)));
    }
}
