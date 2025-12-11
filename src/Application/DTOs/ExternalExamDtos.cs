namespace HealthcareApi.Application.DTOs;

public record ExternalExamSearchRequest(string Cpf, DateTime? DataInicio, DateTime? DataFim);

public record ExternalExamDto(
    string CodigoExame,
    string Nome,
    string Tipo,
    DateTime DataRealizacao,
    string Laboratorio,
    string Status,
    string? Resultado
);

public record ExternalExamResponse(
    bool Sucesso,
    string Mensagem,
    IEnumerable<ExternalExamDto> Exames
);
