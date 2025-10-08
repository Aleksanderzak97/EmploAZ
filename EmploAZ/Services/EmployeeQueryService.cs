using EmploAZ.Data;
using EmploAZ.Interfaces;
using EmploAZ.Models;
using Microsoft.EntityFrameworkCore;

namespace EmploAZ.Services;

public class EmployeeQueryService : IEmployeeQueryService
{
    private readonly EmployeeDbContext _context;

    public EmployeeQueryService(EmployeeDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Zadanie 2a: Zwraca listę wszystkich pracowników z zespołu o nazwie ".NET", którzy mają co najmniej jeden wniosek urlopowy w 2019 roku
    /// </summary>
    public List<Employee> GetDotNetEmployeesWithVacationsIn2019()
    {
        try
        {
            return _context.Employees
                .Include(e => e.Team)
                .Include(e => e.Vacations)
                .Where(e => e.Team.Name == ".NET" &&
                           e.Vacations.Any(v => v.DateSince.Year == 2019))
                .ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving .NET employees with vacations in 2019", ex);
        }
    }

    /// <summary>
    /// Zadanie 2b: Zwraca listę pracowników wraz z liczbą dni urlopowych zużytych w bieżącym roku 
    /// (za dni zużyte uznajemy wszystkie dni we wnioskach urlopowych które są datą przeszłą)
    /// </summary>
    public List<(Employee Employee, int UsedDays)> GetEmployeesWithUsedVacationDaysCurrentYear()
    {
        try
        {
            var currentYear = DateTime.Now.Year;
            var today = DateTime.Now.Date;

            return _context.Employees
                .Include(e => e.Vacations)
                .Select(e => new
                {
                    Employee = e,
                    UsedHours = e.Vacations
                        .Where(v => v.DateSince.Year == currentYear && v.DateUntil.Date < today)
                        .Sum(v => v.NumberOfHours)
                })
                .AsEnumerable()
                .Select(x => (x.Employee, UsedDays: x.UsedHours / 8))
                .ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving employees with used vacation days", ex);
        }
    }

    /// <summary>
    /// Zadanie 2c: Zwraca listę zespołów w których pracownicy nie złożyli jeszcze żadnego dnia urlopowego w 2019 roku
    /// </summary>
    public List<Team> GetTeamsWithNoVacationsIn2019()
    {
        try
        {
            return _context.Teams
                .Include(t => t.Employees)
                    .ThenInclude(e => e.Vacations)
                .Where(t => !t.Employees.Any(e => e.Vacations.Any(v => v.DateSince.Year == 2019)))
                .ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving teams with no vacations in 2019", ex);
        }
    }

    /// <summary>
    /// SQL dla zadania 2a
    /// </summary>
    public string GetSqlForQuery2a()
    {
        return @"
            SELECT e.*
            FROM Employees e
            INNER JOIN Teams t ON e.TeamId = t.Id
            WHERE t.Name = '.NET'
            AND EXISTS (
                SELECT 1 
                FROM Vacations v 
                WHERE v.EmployeeId = e.Id 
                AND YEAR(v.DateSince) = 2019
            )";
    }

    /// <summary>
    /// SQL dla zadania 2b
    /// </summary>
    public string GetSqlForQuery2b()
    {
        return @"
            SELECT 
                e.Id,
                e.Name,
                COALESCE(SUM(v.NumberOfHours) / 8, 0) as UsedDays
            FROM Employees e
            LEFT JOIN Vacations v ON e.Id = v.EmployeeId
                AND YEAR(v.DateSince) = YEAR(GETDATE())
                AND v.DateUntil < CAST(GETDATE() AS DATE)
            GROUP BY e.Id, e.Name";
    }

    /// <summary>
    /// SQL dla zadania 2c
    /// </summary>
    public string GetSqlForQuery2c()
    {
        return @"
            SELECT t.*
            FROM Teams t
            WHERE NOT EXISTS (
                SELECT 1
                FROM Employees e
                INNER JOIN Vacations v ON e.Id = v.EmployeeId
                WHERE e.TeamId = t.Id
                AND YEAR(v.DateSince) = 2019
            )";
    }
}