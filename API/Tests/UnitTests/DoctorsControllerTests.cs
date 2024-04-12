using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using HealthcareAPI.Controllers;
using HealthcareAPI.Data;
using HealthcareAPI.Helper;
using Microsoft.EntityFrameworkCore;

namespace HealthcareAPI.Tests;

[Trait("T", "Doctors")]
public class DoctorsControllerTests
{
    private readonly Mock<TestHealthcareDBContext> _mockContext;
    private readonly DoctorsController _controller;

    public DoctorsControllerTests()
    {
        _mockContext = new();
        _controller = new(_mockContext.Object);
    }

    [Fact]
    public async Task GetAllDoctors_ValidDoctorsList_ReturnsOk()
    {
        var doctors = new List<Doctor>
        {
            new() { Name = "Test" },
            new() { Name = "Test2" },
            new() { Name = "Test3" },
        }.AsQueryable();
        var mockContext = new Mock<TestHealthcareDBContext>();
        mockContext.Setup(c => c.Set<Doctor>()).Returns(MockDBSet.MockDbSet(doctors));
        var controller = new DoctorsController(mockContext.Object);
        var result = await controller.GetAllDoctors() as OkObjectResult;
        Assert.NotNull(result);
        var returnedDoctors = result.Value as List<Doctor>;
        Assert.NotNull(returnedDoctors);
        Assert.Equal(doctors.Count(), returnedDoctors.Count);
    }

    [Fact]
    public async Task GetAllDoctors_EmptyDoctorsList_ReturnsNotFound()
    {       
        var doctors = new List<Doctor>
        {
        }.AsQueryable();
        var mockContext = new Mock<TestHealthcareDBContext>();
        mockContext.Setup(c => c.Set<Doctor>()).Returns(MockDBSet.MockDbSet(doctors));
        var controller = new DoctorsController(mockContext.Object);
        var result = await controller.GetAllDoctors() as NotFoundResult;
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task CreateDoctor_ValidDoctorModel_ReturnsCreatedAtRoute()
    {
        var doctor = new StaffDTO { Name = "Doc", Password = "doc123", Username = "doc", Email = "doc@a.com" };
        _mockContext.Setup(x => x.Users.Add(It.IsAny<Doctor>()));
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
        var result = await _controller.CreateDoctor(doctor) as CreatedAtRouteResult;
        Assert.NotNull(result);
        Assert.Equal("GetDoctor", result.RouteName);
        Assert.Equal(201, result.StatusCode);
    }

    [Fact]
    public async Task CreateDoctor_InvalidDoctorModel_ReturnsBadRequest()
    {
        var doctor = new StaffDTO { Name = "Doc", Password = "doc123", Email = "doc@a.com" };
        _mockContext.Setup(x => x.Users.Add(It.IsAny<Doctor>()));
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(0);
        var result = await _controller.CreateDoctor(doctor) as BadRequestObjectResult;
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("Username", result.Value?.ToString());
    }
}
