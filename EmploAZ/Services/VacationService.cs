using EmploAZ.Interfaces;
using EmploAZ.Models;

namespace EmploAZ.Services;

public class VacationService : IVacationService
{
    /// <summary>
    /// Zadanie 3: Oblicza ile jeszcze dni urlopowych ma do wykorzystania pracownik w bieżącym roku
    /// </summary>
    public int CountFreeDaysForEmployee(Employee employee, List<Vacation> vacations, VacationPackage vacationPackage)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee));
        if (vacations == null) throw new ArgumentNullException(nameof(vacations));

        if (vacationPackage == null)
            return 0;

        var currentYear = DateTime.Now.Year;

        var usedHours = vacations
            .Where(v => v.EmployeeId == employee.Id && v.DateSince.Year == currentYear)
            .Sum(v => v.NumberOfHours);

        var usedDays = usedHours / 8;
        var freeDays = vacationPackage.GrantedDays - usedDays;

        return Math.Max(0, freeDays);
    }

    /// <summary>
    /// Zadanie 4: Sprawdza czy pracownik może zgłosić wniosek urlopowy
    /// </summary>
    public bool IfEmployeeCanRequestVacation(Employee employee, List<Vacation> vacations, VacationPackage vacationPackage)
    {
        if (employee == null) throw new ArgumentNullException(nameof(employee));
        if (vacations == null) throw new ArgumentNullException(nameof(vacations));

        return CountFreeDaysForEmployee(employee, vacations, vacationPackage) > 0;
    }
}
