using Microsoft.EntityFrameworkCore;
using HealthcareApi.Domain.Entities;
using HealthcareApi.Domain.Interfaces;
using HealthcareApi.Infrastructure.Data;

namespace HealthcareApi.Infrastructure.Repositories;

public class PatientRepository : BaseRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetByCpfAsync(string cpf)
    {
        var normalizedCpf = cpf.Replace(".", "").Replace("-", "");
        return await _dbSet.FirstOrDefaultAsync(p => p.Cpf.Replace(".", "").Replace("-", "") == normalizedCpf);
    }

    public async Task<Patient?> GetWithMedicalHistoryAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.HistoricoMedico)
                .ThenInclude(h => h.Diagnosticos)
            .Include(p => p.HistoricoMedico)
                .ThenInclude(h => h.Exames)
            .Include(p => p.HistoricoMedico)
                .ThenInclude(h => h.Prescricoes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Patient>> SearchAsync(string? nome, string? cpf, int page, int pageSize)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(p => p.Nome.ToLower().Contains(nome.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(cpf))
        {
            var normalizedCpf = cpf.Replace(".", "").Replace("-", "");
            query = query.Where(p => p.Cpf.Replace(".", "").Replace("-", "").Contains(normalizedCpf));
        }

        return await query
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(string? nome, string? cpf)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(p => p.Nome.ToLower().Contains(nome.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(cpf))
        {
            var normalizedCpf = cpf.Replace(".", "").Replace("-", "");
            query = query.Where(p => p.Cpf.Replace(".", "").Replace("-", "").Contains(normalizedCpf));
        }

        return await query.CountAsync();
    }
}
