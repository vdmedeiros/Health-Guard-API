namespace HealthcareApi.Domain.Entities;

public class MedicalHistory : BaseEntity
{
    public Guid PatientId { get; set; }
    public DateTime DataConsulta { get; set; }
    public string? Observacoes { get; set; }
    
    public virtual Patient Patient { get; set; } = null!;
    public virtual ICollection<Diagnosis> Diagnosticos { get; set; } = new List<Diagnosis>();
    public virtual ICollection<Exam> Exames { get; set; } = new List<Exam>();
    public virtual ICollection<Prescription> Prescricoes { get; set; } = new List<Prescription>();
}
