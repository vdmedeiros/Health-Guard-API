using Microsoft.EntityFrameworkCore;
using HealthcareApi.Domain.Entities;
using HealthcareApi.Domain.Interfaces;
using HealthcareApi.Infrastructure.Data;

namespace HealthcareApi.Infrastructure.Repositories;

public class MedicalHistoryRepository : BaseRepository<MedicalHistory>, IMedicalHistoryRepository
{
    public MedicalHistoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MedicalHistory>> GetByPatientIdAsync(Guid patientId)
    {
        return await _dbSet
            .Where(h => h.PatientId == patientId)
            .Include(h => h.Diagnosticos)
            .Include(h => h.Exames)
            .Include(h => h.Prescricoes)
            .OrderByDescending(h => h.DataConsulta)
            .ToListAsync();
    }

    public async Task<MedicalHistory?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(h => h.Diagnosticos)
            .Include(h => h.Exames)
            .Include(h => h.Prescricoes)
            .FirstOrDefaultAsync(h => h.Id == id);
    }
}
