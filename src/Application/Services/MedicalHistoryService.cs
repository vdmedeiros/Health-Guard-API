using HealthcareApi.Application.DTOs;
using HealthcareApi.Domain.Entities;
using HealthcareApi.Domain.Interfaces;

namespace HealthcareApi.Application.Services;

public class MedicalHistoryService : IMedicalHistoryService
{
    private readonly IMedicalHistoryRepository _medicalHistoryRepository;
    private readonly IPatientRepository _patientRepository;

    public MedicalHistoryService(
        IMedicalHistoryRepository medicalHistoryRepository,
        IPatientRepository patientRepository)
    {
        _medicalHistoryRepository = medicalHistoryRepository;
        _patientRepository = patientRepository;
    }

    public async Task<MedicalHistoryDto?> GetByIdAsync(Guid id)
    {
        var history = await _medicalHistoryRepository.GetWithDetailsAsync(id);
        return history == null ? null : MapToDto(history);
    }

    public async Task<IEnumerable<MedicalHistoryDto>> GetByPatientIdAsync(Guid patientId)
    {
        var histories = await _medicalHistoryRepository.GetByPatientIdAsync(patientId);
        return histories.Select(MapToDto);
    }

    public async Task<MedicalHistoryDto> CreateAsync(CreateMedicalHistoryRequest request)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
            throw new ArgumentException("Paciente não encontrado.");

        var history = new MedicalHistory
        {
            PatientId = request.PatientId,
            DataConsulta = request.DataConsulta,
            Observacoes = request.Observacoes?.Trim()
        };

        if (request.Diagnosticos != null)
        {
            foreach (var diag in request.Diagnosticos)
            {
                history.Diagnosticos.Add(new Diagnosis
                {
                    CodigoCid = diag.CodigoCid.Trim().ToUpper(),
                    Descricao = diag.Descricao.Trim(),
                    DataDiagnostico = diag.DataDiagnostico,
                    Observacoes = diag.Observacoes?.Trim()
                });
            }
        }

        if (request.Exames != null)
        {
            foreach (var exam in request.Exames)
            {
                history.Exames.Add(new Exam
                {
                    Tipo = exam.Tipo.Trim(),
                    Nome = exam.Nome.Trim(),
                    DataRealizacao = exam.DataRealizacao,
                    Resultado = exam.Resultado?.Trim(),
                    Laboratorio = exam.Laboratorio?.Trim(),
                    CodigoExterno = exam.CodigoExterno?.Trim()
                });
            }
        }

        if (request.Prescricoes != null)
        {
            foreach (var presc in request.Prescricoes)
            {
                history.Prescricoes.Add(new Prescription
                {
                    Medicamento = presc.Medicamento.Trim(),
                    Dosagem = presc.Dosagem.Trim(),
                    Frequencia = presc.Frequencia.Trim(),
                    DataInicio = presc.DataInicio,
                    DataFim = presc.DataFim,
                    Instrucoes = presc.Instrucoes?.Trim()
                });
            }
        }

        await _medicalHistoryRepository.AddAsync(history);
        
        var savedHistory = await _medicalHistoryRepository.GetWithDetailsAsync(history.Id);
        return MapToDto(savedHistory!);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var history = await _medicalHistoryRepository.GetByIdAsync(id);
        if (history == null)
            return false;

        await _medicalHistoryRepository.DeleteAsync(history);
        return true;
    }

    private static MedicalHistoryDto MapToDto(MedicalHistory history)
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
