using Microsoft.EntityFrameworkCore;
using HealthcareApi.Domain.Entities;
using HealthcareApi.Domain.Interfaces;
using HealthcareApi.Infrastructure.Data;

namespace HealthcareApi.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
