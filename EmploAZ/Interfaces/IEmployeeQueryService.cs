using EmploAZ.Models;

namespace EmploAZ.Interfaces;

public interface IEmployeeQueryService
{
    List<Employee> GetDotNetEmployeesWithVacationsIn2019();
    List<(Employee Employee, int UsedDays)> GetEmployeesWithUsedVacationDaysCurrentYear();
    List<Team> GetTeamsWithNoVacationsIn2019();
    string GetSqlForQuery2a();
    string GetSqlForQuery2b();
    string GetSqlForQuery2c();
}
