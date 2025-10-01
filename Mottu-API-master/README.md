# Mottu API — Gestão de Frotas (.NET 8)

API RESTful desenvolvida em **ASP.NET Core 8** para o gerenciamento de **Usuários**, **Motos** e **Manutenções**, com autenticação via **JWT**.  
O projeto segue boas práticas de REST com **paginação HATEOAS**, **status codes** bem definidos e **documentação interativa** via **Swagger/OpenAPI** (com exemplos de payload).

---

## Índice
- [Integrantes do Grupo](#integrantes-do-grupo)
- [Arquitetura e Decisões de Design](#arquitetura-e-decisões-de-design)
  - [Camadas](#camadas)
  - [Justificativa do Domínio](#justificativa-do-domínio)
- [Tecnologias e Pacotes Utilizados](#tecnologias-e-pacotes-utilizados)
- [Como Executar a API](#como-executar-a-api)
- [Como Testar a API (arquivo `Mottu.Api.http`)](#como-testar-a-api-arquivo-mottuapihttp)
- [Exemplos de Uso dos Endpoints](#exemplos-de-uso-dos-endpoints)
  - [Autenticação (Login)](#autenticação-login)
  - [Cadastrar uma nova moto](#cadastrar-uma-nova-moto)
  - [Listar manutenções com paginação e HATEOAS](#listar-manutenções-com-paginação-e-hateoas)
- [Notas e Dicas](#notas-e-dicas)

---

## Integrantes do Grupo

- **Gustavo Camargo de Andrade** (RM555562) — 2TDSPF  
- **Rodrigo Souza Mantovanello** (RM555451) — 2TDSPF  
- **Leonardo Cesar Rodrigues Nascimento** (RM558373) — 2TDSPF

---

## Arquitetura e Decisões de Design

A solução foi organizada para ser **simples, escalável e de fácil manutenção**, separando responsabilidades em camadas claras. Isso favorece desacoplamento e testabilidade.

### Camadas
- **Domain**: classes de negócio (`Usuario`, `Moto`, `Manutencao`). Núcleo do domínio, **sem dependências de frameworks**.
- **Data**: persistência com `AppDbContext` (Entity Framework Core) e **Migrations**. Isola o acesso ao banco de dados.
- **Services**: regras de negócio transversais (ex.: geração de tokens em `TokenService`).
- **Contracts**: **DTOs** de entrada/saída (requests/responses), evitando expor o domínio diretamente.
- **Controllers**: expõem os **endpoints HTTP**, orquestrando serviços e persistência, e retornando respostas padronizadas.
- **Examples**: exemplos de payload para **Swagger**, tornando a documentação mais clara e utilizável.

### Justificativa do Domínio
- **Usuario**: base para **identidade e controle de acesso** aos recursos.  
- **Moto**: ativo principal da frota (placa, ano, modelo).  
- **Manutencao**: controle do **histórico de serviços**, essencial para gestão, segurança e valorização da frota.

---

## Tecnologias e Pacotes Utilizados

- **.NET 8** / ASP.NET Core Web API  
- **Entity Framework Core** com **SQLite** (persistência local)  
- **Swagger / Swashbuckle** (documentação OpenAPI)
  - `Swashbuckle.AspNetCore.Annotations` (metadados)
  - `Swashbuckle.AspNetCore.Filters` (exemplos de payload)
- **Autenticação JWT** (`Microsoft.AspNetCore.Authentication.JwtBearer`)  
- **BCrypt.Net-Next** (hashing seguro de senhas)

---

## Como Executar a API

**Pré-requisito**: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado.

**Passos**:

1) Clone o repositório e acesse a pasta do projeto:
```bash
git clone https://github.com/tcguus/Mottu-API
```

2) Restaure as dependências:
```bash
dotnet restore
```

3) Aplique as migrations para criar o banco:
```bash
dotnet ef database update
```

4) Execute a API:
```bash
dotnet run
```

5) Acesse o Swagger:  
`http://localhost:5075/swagger`

> **Nota:** O banco `mottu.db` será criado na raiz do projeto. Um usuário **admin** é criado automaticamente para testes:  
> **Email:** `admin@mottu.com` — **Senha:** `123456`

---

## Como Testar a API (arquivo `Mottu.Api.http`)

Embora não haja suíte de testes unitários, o projeto inclui o arquivo **`Mottu.Api.http`** com **requisições E2E** para validar rapidamente os endpoints.

**Como usar:**

1) Inicie a API:
```bash
dotnet run
```

2) Use um cliente REST no seu editor:
- **VS Code**: extensão [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)  
- **JetBrains Rider**: suporte nativo

3) Abra o arquivo `Mottu.Api.http` e:
- Envie a requisição **`POST /api/v1/Auth/login`** para autenticar (o token será salvo).  
- Execute as demais requisições: elas usarão o token automaticamente.

---

## Exemplos de Uso dos Endpoints

### Autenticação (Login)

**Request**
```http
POST /api/v1/Auth/login
Content-Type: application/json

{
  "email": "admin@mottu.com",
  "senha": "123456"
}
```

**Response — 200 OK**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "usuario": {
    "id": "GUID...",
    "nome": "Admin Demo",
    "email": "admin@mottu.com"
  }
}
```

> Use o **token JWT** nas próximas chamadas:
>
> **Header**
> ```http
> Authorization: Bearer {seu_token}
> ```

---

### Cadastrar uma nova moto

**Request**
```http
POST /api/v1/Motos
Content-Type: application/json
Authorization: Bearer {seu_token}

{
  "placa": "NEW-9876",
  "ano": 2024,
  "modelo": "Sport"
}
```

**Response — 201 Created**
```json
{
  "placa": "NEW-9876",
  "ano": 2024,
  "modelo": "Sport"
}
```

---

### Listar manutenções com paginação e HATEOAS

**Request**
```http
GET /api/v1/Manutencoes?page=1&pageSize=5
Authorization: Bearer {seu_token}
```

**Response — 200 OK**
```json
{
  "items": [
    {
      "id": "0042",
      "placa": "ABC-1234",
      "problemas": "Troca de óleo",
      "status": "Aberta",
      "data": "2025-09-29T18:30:00Z"
    }
  ],
  "page": 1,
  "pageSize": 5,
  "total": 1,
  "links": [
    { "rel": "self", "href": "http://localhost:5075/api/v1/manutencoes?page=1&pageSize=5", "method": "GET" }
  ]
}
```
