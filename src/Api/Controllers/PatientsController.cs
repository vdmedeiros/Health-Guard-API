using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthcareApi.Application.DTOs;
using HealthcareApi.Application.Services;

namespace HealthcareApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PatientListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? nome,
        [FromQuery] string? cpf,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _patientService.SearchAsync(nome, cpf, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        
        if (patient == null)
            return NotFound(new { message = "Paciente não encontrado." });

        return Ok(patient);
    }

    [HttpGet("{id:guid}/historico")]
    [ProducesResponseType(typeof(PatientWithHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWithMedicalHistory(Guid id)
    {
        var patient = await _patientService.GetWithMedicalHistoryAsync(id);
        
        if (patient == null)
            return NotFound(new { message = "Paciente não encontrado." });

        return Ok(patient);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(new { message = "Nome é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.Cpf))
            return BadRequest(new { message = "CPF é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.Contato))
            return BadRequest(new { message = "Contato é obrigatório." });

        try
        {
            var patient = await _patientService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(new { message = "Nome é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.Contato))
            return BadRequest(new { message = "Contato é obrigatório." });

        var patient = await _patientService.UpdateAsync(id, request);
        
        if (patient == null)
            return NotFound(new { message = "Paciente não encontrado." });

        return Ok(patient);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _patientService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = "Paciente não encontrado." });

        return NoContent();
    }
}
