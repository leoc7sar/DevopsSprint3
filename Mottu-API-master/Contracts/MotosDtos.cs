namespace Mottu.Api.Contracts;

public record MotoResponse(string Placa, int Ano, string Modelo);
public record MotoCreateRequest(string Placa, int Ano, string Modelo);
