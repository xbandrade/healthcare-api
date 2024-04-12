using System.ComponentModel.DataAnnotations;
using Xunit;

namespace HealthcareAPI.Tests;

[Trait("T", "LoginDTO")]
[Trait("T", "DTO")]
public class LoginDTOTests
{
    [Fact]
    public void LoginDTOUsername_Required()
    {
        var dto = new LoginDTO { Username = null, Password = "password" };
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Username)) && r.ErrorMessage == "Username cannot be empty");
    }

    [Fact]
    public void LoginDTOPassword_Required()
    {
        var dto = new LoginDTO { Username = "username", Password = null };
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Password)) && r.ErrorMessage == "Password cannot be empty");
    }
}

[Trait("T", "StaffDTO")]
[Trait("T", "DTO")]
public class StaffDTOTests
{
    [Fact]
    public void StaffDTOName_Required()
    {
        var dto = new StaffDTO { Username = "username", Password = "password", Name = null, Email = "a@a.com" };
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Name)) && r.ErrorMessage == "Name field cannot be empty");
    }

    [Fact]
    public void StaffDTOEmail_Required()
    {
        var dto = new StaffDTO { Username = "username", Password = "password", Name = "Name", Email = null };
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Email)) && r.ErrorMessage == "Email field cannot be empty");
    }
}

[Trait("T", "PatientDTO")]
[Trait("T", "DTO")]
public class PatientDTOTests
{
    [Fact]
    public void PatientDTOName_Required()
    {
        var dto = new PatientDTO { Name = null, Email = "a@a.com", BirthDate = DateTime.Now };
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Name)) && r.ErrorMessage == "Name field cannot be empty");
    }

    [Fact]
    public void StaffDTOPassword_Required()
    {
        var dto = new PatientDTO { Name = "Name", Email = null, BirthDate = DateTime.Now };
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.Email)) && r.ErrorMessage == "Email field cannot be empty");
    }

    [Fact]
    public void StaffDTOBirthDate_Required()
    {
        var dto = new PatientDTO { Name = "Name", Email = "a@a.com", BirthDate = null };
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(dto.BirthDate)) && r.ErrorMessage == "Birth Date field cannot be empty");
    }
}
