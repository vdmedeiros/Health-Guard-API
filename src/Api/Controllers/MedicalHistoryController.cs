using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthcareApi.Application.DTOs;
using HealthcareApi.Application.Services;

namespace HealthcareApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class MedicalHistoryController : ControllerBase
{
    private readonly IMedicalHistoryService _medicalHistoryService;

    public MedicalHistoryController(IMedicalHistoryService medicalHistoryService)
    {
        _medicalHistoryService = medicalHistoryService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MedicalHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var history = await _medicalHistoryService.GetByIdAsync(id);
        
        if (history == null)
            return NotFound(new { message = "Histórico médico não encontrado." });

        return Ok(history);
    }

    [HttpGet("patient/{patientId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<MedicalHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPatientId(Guid patientId)
    {
        var histories = await _medicalHistoryService.GetByPatientIdAsync(patientId);
        return Ok(histories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(MedicalHistoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMedicalHistoryRequest request)
    {
        if (request.PatientId == Guid.Empty)
            return BadRequest(new { message = "ID do paciente é obrigatório." });

        try
        {
            var history = await _medicalHistoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = history.Id }, history);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _medicalHistoryService.DeleteAsync(id);
        
        if (!result)
            return NotFound(new { message = "Histórico médico não encontrado." });

        return NoContent();
    }
}
