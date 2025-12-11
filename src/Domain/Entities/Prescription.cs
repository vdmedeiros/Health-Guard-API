namespace HealthcareApi.Domain.Entities;

public class Prescription : BaseEntity
{
    public Guid MedicalHistoryId { get; set; }
    public string Medicamento { get; set; } = string.Empty;
    public string Dosagem { get; set; } = string.Empty;
    public string Frequencia { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Instrucoes { get; set; }
    
    public virtual MedicalHistory MedicalHistory { get; set; } = null!;
}
