using HealthcareApi.Application.DTOs;

namespace HealthcareApi.Application.Services;

public interface IMedicalHistoryService
{
    Task<MedicalHistoryDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<MedicalHistoryDto>> GetByPatientIdAsync(Guid patientId);
    Task<MedicalHistoryDto> CreateAsync(CreateMedicalHistoryRequest request);
    Task<bool> DeleteAsync(Guid id);
}
