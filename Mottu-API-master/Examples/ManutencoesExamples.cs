using Swashbuckle.AspNetCore.Filters;
using Mottu.Api.Contracts;
using System;

namespace Mottu.Api.Examples;

public class ManutencaoCreateRequestExample : IExamplesProvider<ManutencaoCreateRequest>
{
  public ManutencaoCreateRequest GetExamples() => new("DEF-5678", "Troca de óleo e revisão");
}

public class ManutencaoUpdateRequestExample : IExamplesProvider<ManutencaoUpdateRequest>
{
  public ManutencaoUpdateRequest GetExamples() => new("Troca de óleo concluída","Concluida");
}

public class ManutencaoResponseExample : IExamplesProvider<ManutencaoResponse>
{
  public ManutencaoResponse GetExamples() =>
    new("0042","DEF-5678","Troca de óleo e revisão","Aberta", DateTime.UtcNow);
}
