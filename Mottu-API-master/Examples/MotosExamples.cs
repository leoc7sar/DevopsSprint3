using Swashbuckle.AspNetCore.Filters;
using Mottu.Api.Contracts;

namespace Mottu.Api.Examples;

public class MotoCreateRequestExample : IExamplesProvider<MotoCreateRequest>
{
  public MotoCreateRequest GetExamples() => new("DEF-5678", 2023, "Sport");
}

public class MotoResponseExample : IExamplesProvider<MotoResponse>
{
  public MotoResponse GetExamples() => new("DEF-5678", 2023, "Sport");
}
