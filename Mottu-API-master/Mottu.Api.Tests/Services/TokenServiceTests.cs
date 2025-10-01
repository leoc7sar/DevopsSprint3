using Microsoft.Extensions.Configuration;
using Moq;
using Mottu.Api.Domain;
using Mottu.Api.Services;
using Xunit;

namespace Mottu.Api.Tests.Services;

public class TokenServiceTests
{
  [Fact]
  public void Generate_DeveRetornarUmTokenValido_QuandoUsuarioFornecido()
  {
    // Arrange
    var mockConfiguration = new Mock<IConfiguration>();
        
    mockConfiguration.Setup(c => c["Jwt:Key"])
      .Returns("uma-chave-de-teste-super-segura-e-com-mais-de-32-bytes");

    var tokenService = new TokenService(mockConfiguration.Object);

    var usuario = new Usuario
    {
      Id = Guid.NewGuid(),
      Nome = "Usuario de Teste",
      Email = "teste@email.com"
    };

    // Act
    var token = tokenService.Generate(usuario);

    // Assert
    Assert.NotNull(token);
    Assert.False(string.IsNullOrWhiteSpace(token));
  }
}
