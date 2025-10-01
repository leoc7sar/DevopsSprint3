using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Contracts;
using Mottu.Api.Data;
using Mottu.Api.Domain;
using Mottu.Api.Examples;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Mottu.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ManutencoesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly Random _rng = new();

    public ManutencoesController(AppDbContext db) { _db = db; }

    private string GerarId4DigitosUnico()
    {
        string id;
        do { id = _rng.Next(0, 10000).ToString("0000"); }
        while (_db.Manutencoes.Any(x => x.Id == id));
        return id;
    }

    [SwaggerOperation(Summary = "Abrir manutenção", Description = "Cadastra manutenção pela placa da moto (gera ID de 4 dígitos).")]
    [SwaggerRequestExample(typeof(ManutencaoCreateRequest), typeof(ManutencaoCreateRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ManutencaoResponseExample))]
    [ProducesResponseType(typeof(ManutencaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ManutencaoCreateRequest req)
    {
        var placa = (req.Placa ?? "").ToUpper();
        var moto = await _db.Motos.AsNoTracking().FirstOrDefaultAsync(x => x.Placa == placa);
        if (moto == null) return NotFound(new { message = "Moto não encontrada" });

        var m = new Domain.Manutencao
        {
            Id = GerarId4DigitosUnico(),
            Placa = placa,
            Problemas = req.Problemas,
            Data = DateTime.UtcNow,
            Status = StatusManutencao.Aberta
        };
        _db.Manutencoes.Add(m);
        await _db.SaveChangesAsync();

        var resp = new ManutencaoResponse(m.Id, m.Placa, m.Problemas, m.Status.ToString(), m.Data);
        return Created($"/api/v1/manutencoes/{m.Id}", resp);
    }

    [SwaggerOperation(Summary = "Atualizar manutenção", Description = "Atualiza descrição e/ou status (Aberta/Concluida); não muda a placa.")]
    [SwaggerRequestExample(typeof(ManutencaoUpdateRequest), typeof(ManutencaoUpdateRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ManutencaoResponseExample))]
    [ProducesResponseType(typeof(ManutencaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] ManutencaoUpdateRequest req)
    {
        var m = await _db.Manutencoes.FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return NotFound(new { message = "Manutenção não encontrada" });

        m.Problemas = req.Problemas ?? m.Problemas;
        if (Enum.TryParse<StatusManutencao>(req.Status, out var st)) m.Status = st;

        await _db.SaveChangesAsync();
        return Ok(new ManutencaoResponse(m.Id, m.Placa, m.Problemas, m.Status.ToString(), m.Data));
    }

    [SwaggerOperation(Summary = "Excluir manutenção", Description = "Exclui uma manutenção pelo ID de 4 dígitos.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var m = await _db.Manutencoes.FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return NotFound(new { message = "Manutenção não encontrada" });

        _db.Manutencoes.Remove(m);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [SwaggerOperation(Summary = "Buscar manutenção por ID", Description = "Retorna placa, problemas, data e status.")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ManutencaoResponseExample))]
    [ProducesResponseType(typeof(ManutencaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var m = await _db.Manutencoes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (m == null) return NotFound(new { message = "Manutenção não encontrada" });
        return Ok(new ManutencaoResponse(m.Id, m.Placa, m.Problemas, m.Status.ToString(), m.Data));
    }

    [SwaggerOperation(Summary = "Listar manutenções", Description = "Filtrar por status=Aberta/Concluida. Retorna paginação com HATEOAS.")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PagedManutencoesExample))]
    [ProducesResponseType(typeof(PagedResult<ManutencaoResponse>), StatusCodes.Status200OK)]
    [HttpGet(Name = "manutencoes")]
    [ProducesResponseType(typeof(PagedResult<ManutencaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? status = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = page < 1 ? 1 : page; pageSize = pageSize <= 0 ? 10 : pageSize;

        var q = _db.Manutencoes.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<StatusManutencao>(status, true, out var st))
            q = q.Where(x => x.Status == st);

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(x => x.Data)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new ManutencaoResponse(m.Id, m.Placa, m.Problemas, m.Status.ToString(), m.Data))
            .ToListAsync();

        var links = Mottu.Api.Utils.Hateoas.PaginateLinks(HttpContext, "manutencoes", page, pageSize, total);
        return Ok(new PagedResult<ManutencaoResponse>(items, page, pageSize, total, links));
    }
}
