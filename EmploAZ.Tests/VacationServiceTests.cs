using EmploAZ.Interfaces;
using EmploAZ.Models;
using EmploAZ.Services;

namespace EmploAZ.Tests;

public class VacationServiceTests
{
    private readonly IVacationService _vacationService;

    public VacationServiceTests()
    {
        _vacationService = new VacationService();
    }

    [Fact]
    public void CountFreeDaysForEmployee_WithValidData_ReturnsCorrectDays()
    {
        // Arrange
        var employee = new Employee { Id = 1, Name = "Jan Kowalski" };
        var vacationPackage = new VacationPackage { GrantedDays = 26, Year = DateTime.Now.Year };
        var vacations = new List<Vacation>
        {
            new Vacation
            {
                EmployeeId = 1,
                NumberOfHours = 40, // 5 dni
                DateSince = DateTime.Now,
                DateUntil = DateTime.Now.AddDays(5)
            }
        };

        // Act
        var freeDays = _vacationService.CountFreeDaysForEmployee(employee, vacations, vacationPackage);

        // Assert
        Assert.Equal(21, freeDays);
    }

    [Fact]
    public void CountFreeDaysForEmployee_WithNullVacationPackage_ReturnsZero()
    {
        // Arrange
        var employee = new Employee { Id = 1, Name = "Jan Kowalski" };
        var vacations = new List<Vacation>();

        // Act
        var freeDays = _vacationService.CountFreeDaysForEmployee(employee, vacations, null!);

        // Assert
        Assert.Equal(0, freeDays);
    }

    [Fact]
    public void CountFreeDaysForEmployee_WithNullEmployee_ThrowsArgumentNullException()
    {
        // Arrange
        var vacations = new List<Vacation>();
        var vacationPackage = new VacationPackage { GrantedDays = 26, Year = DateTime.Now.Year };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _vacationService.CountFreeDaysForEmployee(null!, vacations, vacationPackage));
    }

    [Fact]
    public void IfEmployeeCanRequestVacation_WithFreeDays_ReturnsTrue()
    {
        // Arrange
        var employee = new Employee { Id = 1, Name = "Jan Kowalski" };
        var vacationPackage = new VacationPackage { GrantedDays = 26, Year = DateTime.Now.Year };
        var vacations = new List<Vacation>();

        // Act
        var canRequest = _vacationService.IfEmployeeCanRequestVacation(employee, vacations, vacationPackage);

        // Assert
        Assert.True(canRequest);
    }

    [Fact]
    public void IfEmployeeCanRequestVacation_WithNoFreeDays_ReturnsFalse()
    {
        // Arrange
        var employee = new Employee { Id = 1, Name = "Jan Kowalski" };
        var vacationPackage = new VacationPackage { GrantedDays = 5, Year = DateTime.Now.Year };
        var vacations = new List<Vacation>
        {
            new Vacation
            {
                EmployeeId = 1,
                NumberOfHours = 40,
                DateSince = DateTime.Now,
                DateUntil = DateTime.Now.AddDays(5)
            }
        };

        // Act
        var canRequest = _vacationService.IfEmployeeCanRequestVacation(employee, vacations, vacationPackage);

        // Assert
        Assert.False(canRequest);
    }
}