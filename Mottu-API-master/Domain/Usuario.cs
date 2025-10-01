namespace Mottu.Api.Domain;

public class Usuario
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public string Nome { get; set; } = default!;
  public string Email { get; set; } = default!;  
  public string SenhaHash { get; set; } = default!;
}
