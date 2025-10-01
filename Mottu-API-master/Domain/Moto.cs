namespace Mottu.Api.Domain;

public enum ModeloMoto { Sport, Pop, E /* -E */ }

public class Moto
{
  public int Id { get; set; }
  public string Placa { get; set; } = default!;    
  public int Ano { get; set; }                    
  public ModeloMoto Modelo { get; set; }
}
