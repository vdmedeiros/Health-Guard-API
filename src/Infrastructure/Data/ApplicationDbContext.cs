using Microsoft.EntityFrameworkCore;
using HealthcareApi.Domain.Entities;

namespace HealthcareApi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<MedicalHistory> MedicalHistories => Set<MedicalHistory>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Cpf).IsUnique();
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cpf).IsRequired().HasMaxLength(14);
            entity.Property(e => e.Contato).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Endereco).HasMaxLength(500);
            
            entity.HasMany(e => e.HistoricoMedico)
                .WithOne(h => h.Patient)
                .HasForeignKey(h => h.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.DataConsulta);
            entity.Property(e => e.Observacoes).HasMaxLength(2000);
            
            entity.HasMany(e => e.Diagnosticos)
                .WithOne(d => d.MedicalHistory)
                .HasForeignKey(d => d.MedicalHistoryId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.Exames)
                .WithOne(e => e.MedicalHistory)
                .HasForeignKey(e => e.MedicalHistoryId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasMany(e => e.Prescricoes)
                .WithOne(p => p.MedicalHistory)
                .HasForeignKey(p => p.MedicalHistoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MedicalHistoryId);
            entity.HasIndex(e => e.CodigoCid);
            entity.Property(e => e.CodigoCid).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Observacoes).HasMaxLength(1000);
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MedicalHistoryId);
            entity.HasIndex(e => e.CodigoExterno);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Resultado).HasMaxLength(2000);
            entity.Property(e => e.Laboratorio).HasMaxLength(200);
            entity.Property(e => e.CodigoExterno).HasMaxLength(50);
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.MedicalHistoryId);
            entity.Property(e => e.Medicamento).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Dosagem).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Frequencia).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Instrucoes).HasMaxLength(1000);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
