using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mottu.Api.Contracts;
using Mottu.Api.Data;
using Mottu.Api.Domain;
using Mottu.Api.Examples;
using Mottu.Api.Utils;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Mottu.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MotosController : ControllerBase
{
    private readonly AppDbContext _db;
    public MotosController(AppDbContext db) { _db = db; }

    [SwaggerOperation(Summary = "Cadastrar moto", Description = "Placa no formato XXX-1234, Ano 2020–2025, Modelo: Sport | Pop | -E.")]
    [SwaggerRequestExample(typeof(MotoCreateRequest), typeof(MotoCreateRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status201Created, typeof(MotoResponseExample))]
    [ProducesResponseType(typeof(MotoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MotoCreateRequest req)
    {
        var placa = (req.Placa ?? "").ToUpper();
        if (!Validators.PlacaValida(placa)) return BadRequest(new { message = "Placa inválida (formato XXX-1234)" });
        if (!Validators.AnoValido(req.Ano)) return BadRequest(new { message = "Ano deve ser entre 2020 e 2025" });
        if (!Validators.ModeloValido(req.Modelo)) return BadRequest(new { message = "Modelo deve ser Sport, Pop ou -E" });

        var exists = await _db.Motos.AnyAsync(x => x.Placa == placa);
        if (exists) return Conflict(new { message = "Placa já cadastrada" });

        var m = new Moto { Placa = placa, Ano = req.Ano, Modelo = Validators.ParseModelo(req.Modelo) };
        _db.Motos.Add(m);
        await _db.SaveChangesAsync();

        var resp = new MotoResponse(m.Placa, m.Ano, Validators.ModeloToString(m.Modelo));
        return Created($"/api/v1/motos/{m.Placa}", resp);
    }

    [SwaggerOperation(Summary = "Buscar moto por placa", Description = "Retorna placa, ano e modelo.")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MotoResponseExample))]
    [ProducesResponseType(typeof(MotoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{placa}")]
    public async Task<IActionResult> GetByPlaca([FromRoute] string placa)
    {
        placa = (placa ?? "").ToUpper();
        var m = await _db.Motos.AsNoTracking().FirstOrDefaultAsync(x => x.Placa == placa);
        if (m == null) return NotFound(new { message = "Moto não encontrada" });

        return Ok(new MotoResponse(m.Placa, m.Ano, Validators.ModeloToString(m.Modelo)));
    }

    [SwaggerOperation(Summary = "Listar motos", Description = "Lista paginada com HATEOAS.")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PagedMotosExample))]
    [ProducesResponseType(typeof(PagedResult<MotoResponse>), StatusCodes.Status200OK)]
    [HttpGet(Name = "motos")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = page < 1 ? 1 : page; pageSize = pageSize <= 0 ? 10 : pageSize;
        var q = _db.Motos.AsNoTracking();
        var total = await q.CountAsync();
        var items = await q
            .OrderBy(x => x.Placa)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MotoResponse(m.Placa, m.Ano, Validators.ModeloToString(m.Modelo)))
            .ToListAsync();

        var links = Mottu.Api.Utils.Hateoas.PaginateLinks(HttpContext, "motos", page, pageSize, total);
        return Ok(new PagedResult<MotoResponse>(items, page, pageSize, total, links));
    }

    [SwaggerOperation(Summary = "Excluir moto", Description = "Exclui uma moto pela placa.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{placa}")]
    public async Task<IActionResult> Delete([FromRoute] string placa)
    {
        placa = (placa ?? "").ToUpper();
        var m = await _db.Motos.FirstOrDefaultAsync(x => x.Placa == placa);
        if (m == null) return NotFound(new { message = "Moto não encontrada" });

        _db.Motos.Remove(m);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
