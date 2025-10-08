using EmploAZ.Models;

namespace EmploAZ.Interfaces;

public interface IVacationService
{
    int CountFreeDaysForEmployee(Employee employee, List<Vacation> vacations, VacationPackage vacationPackage);
    bool IfEmployeeCanRequestVacation(Employee employee, List<Vacation> vacations, VacationPackage vacationPackage);
}
