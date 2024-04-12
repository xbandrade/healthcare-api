using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using HealthcareAPI.Controllers;
using HealthcareAPI.Data;
using HealthcareAPI.Helper;

namespace HealthcareAPI.Tests;

[Trait("T", "Patients")]
public class PatientControllerTests
{
    private readonly Mock<TestHealthcareDBContext> _mockContext;
    private readonly PatientsController _controller;

    public PatientControllerTests()
    {
        _mockContext = new();
        _controller = new(_mockContext.Object);
    }

    [Fact]
    public async Task GetAllPatients_ValidPatientsList_ReturnsOk()
    {
        var patients = new List<Patient>
        {
            new() { Name = "Test" },
            new() { Name = "Test2" },
            new() { Name = "Test3" },
        }.AsQueryable();
        var mockContext = new Mock<TestHealthcareDBContext>();
        mockContext.Setup(c => c.Set<Patient>()).Returns(MockDBSet.MockDbSet(patients));
        var controller = new PatientsController(mockContext.Object);
        var result = await controller.GetAllPatients() as OkObjectResult;
        Assert.NotNull(result);
        var returnedPatients = result.Value as List<Patient>;
        Assert.NotNull(returnedPatients);
        Assert.Equal(patients.Count(), returnedPatients.Count);
    }

    [Fact]
    public async Task GetAllPatients_EmptyPatientsList_ReturnsNotFound()
    {       
        var patients = new List<Patient>
        {
        }.AsQueryable();
        var mockContext = new Mock<TestHealthcareDBContext>();
        mockContext.Setup(c => c.Set<Patient>()).Returns(MockDBSet.MockDbSet(patients));
        var controller = new PatientsController(mockContext.Object);
        var result = await controller.GetAllPatients() as NotFoundResult;
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task CreatePatient_ValidPatient_ReturnsCreatedAtRoute()
    {
        var patient = new PatientDTO { Name = "Pat", Email = "pat@a.com", BirthDate = DateTime.MinValue };
        _mockContext.Setup(x => x.Users.Add(It.IsAny<Patient>()));
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
        var result = await _controller.CreatePatient(patient) as CreatedAtRouteResult;
        Assert.NotNull(result);
        Assert.Equal("GetPatient", result.RouteName);
        Assert.Equal(201, result.StatusCode);
    }

    [Fact]
    public async Task CreatePatient_InvalidPatient_ReturnsBadRequest()
    {
        var patient = new PatientDTO { Name = "Pat", Email = "pat@a.com" };
        _mockContext.Setup(x => x.Users.Add(It.IsAny<Patient>()));
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(0);
        var result = await _controller.CreatePatient(patient) as BadRequestObjectResult;
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("BirthDate", result.Value?.ToString());
    }
}
