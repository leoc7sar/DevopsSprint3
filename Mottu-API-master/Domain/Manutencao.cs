namespace Mottu.Api.Domain;

public enum StatusManutencao { Aberta, Concluida }

public class Manutencao
{
  public string Id { get; set; } = default!;
  public string Placa { get; set; } = default!; 
  public string Problemas { get; set; } = default!;
  public DateTime Data { get; set; } = DateTime.UtcNow;
  public StatusManutencao Status { get; set; } = StatusManutencao.Aberta;
}
