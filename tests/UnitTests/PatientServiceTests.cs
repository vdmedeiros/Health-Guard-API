using Moq;
using Xunit;
using HealthcareApi.Application.DTOs;
using HealthcareApi.Application.Services;
using HealthcareApi.Domain.Entities;
using HealthcareApi.Domain.Interfaces;

namespace HealthcareApi.Tests;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _patientService = new PatientService(_patientRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientExists_ReturnsPatientDto()
    {
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            Id = patientId,
            Nome = "João Silva",
            Cpf = "529.982.247-25",
            DataNascimento = new DateTime(1990, 5, 15),
            Contato = "(11) 99999-9999"
        };

        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(patient);

        var result = await _patientService.GetByIdAsync(patientId);

        Assert.NotNull(result);
        Assert.Equal(patientId, result.Id);
        Assert.Equal("João Silva", result.Nome);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPatientDoesNotExist_ReturnsNull()
    {
        var patientId = Guid.NewGuid();

        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync((Patient?)null);

        var result = await _patientService.GetByIdAsync(patientId);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesPatient()
    {
        var request = new CreatePatientRequest(
            "Maria Santos",
            "529.982.247-25",
            new DateTime(1985, 10, 20),
            "(11) 98888-8888",
            "maria@email.com",
            "Rua das Flores, 123"
        );

        _patientRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>()))
            .ReturnsAsync((Patient?)null);

        _patientRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Patient>()))
            .ReturnsAsync((Patient p) => p);

        var result = await _patientService.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("Maria Santos", result.Nome);
        _patientRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCpf_ThrowsArgumentException()
    {
        var request = new CreatePatientRequest(
            "Maria Santos",
            "11111111111",
            new DateTime(1985, 10, 20),
            "(11) 98888-8888",
            null,
            null
        );

        await Assert.ThrowsAsync<ArgumentException>(() => _patientService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCpf_ThrowsArgumentException()
    {
        var request = new CreatePatientRequest(
            "Maria Santos",
            "529.982.247-25",
            new DateTime(1985, 10, 20),
            "(11) 98888-8888",
            null,
            null
        );

        _patientRepositoryMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>()))
            .ReturnsAsync(new Patient { Cpf = "529.982.247-25" });

        await Assert.ThrowsAsync<ArgumentException>(() => _patientService.CreateAsync(request));
    }

    [Fact]
    public async Task DeleteAsync_WhenPatientExists_ReturnsTrue()
    {
        var patientId = Guid.NewGuid();
        var patient = new Patient { Id = patientId };

        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(patient);

        _patientRepositoryMock.Setup(r => r.DeleteAsync(patient))
            .Returns(Task.CompletedTask);

        var result = await _patientService.DeleteAsync(patientId);

        Assert.True(result);
        _patientRepositoryMock.Verify(r => r.DeleteAsync(patient), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenPatientDoesNotExist_ReturnsFalse()
    {
        var patientId = Guid.NewGuid();

        _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync((Patient?)null);

        var result = await _patientService.DeleteAsync(patientId);

        Assert.False(result);
    }
}
