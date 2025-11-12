# GS_Microservicos_IA

# Integrantes
Amanda Cornelsen - RM97760

Vinicius Shuet - RM98160

# Sistema de Gerenciamento de Promts usados

## 📋 Visão Geral

Este projeto implementa uma API REST para gerenciamento de prompts utilizando **Arquitetura em Camadas**, **Injeção de Dependência** e **Tratamento de Erros**.

## 🏗️ Arquitetura do Projeto

### **Padrão de Arquitetura: Camadas (Layered Architecture)**

```
┌─────────────────────────────────────┐
│           API Layer                 │ ← Controllers, Program.cs
├─────────────────────────────────────┤
│         Business Layer              │ ← Domain Models
├─────────────────────────────────────┤
│        Data Access Layer            │ ← Repository Pattern
├─────────────────────────────────────┤
│         Infrastructure              │ ← MySQL
└─────────────────────────────────────┘
```

### **Estrutura de Pastas**
```
GSModelosIAsln/
├── Domain/                    # Camada de Domínio
│   └── Prompt.cs            # Entidade do domínio
├── Repository/               # Camada de Acesso a Dados
│   ├── IPromptRepository.cs # Interface (Contrato)
│   └── PromptRepository.cs  # Implementação
├── GSModelosIAsln/          # Camada de Apresentação (API)
│   ├── Controllers/
│   │   └── PromptController.cs
│   ├── Program.cs
│   └── appsettings.json
└── 
```

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **ASP.NET Core Web API** - Para criar a API REST
- **MySQL** - Banco de dados principal
- **Dapper** - Micro-ORM para acesso a dados
- **Swagger/OpenAPI** - Documentação da API

## 📦 Passo a Passo para Criar o Projeto

### **Passo 1: Criar a Solution e Projetos**

```bash
# Criar a solution
dotnet new sln -n GSModelosIAsln

# Criar projeto Domain (Class Library)
dotnet new classlib -n Domain
dotnet sln add Domain/Domain.csproj

# Criar projeto Repository (Class Library)
dotnet new classlib -n Repository
dotnet sln add Repository/Repository.csproj

# Criar projeto API (Web API)
dotnet new webapi -n GSModelosIAsln
dotnet sln add GSModelosIAsln/GSModelosIAsln.csproj
```

### **Passo 2: Configurar Dependências entre Projetos**

```bash
# Repository depende de Domain
cd Repository
dotnet add reference ../Domain/Domain.csproj

# API depende de Repository e Domain
cd ../GSModelosIAsln
dotnet add reference ../Repository/Repository.csproj
dotnet add reference ../Domain/Domain.csproj
```

### **Passo 3: Instalar Pacotes NuGet**

```bash
# No projeto Repository
cd Repository
dotnet add package Dapper
dotnet add package MySqlConnector

# No projeto API
cd ../performance-cache
dotnet add package StackExchange.Redis
dotnet add package Newtonsoft.Json
```

### **Passo 4: Criar a Entidade Prompt (Domain Layer)**

**Domain/Prompt.cs**
```csharp
namespace Domain
{
    public class Prompt
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public DateTime Data { get; set; }
    }
}
```

### **Passo 5: Criar Interface do Repository**

**Repository/IVehicleRepository.cs**
```csharp
namespace Service
{
    public interface IPromptService
    {
        Task<IEnumerable<Prompt>> GetAllAsync();
        Task<Prompt> AddAsync(Prompt prompt);
        Task UpdateAsync(int id, Prompt prompt);
        Task DeleteAsync(int id);
    }
}

```

### **Passo 6: Implementar o Repository**

**Repository/VehicleRepository.cs**
```csharp
using Domain;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class PromptService : IPromptService
    {
        private readonly IPromptRepository _promptRepository;

        public PromptService(IPromptRepository promptRepository)
        {
            _promptRepository = promptRepository;
        }

        public async Task<IEnumerable<Prompt>> GetAllAsync()
        {
            var prompts = await _promptRepository.GetAllPromptsAsync();
            return prompts ?? new List<Prompt>();
        }

        public async Task<Prompt> AddAsync(Prompt prompt)
        {
            if (prompt == null)
                throw new ArgumentException("Dados do prompt são obrigatórios");

            if (string.IsNullOrWhiteSpace(prompt.Nome) || string.IsNullOrWhiteSpace(prompt.Descricao))
                throw new ArgumentException("Nome e descrição são campos obrigatórios");

            var newPrompt = await _promptRepository.AddPromptAsync(prompt);
            if (newPrompt == null)
                throw new InvalidOperationException("Erro interno ao criar prompt");

            return newPrompt;
        }

        public async Task UpdateAsync(int id, Prompt prompt)
        {
            if (id <= 0)
                throw new ArgumentException("ID do prompt deve ser maior que zero");

            if (prompt == null)
                throw new ArgumentException("Dados do prompt são obrigatórios");

            if (string.IsNullOrWhiteSpace(prompt.Nome) || string.IsNullOrWhiteSpace(prompt.Descricao))
                throw new ArgumentException("Nome e descrição são campos obrigatórios");

            prompt.Id = id;
            await _promptRepository.UpdatePromptAsync(prompt);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID do prompt deve ser maior que zero");

            await _promptRepository.DeletePromptAsync(id);
        }
    }
}

```

### **Passo 7: Configurar Injeção de Dependência**

**GSModelosIAsln/Program.cs**
```csharp
using Repository;
using Service; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// INJEÇÃO DE DEPENDÊNCIA
builder.Services.AddScoped<IPromptRepository, PromptRepository>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                           ?? "Server=localhost;Database=fiap;User=root;Password=123;Port=3306;";

    return new PromptRepository(connectionString);
});

builder.Services.AddScoped<IPromptService, PromptService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

```

### **Passo 8: Configurar Connection Strings**

**GSModelosIAsln/appsettings.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=fiap;User=root;Password=123;Port=3306;",
    "RedisConnection": "localhost:6379"
  }
}
```

### **Passo 9: Criar Tabela no MySQL**

```sql
CREATE DATABASE fiap;
USE fiap;

CREATE TABLE prompt (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(100) NOT NULL,
    data Date NOT NULL,
);

-- Inserir dados de exemplo
INSERT INTO prompt (nome, descricao) VALUES
('Aprendizado continuo', 'Capacidade de aprendender constantimente é um novo superpoder'),
('Desigualdade e inclusao', 'Sem politicas de acesso e educacao, a tecnologia pode ampliar as desigualdades');
```

## 🚀 Como Executar

### **Pré-requisitos**
- .NET 8.0 SDK
- MySQL Server

## 📚 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/Prompt` | Lista todos os prompts |
| POST | `/api/Prompt` | Cria um novo prompt |
| PUT | `/api/Prompt/{id}` | Atualiza um prompt |
| DELETE | `/api/Prompt/{id}` | Exclui um prompt |

