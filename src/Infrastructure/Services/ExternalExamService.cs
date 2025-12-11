using HealthcareApi.Application.DTOs;

namespace HealthcareApi.Infrastructure.Services;

public interface IExternalExamService
{
    Task<ExternalExamResponse> SearchExamsAsync(ExternalExamSearchRequest request);
}

public class ExternalExamService : IExternalExamService
{
    private readonly ILogger<ExternalExamService> _logger;

    public ExternalExamService(ILogger<ExternalExamService> logger)
    {
        _logger = logger;
    }

    public async Task<ExternalExamResponse> SearchExamsAsync(ExternalExamSearchRequest request)
    {
        _logger.LogInformation("Buscando exames externos para CPF: {Cpf}", request.Cpf);

        await Task.Delay(500);

        var mockExams = GenerateMockExams(request);

        return new ExternalExamResponse(
            Sucesso: true,
            Mensagem: "Consulta realizada com sucesso (dados mockados)",
            Exames: mockExams
        );
    }

    private static IEnumerable<ExternalExamDto> GenerateMockExams(ExternalExamSearchRequest request)
    {
        var random = new Random(request.Cpf.GetHashCode());
        var examTypes = new[]
        {
            ("Hemograma Completo", "Laboratorial"),
            ("Glicemia em Jejum", "Laboratorial"),
            ("Colesterol Total", "Laboratorial"),
            ("Raio-X Tórax", "Imagem"),
            ("Eletrocardiograma", "Cardiológico"),
            ("Ultrassonografia Abdominal", "Imagem"),
            ("TSH", "Laboratorial"),
            ("Creatinina", "Laboratorial"),
            ("Ureia", "Laboratorial"),
            ("TGO/TGP", "Laboratorial")
        };

        var labs = new[] { "Lab São Paulo", "Diagnósticos Brasil", "Lab Central", "Exames Plus", "Lab Vida" };
        var statuses = new[] { "Concluído", "Concluído", "Concluído", "Em Análise", "Pendente" };

        var startDate = request.DataInicio ?? DateTime.UtcNow.AddMonths(-6);
        var endDate = request.DataFim ?? DateTime.UtcNow;

        var examsCount = random.Next(2, 8);
        var exams = new List<ExternalExamDto>();

        for (int i = 0; i < examsCount; i++)
        {
            var examType = examTypes[random.Next(examTypes.Length)];
            var status = statuses[random.Next(statuses.Length)];
            var daysOffset = random.Next((int)(endDate - startDate).TotalDays);
            var examDate = startDate.AddDays(daysOffset);

            exams.Add(new ExternalExamDto(
                CodigoExame: $"EXT-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                Nome: examType.Item1,
                Tipo: examType.Item2,
                DataRealizacao: examDate,
                Laboratorio: labs[random.Next(labs.Length)],
                Status: status,
                Resultado: status == "Concluído" ? GenerateMockResult(examType.Item1, random) : null
            ));
        }

        return exams.OrderByDescending(e => e.DataRealizacao);
    }

    private static string GenerateMockResult(string examName, Random random)
    {
        return examName switch
        {
            "Hemograma Completo" => $"Hemoglobina: {12 + random.NextDouble() * 4:F1} g/dL; Leucócitos: {4000 + random.Next(6000)}/mm³; Plaquetas: {150000 + random.Next(250000)}/mm³",
            "Glicemia em Jejum" => $"{70 + random.Next(50)} mg/dL - {(random.Next(100) < 80 ? "Normal" : "Alterado")}",
            "Colesterol Total" => $"{150 + random.Next(100)} mg/dL - {(random.Next(100) < 70 ? "Desejável" : "Limítrofe")}",
            "TSH" => $"{0.5 + random.NextDouble() * 4:F2} mUI/L - Normal",
            "Creatinina" => $"{0.6 + random.NextDouble() * 0.8:F2} mg/dL - Normal",
            _ => "Resultado dentro dos parâmetros normais"
        };
    }
}
