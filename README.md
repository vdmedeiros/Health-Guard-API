# Healthcare API

API REST desenvolvida em .NET 8 para gestão de pacientes e histórico médico, com autenticação JWT e integração com serviços externos.

## Índice

- [Visão Geral](#visão-geral)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Arquitetura](#arquitetura)
- [Decisões Técnicas](#decisões-técnicas)
- [Segurança](#segurança)
- [Escalabilidade](#escalabilidade)
- [Modelagem de Dados](#modelagem-de-dados)
- [Endpoints da API](#endpoints-da-api)
- [Como Executar](#como-executar)
- [Testes](#testes)

## Visão Geral

Esta API fornece funcionalidades completas para:

- **CRUD de Pacientes**: Gerenciamento completo de pacientes com validação de CPF
- **Histórico Médico**: Registro de consultas, diagnósticos, exames e prescrições
- **Autenticação JWT**: Sistema seguro de autenticação e autorização
- **Consulta de Exames Externos**: Endpoint mockado simulando integração com serviços

## Tecnologias Utilizadas

| Tecnologia | Versão | Propósito |
|------------|--------|-----------|
| .NET | 8.0 | Framework principal |
| ASP.NET Core | 8.0 | Web API |
| Entity Framework Core | 8.0.4 | ORM |
| Sql Server | 16+ | Banco de dados |
| JWT Bearer | 8.0.4 | Autenticação |
| BCrypt.Net | 4.0.3 | Hash de senhas |
| Swashbuckle | 6.5.0 | Documentação Swagger |
| xUnit | 2.5.3 | Testes unitários |
| Moq | 4.20.70 | Mocking para testes |

## Arquitetura

O projeto segue os princípios de **Clean Architecture** e **SOLID**:

```
├── src/
│   ├── Domain/
│   │   ├── Entities/          # Entidades do domínio
│   │   └── Interfaces/        # Contratos (interfaces)
│   ├── Application/
│   │   ├── DTOs/              # Data Transfer Objects
│   │   ├── Services/          # Lógica de negócio
│   │   └── Validators/        # Validações (ex: CPF)
│   ├── Infrastructure/
│   │   ├── Data/              # DbContext e configurações
│   │   ├── Repositories/      # Implementação dos repositórios
│   │   └── Services/          # Serviços externos
│   └── Api/
│       └── Controllers/       # Endpoints da API
├── tests/
│   ├── UnitTests/             # Testes unitários
│   └── IntegrationTests/      # Testes de integração
└── Program.cs                 # Configuração da aplicação
```

## Decisões Técnicas

### 1. SQL Server

### 2. Repository Pattern

Implementamos o padrão Repository para:
- Abstrair a camada de dados
- Facilitar testes unitários com mocks
- Permitir troca de ORM sem impactar a lógica de negócio

### 3. DTOs (Data Transfer Objects)

Utilizamos records do C# para DTOs, garantindo:
- Imutabilidade dos dados
- Menor uso memória
- Separação clara entre domínio e API

### 4. Validação de CPF

Implementação própria de validação de CPF seguindo o algoritmo oficial, incluindo:
- Verificação de dígitos verificadores
- Rejeição de CPFs com todos os dígitos iguais
- Normalização automática (remoção de pontuação)

## Segurança

### Autenticação JWT

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Cliente   │────▶│  Login/     │────▶│   Gera      │
│             │     │  Register   │     │   JWT       │
└─────────────┘     └─────────────┘     └─────────────┘
                           │
                           ▼
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   Request   │────▶│  Valida     │────▶│  Acessa     │
│  + Bearer   │     │   Token     │     │  Recursos   │
└─────────────┘     └─────────────┘     └─────────────┘
```

**Características:**
- Tokens com expiração configurável (padrão: 24 horas)
- Claims incluem: ID, Email, Nome e Role
- Senhas hasheadas com BCrypt (custo: 11)
- Chave simétrica (HMAC-SHA256) para assinatura

### Boas Práticas Implementadas

1. **Hash de senhas**: BCrypt com salt automático
2. **Tokens de curta duração**: Expiração configurável
3. **Validação de entrada**: Todas as requisições são validadas
4. **CORS configurado**: Permite controle de origens
5. **Endpoints protegidos**: Todos os recursos requerem autenticação (exceto login/register)

## Escalabilidade

### Estratégias para Grandes Volumes

#### 1. Paginação

Todas as listagens suportam paginação:

```csharp
GET /api/patients?page=1&pageSize=10&nome=João
```

#### 2. Índices no Banco de Dados

Índices criados automaticamente:
- `IX_Patients_Cpf` (UNIQUE) - Busca por CPF
- `IX_MedicalHistories_PatientId` - Histórico por paciente
- `IX_MedicalHistories_DataConsulta` - Ordenação por data
- `IX_Diagnoses_CodigoCid` - Busca por CID

#### 3. Lazy Loading Controlado

O Entity Framework está configurado com:
- Lazy loading desabilitado por padrão
- Eager loading explícito quando necessário (Include)
- Projeções com Select para evitar over-fetching (dados desnecessários)

#### 4. Consultas Otimizadas

```csharp
// Exemplo de consulta otimizada com paginação
public async Task<IEnumerable<Patient>> SearchAsync(
    string? nome, string? cpf, int page, int pageSize)
{
    return await _dbSet
        .Where(p => nome == null || p.Nome.Contains(nome))
        .OrderBy(p => p.Nome)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
```

### Melhorias Futuras para Escala

1. **Cache distribuído** (Redis) para consultas frequentes
2. **Read replicas** para separar leitura/escrita
3. **Message Queue** para operações assíncronas
4. **API Gateway** para rate limiting e load balancing

## Modelagem de Dados

### Diagrama ER

```
┌──────────────────┐
│      User        │
├──────────────────┤
│ Id (PK)          │
│ Email (UNIQUE)   │
│ PasswordHash     │
│ Name             │
│ Role             │
│ IsActive         │
│ CreatedAt        │
└──────────────────┘

┌──────────────────┐       ┌──────────────────┐
│     Patient      │       │  MedicalHistory  │
├──────────────────┤       ├──────────────────┤
│ Id (PK)          │──────▶│ Id (PK)          │
│ Nome             │       │ PatientId (FK)   │
│ Cpf (UNIQUE)     │       │ DataConsulta     │
│ DataNascimento   │       │ Observacoes      │
│ Contato          │       │ CreatedAt        │
│ Email            │       └────────┬─────────┘
│ Endereco         │                │
│ Ativo            │                │
│ CreatedAt        │    ┌───────────┼───────────┐
└──────────────────┘    ▼           ▼           ▼
              ┌─────────────┐ ┌───────────┐ ┌─────────────┐
              │  Diagnosis  │ │   Exam    │ │Prescription │
              ├─────────────┤ ├───────────┤ ├─────────────┤
              │ Id (PK)     │ │ Id (PK)   │ │ Id (PK)     │
              │ MedHistId   │ │ MedHistId │ │ MedHistId   │
              │ CodigoCid   │ │ Tipo      │ │ Medicamento │
              │ Descricao   │ │ Nome      │ │ Dosagem     │
              │ DataDiag    │ │ DataReal  │ │ Frequencia  │
              │ Observacoes │ │ Resultado │ │ DataInicio  │
              └─────────────┘ │ Laborat   │ │ DataFim     │
                              └───────────┘ │ Instrucoes  │
                                            └─────────────┘
```

### Relacionamentos

- **Patient → MedicalHistory**: 1:N (Um paciente tem muitos históricos)
- **MedicalHistory → Diagnosis**: 1:N (Um histórico tem muitos diagnósticos)
- **MedicalHistory → Exam**: 1:N (Um histórico tem muitos exames)
- **MedicalHistory → Prescription**: 1:N (Um histórico tem muitas prescrições)

## Endpoints da API

### Autenticação

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/auth/register` | Registrar novo usuário |
| POST | `/api/auth/login` | Autenticar e obter token |

### Pacientes

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/patients` | Listar pacientes (com paginação) |
| GET | `/api/patients/{id}` | Obter paciente por ID |
| GET | `/api/patients/{id}/historico` | Obter paciente com histórico completo |
| POST | `/api/patients` | Criar novo paciente |
| PUT | `/api/patients/{id}` | Atualizar paciente |
| DELETE | `/api/patients/{id}` | Remover paciente |

### Histórico Médico

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/medicalhistory/{id}` | Obter histórico por ID |
| GET | `/api/medicalhistory/patient/{patientId}` | Listar históricos do paciente |
| POST | `/api/medicalhistory` | Criar novo histórico |
| DELETE | `/api/medicalhistory/{id}` | Remover histórico |

### Exames Externos

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/externalexams` | Consultar exames externos (mockado) |

## Como Executar

### Pré-requisitos

- .NET 8 SDK
- Sql Server 2019+

### Configuração

1. Configure a connection string do Sql Server no `appsettings.json`:
   ```json
   "ConnectionStrings": {
	   "DefaultConnection": "Server=seu_servidor;Database=HealthcareDb;User Id=seu_usuario;Password=sua_senha;"
   }
   ```

2. (Opcional) Configure um secret JWT:
   ```
   JWT_SECRET=sua_chave_secreta_aqui
   ```

3. Execute a aplicação:
   ```bash
   dotnet run
   ```

4. Acesse a documentação Swagger em: `http://localhost:5000`

### Usando Docker (Opcional)

```bash
docker-compose up -d
```

## Testes

### Executar Testes Unitários

```bash
cd tests/UnitTests
dotnet test
```

### Executar Testes de Integração

```bash
cd tests/IntegrationTests
dotnet test
```

### Cobertura de Testes

Os testes cobrem:

- **CpfValidator**: Validação de CPF (casos válidos e inválidos)
- **PatientService**: CRUD de pacientes, validações de negócio
- **AuthController**: Registro, login e proteção de endpoints

## Postman Collection

O arquivo `postman_collection.json` contém todas as requisições organizadas:

1. Importe no Postman
2. Execute "Register" ou "Login" primeiro
3. O token será salvo automaticamente para as demais requisições