using HealthcareApi.Application.DTOs;

namespace HealthcareApi.Application.Services;

public interface IPatientService
{
    Task<PatientDto?> GetByIdAsync(Guid id);
    Task<PatientWithHistoryDto?> GetWithMedicalHistoryAsync(Guid id);
    Task<PatientListResponse> SearchAsync(string? nome, string? cpf, int page = 1, int pageSize = 10);
    Task<PatientDto> CreateAsync(CreatePatientRequest request);
    Task<PatientDto?> UpdateAsync(Guid id, UpdatePatientRequest request);
    Task<bool> DeleteAsync(Guid id);
}
