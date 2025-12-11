using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HealthcareApi.Application.DTOs;
using HealthcareApi.Infrastructure.Services;

namespace HealthcareApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ExternalExamsController : ControllerBase
{
    private readonly IExternalExamService _externalExamService;

    public ExternalExamsController(IExternalExamService externalExamService)
    {
        _externalExamService = externalExamService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ExternalExamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] string cpf,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return BadRequest(new { message = "CPF é obrigatório para consulta de exames externos." });

        var request = new ExternalExamSearchRequest(cpf, dataInicio, dataFim);
        var result = await _externalExamService.SearchExamsAsync(request);
        
        return Ok(result);
    }
}
