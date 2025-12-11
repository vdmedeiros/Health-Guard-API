namespace HealthcareApi.Application.DTOs;

public record CreateMedicalHistoryRequest(
    Guid PatientId,
    DateTime DataConsulta,
    string? Observacoes,
    IEnumerable<CreateDiagnosisRequest>? Diagnosticos,
    IEnumerable<CreateExamRequest>? Exames,
    IEnumerable<CreatePrescriptionRequest>? Prescricoes
);

public record MedicalHistoryDto(
    Guid Id,
    Guid PatientId,
    DateTime DataConsulta,
    string? Observacoes,
    DateTime CreatedAt,
    IEnumerable<DiagnosisDto> Diagnosticos,
    IEnumerable<ExamDto> Exames,
    IEnumerable<PrescriptionDto> Prescricoes
);

public record CreateDiagnosisRequest(
    string CodigoCid,
    string Descricao,
    DateTime DataDiagnostico,
    string? Observacoes
);

public record DiagnosisDto(
    Guid Id,
    string CodigoCid,
    string Descricao,
    DateTime DataDiagnostico,
    string? Observacoes
);

public record CreateExamRequest(
    string Tipo,
    string Nome,
    DateTime DataRealizacao,
    string? Resultado,
    string? Laboratorio,
    string? CodigoExterno
);

public record ExamDto(
    Guid Id,
    string Tipo,
    string Nome,
    DateTime DataRealizacao,
    string? Resultado,
    string? Laboratorio,
    string? CodigoExterno
);

public record CreatePrescriptionRequest(
    string Medicamento,
    string Dosagem,
    string Frequencia,
    DateTime DataInicio,
    DateTime? DataFim,
    string? Instrucoes
);

public record PrescriptionDto(
    Guid Id,
    string Medicamento,
    string Dosagem,
    string Frequencia,
    DateTime DataInicio,
    DateTime? DataFim,
    string? Instrucoes
);
