using EmploAZ.Models;

namespace EmploAZ.Interfaces;

public interface IEmployeesHierarchyService
{
    List<EmployeeStructure> FillEmployeesStructure(List<Employee> employees);
    int? GetSuperiorRowOfEmployee(int employeeId, int superiorId);
}
