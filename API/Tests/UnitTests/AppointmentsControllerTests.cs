using Xunit;
using Microsoft.AspNetCore.Mvc;
using HealthcareAPI.Controllers;
using HealthcareAPI.Data;
using Moq;
using HealthcareAPI.Helper;

namespace HealthcareAPI.Tests;

public class AppointmentsControllerTests
{
    private readonly Mock<TestHealthcareDBContext> _mockContext;
    private readonly AppointmentsController _controller;

    public AppointmentsControllerTests()
    {
        _mockContext = new();
        _controller = new(_mockContext.Object);
    }

    [Fact]
    public async Task GetAllAppointments_ValidAppointmentsList_ReturnsOk()
    {
        var appointments = new List<Appointment>
        {
            new() { Title = "Test" },
            new() { Title = "Appointment"},
            new() { Title = "Another"},
        }.AsQueryable();
        var mockContext = new Mock<TestHealthcareDBContext>();
        mockContext.Setup(c => c.Set<Appointment>()).Returns(MockDBSet.MockDbSet(appointments));
        var controller = new AppointmentsController(mockContext.Object);
        var result = await controller.GetAllAppointments() as OkObjectResult;
        Assert.NotNull(result);
        var returnedAppointments = result.Value as List<Appointment>;
        Assert.NotNull(returnedAppointments);
        Assert.Equal(appointments.Count(), returnedAppointments.Count);
    }

    [Fact]
    public async Task GetAllAppointments_EmptyAppointmentsList_ReturnsNotFound()
    {       
        var appointments = new List<Appointment>
        {
        }.AsQueryable();
        var mockContext = new Mock<TestHealthcareDBContext>();
        mockContext.Setup(c => c.Set<Appointment>()).Returns(MockDBSet.MockDbSet(appointments));
        var controller = new AppointmentsController(mockContext.Object);
        var result = await controller.GetAllAppointments() as NotFoundResult;
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }
    
    [Fact]
    public async Task GetAppointment_ValidAppointmentID_ReturnsOk()
    {
        var appointments = new List<Appointment>
        {
            new() { Id = 1, Title = "Testing Appointment" }
        }.AsQueryable();
        var mockContext = new Mock<TestHealthcareDBContext>();
        mockContext.Setup(c => c.Appointments.FindAsync(1)).ReturnsAsync(appointments.First());
        var controller = new AppointmentsController(mockContext.Object);
        var result = await controller.GetAppointment(1) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        var appointment = result.Value as Appointment;
        Assert.NotNull(appointment);
        Assert.Equal("Testing Appointment", appointment.Title);
    }   

    [Fact]
    public async Task GetAppointment_InvalidAppointmentID_ReturnsNotFound()
    {
        var appointments = new List<Appointment>().AsQueryable();
        var mockContext = new Mock<TestHealthcareDBContext>();
        mockContext.Setup(c => c.Appointments.FindAsync(999999)).ReturnsAsync(default(Appointment));
        var controller = new AppointmentsController(mockContext.Object);
        var result = await controller.GetAppointment(999999) as NotFoundResult;
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }
}
