namespace HealthcareApi.Domain.Entities;

public class Diagnosis : BaseEntity
{
    public Guid MedicalHistoryId { get; set; }
    public string CodigoCid { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataDiagnostico { get; set; }
    public string? Observacoes { get; set; }
    
    public virtual MedicalHistory MedicalHistory { get; set; } = null!;
}
