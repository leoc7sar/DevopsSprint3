CREATE TABLE motos (
  id INT IDENTITY(1,1) PRIMARY KEY,
  placa NVARCHAR(10) NOT NULL,
  modelo NVARCHAR(50) NOT NULL,
  status NVARCHAR(20) NOT NULL
);
GO
INSERT INTO motos (placa, modelo, status) VALUES
('ABC1D23','Honda CG 160','disponivel'),
('XYZ9K88','Yamaha Fazer 250','manutencao');
GO
