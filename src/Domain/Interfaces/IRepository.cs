using System.Linq.Expressions;
using HealthcareApi.Domain.Entities;

namespace HealthcareApi.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByCpfAsync(string cpf);
    Task<Patient?> GetWithMedicalHistoryAsync(Guid id);
    Task<IEnumerable<Patient>> SearchAsync(string? nome, string? cpf, int page, int pageSize);
    Task<int> CountAsync(string? nome, string? cpf);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public interface IMedicalHistoryRepository : IRepository<MedicalHistory>
{
    Task<IEnumerable<MedicalHistory>> GetByPatientIdAsync(Guid patientId);
    Task<MedicalHistory?> GetWithDetailsAsync(Guid id);
}
