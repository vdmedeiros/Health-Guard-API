namespace HealthcareApi.Domain.Entities;

public class Patient : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Contato { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public bool Ativo { get; set; } = true;
    
    public virtual ICollection<MedicalHistory> HistoricoMedico { get; set; } = new List<MedicalHistory>();
}
