using System.Text.RegularExpressions;
using Mottu.Api.Domain;

namespace Mottu.Api.Utils;

public static class Validators
{
  private static readonly Regex _placaRegex = new(@"^[A-Z]{3}-\d{4}$");
  public static bool PlacaValida(string placa) => _placaRegex.IsMatch(placa ?? "");

  public static bool AnoValido(int ano) => ano >= 2020 && ano <= 2025;

  public static bool ModeloValido(string modelo) =>
    new[] { "Sport", "Pop", "-E" }.Contains(modelo);

  public static ModeloMoto ParseModelo(string modelo) =>
    modelo == "-E" ? ModeloMoto.E :
    Enum.TryParse<ModeloMoto>(modelo, ignoreCase: true, out var m) ? m : throw new ArgumentException("Modelo inválido");

  public static string ModeloToString(ModeloMoto m) => m == ModeloMoto.E ? "-E" : m.ToString();
}
