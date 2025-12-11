using HealthcareApi.Application.DTOs;
using HealthcareApi.Application.Validators;
using HealthcareApi.Domain.Entities;
using HealthcareApi.Domain.Interfaces;

namespace HealthcareApi.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto?> GetByIdAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        return patient == null ? null : MapToDto(patient);
    }

    public async Task<PatientWithHistoryDto?> GetWithMedicalHistoryAsync(Guid id)
    {
        var patient = await _patientRepository.GetWithMedicalHistoryAsync(id);
        if (patient == null)
            return null;

        return new PatientWithHistoryDto(
            patient.Id,
            patient.Nome,
            patient.Cpf,
            patient.DataNascimento,
            patient.Contato,
            patient.Email,
            patient.Endereco,
            patient.Ativo,
            patient.CreatedAt,
            patient.HistoricoMedico.Select(MapToMedicalHistoryDto)
        );
    }

    public async Task<PatientListResponse> SearchAsync(string? nome, string? cpf, int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var patients = await _patientRepository.SearchAsync(nome, cpf, page, pageSize);
        var totalCount = await _patientRepository.CountAsync(nome, cpf);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PatientListResponse(
            patients.Select(MapToDto),
            totalCount,
            page,
            pageSize,
            totalPages
        );
    }

    public async Task<PatientDto> CreateAsync(CreatePatientRequest request)
    {
        var normalizedCpf = CpfValidator.Normalize(request.Cpf);
        
        if (!CpfValidator.IsValid(normalizedCpf))
            throw new ArgumentException("CPF inválido.");

        var existingPatient = await _patientRepository.GetByCpfAsync(normalizedCpf);
        if (existingPatient != null)
            throw new ArgumentException("Já existe um paciente cadastrado com este CPF.");

        var patient = new Patient
        {
            Nome = request.Nome.Trim(),
            Cpf = CpfValidator.Format(normalizedCpf),
            DataNascimento = request.DataNascimento,
            Contato = request.Contato.Trim(),
            Email = request.Email?.Trim(),
            Endereco = request.Endereco?.Trim()
        };

        await _patientRepository.AddAsync(patient);
        return MapToDto(patient);
    }

    public async Task<PatientDto?> UpdateAsync(Guid id, UpdatePatientRequest request)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            return null;

        patient.Nome = request.Nome.Trim();
        patient.DataNascimento = request.DataNascimento;
        patient.Contato = request.Contato.Trim();
        patient.Email = request.Email?.Trim();
        patient.Endereco = request.Endereco?.Trim();
        patient.Ativo = request.Ativo;

        await _patientRepository.UpdateAsync(patient);
        return MapToDto(patient);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
            return false;

        await _patientRepository.DeleteAsync(patient);
        return true;
    }

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto(
            patient.Id,
            patient.Nome,
            patient.Cpf,
            patient.DataNascimento,
            patient.Contato,
            patient.Email,
            patient.Endereco,
            patient.Ativo,
            patient.CreatedAt,
            patient.UpdatedAt
        );
    }

    private static MedicalHistoryDto MapToMedicalHistoryDto(MedicalHistory history)
    {
        return new MedicalHistoryDto(
            history.Id,
            history.PatientId,
            history.DataConsulta,
            history.Observacoes,
            history.CreatedAt,
            history.Diagnosticos.Select(d => new DiagnosisDto(
                d.Id,
                d.CodigoCid,
                d.Descricao,
                d.DataDiagnostico,
                d.Observacoes
            )),
            history.Exames.Select(e => new ExamDto(
                e.Id,
                e.Tipo,
                e.Nome,
                e.DataRealizacao,
                e.Resultado,
                e.Laboratorio,
                e.CodigoExterno
            )),
            history.Prescricoes.Select(p => new PrescriptionDto(
                p.Id,
                p.Medicamento,
                p.Dosagem,
                p.Frequencia,
                p.DataInicio,
                p.DataFim,
                p.Instrucoes
            ))
        );
    }
}
