namespace HealthcareApi.Domain.Entities;

public class Exam : BaseEntity
{
    public Guid MedicalHistoryId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateTime DataRealizacao { get; set; }
    public string? Resultado { get; set; }
    public string? Laboratorio { get; set; }
    public string? CodigoExterno { get; set; }
    
    public virtual MedicalHistory MedicalHistory { get; set; } = null!;
}
