namespace Mottu.Api.Contracts;

public record ManutencaoResponse(string Id, string Placa, string Problemas, string Status, DateTime Data);
public record ManutencaoCreateRequest(string Placa, string Problemas);
public record ManutencaoUpdateRequest(string Problemas, string Status); 
