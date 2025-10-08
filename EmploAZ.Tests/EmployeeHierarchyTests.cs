using EmploAZ.Interfaces;
using EmploAZ.Models;
using EmploAZ.Services;

namespace EmploAZ.Tests;

public class EmployeeHierarchyTests
{
    private readonly IEmployeesHierarchyService _hierarchyService;

    public EmployeeHierarchyTests()
    {
        _hierarchyService = new EmployeesHierarchy();
    }

    [Fact]
    public void FillEmployeesStructure_WithValidEmployees_ReturnsCorrectStructure()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "Jan Kowalski", SuperiorId = null },
            new Employee { Id = 2, Name = "Kamil Nowak", SuperiorId = 1 },
            new Employee { Id = 3, Name = "Anna Mariacka", SuperiorId = 1 },
            new Employee { Id = 4, Name = "Andrzej Abacki", SuperiorId = 2 }
        };

        // Act
        var structure = _hierarchyService.FillEmployeesStructure(employees);

        // Assert
        Assert.Equal(4, structure.Count);

        // Sprawdź konkretne relacje
        Assert.Contains(structure, s => s.EmployeeId == 2 && s.SuperiorId == 1 && s.Distance == 1);
        Assert.Contains(structure, s => s.EmployeeId == 3 && s.SuperiorId == 1 && s.Distance == 1);
        Assert.Contains(structure, s => s.EmployeeId == 4 && s.SuperiorId == 2 && s.Distance == 1);
        Assert.Contains(structure, s => s.EmployeeId == 4 && s.SuperiorId == 1 && s.Distance == 2);
    }

    [Fact]
    public void GetSuperiorRowOfEmployee_WithValidRelation_ReturnsCorrectDistance()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "Jan Kowalski", SuperiorId = null },
            new Employee { Id = 2, Name = "Kamil Nowak", SuperiorId = 1 },
            new Employee { Id = 4, Name = "Andrzej Abacki", SuperiorId = 2 }
        };
        _hierarchyService.FillEmployeesStructure(employees);

        // Act & Assert
        Assert.Equal(1, _hierarchyService.GetSuperiorRowOfEmployee(2, 1));
        Assert.Equal(2, _hierarchyService.GetSuperiorRowOfEmployee(4, 1));
        Assert.Equal(1, _hierarchyService.GetSuperiorRowOfEmployee(4, 2));
        Assert.Null(_hierarchyService.GetSuperiorRowOfEmployee(4, 3));
    }

    [Fact]
    public void GetSuperiorRowOfEmployee_WithoutInitialization_ThrowsInvalidOperationException()
    {
        // Arrange
        var hierarchyService = new EmployeesHierarchy();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            hierarchyService.GetSuperiorRowOfEmployee(1, 2));

        Assert.Contains("initialized", exception.Message);
    }

    [Fact]
    public void FillEmployeesStructure_WithNullEmployees_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _hierarchyService.FillEmployeesStructure(null!));
    }

    [Fact]
    public void FillEmployeesStructure_WithComplexHierarchy_ReturnsCorrectStructure()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "CEO", SuperiorId = null },
            new Employee { Id = 2, Name = "Manager1", SuperiorId = 1 },
            new Employee { Id = 3, Name = "Manager2", SuperiorId = 1 },
            new Employee { Id = 4, Name = "Employee1", SuperiorId = 2 },
            new Employee { Id = 5, Name = "Employee2", SuperiorId = 2 },
            new Employee { Id = 6, Name = "Employee3", SuperiorId = 4 }
        };

        // Act
        var structure = _hierarchyService.FillEmployeesStructure(employees);

        // Assert
        Assert.Equal(9, structure.Count);

        Assert.Contains(structure, s => s.EmployeeId == 6 && s.SuperiorId == 1 && s.Distance == 3);
        Assert.Contains(structure, s => s.EmployeeId == 6 && s.SuperiorId == 2 && s.Distance == 2);
        Assert.Contains(structure, s => s.EmployeeId == 6 && s.SuperiorId == 4 && s.Distance == 1);
    }
}
