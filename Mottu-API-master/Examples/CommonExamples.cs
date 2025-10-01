using System;
using Mottu.Api.Contracts;
using Swashbuckle.AspNetCore.Filters;

namespace Mottu.Api.Examples
{
    public class PagedUsuariosExample : IExamplesProvider<PagedResult<UsuarioResponse>>
    {
        public PagedResult<UsuarioResponse> GetExamples() =>
            new(
                Items: new[]
                {
                    new UsuarioResponse(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Admin Demo", "admin@mottu.com"),
                    new UsuarioResponse(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Gustavo", "gus@mottu.com")
                },
                Page: 1,
                PageSize: 10,
                Total: 2,
                Links: new[]
                {
                    new PageLink("self", "http://localhost:5000/api/v1/usuarios?page=1&pageSize=10", "GET")
                }
            );
    }

    public class PagedMotosExample : IExamplesProvider<PagedResult<MotoResponse>>
    {
        public PagedResult<MotoResponse> GetExamples() =>
            new(
                Items: new[]
                {
                    new MotoResponse("ABC-1234", 2024, "Sport"),
                    new MotoResponse("XYZ-0001", 2023, "Pop")
                },
                Page: 1,
                PageSize: 10,
                Total: 2,
                Links: new[]
                {
                    new PageLink("self", "http://localhost:5000/api/v1/motos?page=1&pageSize=10", "GET")
                }
            );
    }

    public class PagedManutencoesExample : IExamplesProvider<PagedResult<ManutencaoResponse>>
    {
        public PagedResult<ManutencaoResponse> GetExamples() =>
            new(
                Items: new[]
                {
                    new ManutencaoResponse("0042","ABC-1234","Troca de óleo","Aberta", DateTime.UtcNow)
                },
                Page: 1,
                PageSize: 10,
                Total: 1,
                Links: new[]
                {
                    new PageLink("self", "http://localhost:5000/api/v1/manutencoes?status=Aberta&page=1&pageSize=10", "GET")
                }
            );
    }
}
