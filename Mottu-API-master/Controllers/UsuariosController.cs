using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Contracts;
using Mottu.Api.Data;
using BCrypt.Net;
using Mottu.Api.Examples;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Mottu.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsuariosController(AppDbContext db) { _db = db; }

    [SwaggerOperation(Summary = "Listar usuários", Description = "Retorna lista paginada com links de navegação (HATEOAS).")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PagedUsuariosExample))]
    [ProducesResponseType(typeof(PagedResult<UsuarioResponse>), StatusCodes.Status200OK)]
    [HttpGet(Name = "usuarios")]
    [ProducesResponseType(typeof(PagedResult<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = page < 1 ? 1 : page; pageSize = pageSize <= 0 ? 10 : pageSize;
        var q = _db.Usuarios.AsNoTracking();
        var total = await q.CountAsync();
        var items = await q
            .OrderBy(x => x.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UsuarioResponse(u.Id, u.Nome, u.Email))
            .ToListAsync();

        var links = Mottu.Api.Utils.Hateoas.PaginateLinks(HttpContext, "usuarios", page, pageSize, total);
        return Ok(new PagedResult<UsuarioResponse>(items, page, pageSize, total, links));
    }

    [SwaggerOperation(Summary = "Atualizar usuário", Description = "Atualiza nome e/ou senha pelo e-mail (e-mail não muda).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{email}")]
    public async Task<IActionResult> Update([FromRoute] string email, [FromBody] UsuarioUpdateRequest req)
    {
        var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
        if (u == null) return NotFound(new { message = "Usuário não encontrado" });

        if (!string.IsNullOrWhiteSpace(req.Nome)) u.Nome = req.Nome;
        if (!string.IsNullOrWhiteSpace(req.Senha))
        {
            if (req.Senha.Length < 5) return BadRequest(new { message = "Senha mínima de 5 caracteres" });
            u.SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Senha);
        }
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [SwaggerOperation(Summary = "Excluir usuário", Description = "Apaga um usuário pelo e-mail.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{email}")]
    public async Task<IActionResult> Delete([FromRoute] string email)
    {
        var u = await _db.Usuarios.FirstOrDefaultAsync(x => x.Email == email);
        if (u == null) return NotFound(new { message = "Usuário não encontrado" });

        _db.Usuarios.Remove(u);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
