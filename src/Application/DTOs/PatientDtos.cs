namespace HealthcareApi.Application.DTOs;

public record CreatePatientRequest(
    string Nome,
    string Cpf,
    DateTime DataNascimento,
    string Contato,
    string? Email,
    string? Endereco
);

public record UpdatePatientRequest(
    string Nome,
    DateTime DataNascimento,
    string Contato,
    string? Email,
    string? Endereco,
    bool Ativo
);

public record PatientDto(
    Guid Id,
    string Nome,
    string Cpf,
    DateTime DataNascimento,
    string Contato,
    string? Email,
    string? Endereco,
    bool Ativo,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record PatientWithHistoryDto(
    Guid Id,
    string Nome,
    string Cpf,
    DateTime DataNascimento,
    string Contato,
    string? Email,
    string? Endereco,
    bool Ativo,
    DateTime CreatedAt,
    IEnumerable<MedicalHistoryDto> HistoricoMedico
);

public record PatientListResponse(
    IEnumerable<PatientDto> Patients,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
