using EmploAZ.Data;
using EmploAZ.Interfaces;
using EmploAZ.Models;
using EmploAZ.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmploAZ;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== EmploAZ - Employee Management System ===\n");

        var services = new ServiceCollection();
        ConfigureServices(services);

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        var vacationService = scope.ServiceProvider.GetRequiredService<IVacationService>();
        var hierarchyService = scope.ServiceProvider.GetRequiredService<IEmployeesHierarchyService>();
        var queryService = scope.ServiceProvider.GetRequiredService<IEmployeeQueryService>();

        try
        {
            SeedData(context);

            Console.WriteLine("--- Test 1: Hierarchia Pracowników ---");
            TestEmployeeHierarchy(context, hierarchyService);

            Console.WriteLine("\n--- Test 2: Zapytania SQL/LINQ ---");
            TestEmployeeQueries(queryService);

            Console.WriteLine("\n--- Test 3: Urlopy ---");
            TestVacationServices(context, vacationService);

            Console.WriteLine("\n--- Test 4: SQL Optimization ---");
            Console.WriteLine("Optymalizacje SQL dla zadania 3:");
            Console.WriteLine("1. Indeks na Vacation(EmployeeId, DateSince) - przyspiesza filtrowanie po pracowniku i roku");
            Console.WriteLine("2. Indeks na Employee(VacationPackageId) - przyspiesza JOIN z VacationPackage");
            Console.WriteLine("3. Materialized view dla sum urlopów - cache wyników agregacji");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Details: {ex.InnerException?.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void ConfigureServices(ServiceCollection services)
    {
        // Database
        services.AddDbContext<EmployeeDbContext>(options =>
            options.UseInMemoryDatabase("EmployeeDb"));

        // Services
        services.AddScoped<IVacationService, VacationService>();
        services.AddScoped<IEmployeesHierarchyService, EmployeesHierarchy>();
        services.AddScoped<IEmployeeQueryService, EmployeeQueryService>();
    }

    static void SeedData(EmployeeDbContext context)
    {
        // Team
        var dotNetTeam = new Team { Id = 1, Name = ".NET" };
        var javaTeam = new Team { Id = 2, Name = "Java" };
        context.Teams.AddRange(dotNetTeam, javaTeam);

        // VacationPackage
        var vacPackage = new VacationPackage
        {
            Id = 1,
            Name = "Standard 2019",
            GrantedDays = 26,
            Year = 2019
        };
        context.VacationPackages.Add(vacPackage);

        // Employees
        var employees = new[]
        {
            new Employee { Id = 1, Name = "Jan Kowalski", TeamId = 1, SuperiorId = null, VacationPackageId = 1 },
            new Employee { Id = 2, Name = "Kamil Nowak", TeamId = 1, SuperiorId = 1, VacationPackageId = 1 },
            new Employee { Id = 3, Name = "Anna Mariacka", TeamId = 1, SuperiorId = 1, VacationPackageId = 1 },
            new Employee { Id = 4, Name = "Andrzej Abacki", TeamId = 1, SuperiorId = 2, VacationPackageId = 1 },
            new Employee { Id = 5, Name = "Tomasz Java", TeamId = 2, SuperiorId = null, VacationPackageId = 1 } // spełnia 2c
        };
        context.Employees.AddRange(employees);

        // Vacations
        var vacations = new[]
        {
            new Vacation
            {
                Id = 1,
                EmployeeId = 2,
                DateSince = new DateTime(2019, 6, 1),
                DateUntil = new DateTime(2019, 6, 10),
                NumberOfHours = 80,
                IsPartialVacation = false
            },
            new Vacation
            {
                Id = 2,
                EmployeeId = 4,
                DateSince = new DateTime(2019, 7, 1),
                DateUntil = new DateTime(2019, 7, 5),
                NumberOfHours = 40,
                IsPartialVacation = false
            },
            new Vacation
            {
                Id = 3,
                EmployeeId = 5,
                DateSince = new DateTime(2020, 6, 1),
                DateUntil = new DateTime(2020, 6, 5),
                NumberOfHours = 40,
                IsPartialVacation = false
            }
        };
        context.Vacations.AddRange(vacations);

        context.SaveChanges();
    }

    static void TestEmployeeHierarchy(EmployeeDbContext context, IEmployeesHierarchyService hierarchyService)
    {
        var employees = context.Employees.ToList();
        var structure = hierarchyService.FillEmployeesStructure(employees);

        Console.WriteLine($"Struktura hierarchii ({structure.Count} relacji):\n");

        var grouped = structure.GroupBy(s => s.EmployeeId);
        foreach (var group in grouped)
        {
            var emp = employees.First(e => e.Id == group.Key);
            Console.WriteLine($"{emp.Name} (ID: {emp.Id}):");
            foreach (var s in group.OrderBy(x => x.Distance))
            {
                var sup = employees.First(e => e.Id == s.SuperiorId);
                Console.WriteLine($"  -> {sup.Name} (rząd: {s.Distance})");
            }
        }

        // Przykłady z zadania
        Console.WriteLine("\nPrzykłady GetSuperiorRowOfEmployee:");
        var row1 = hierarchyService.GetSuperiorRowOfEmployee(2, 1);
        Console.WriteLine($"  GetSuperiorRowOfEmployee(2, 1) = {row1}");

        var row2 = hierarchyService.GetSuperiorRowOfEmployee(4, 3);
        Console.WriteLine($"  GetSuperiorRowOfEmployee(4, 3) = {row2?.ToString() ?? "null"}");

        var row3 = hierarchyService.GetSuperiorRowOfEmployee(4, 1);
        Console.WriteLine($"  GetSuperiorRowOfEmployee(4, 1) = {row3}");
    }

    static void TestEmployeeQueries(IEmployeeQueryService queryService)
    {
        // Zadanie 2a
        var dotNetEmployees = queryService.GetDotNetEmployeesWithVacationsIn2019();
        Console.WriteLine($"2a) Pracownicy .NET z urlopami w 2019: {dotNetEmployees.Count}");
        foreach (var emp in dotNetEmployees)
        {
            Console.WriteLine($"  - {emp.Name}");
        }

        // Zadanie 2b
        var employeesWithUsedDays = queryService.GetEmployeesWithUsedVacationDaysCurrentYear();
        Console.WriteLine($"\n2b) Pracownicy z wykorzystanymi dniami w bieżącym roku:");
        foreach (var (employee, usedDays) in employeesWithUsedDays)
        {
            Console.WriteLine($"  - {employee.Name}: {usedDays} dni");
        }

        // Zadanie 2c
        var teamsWithoutVacations = queryService.GetTeamsWithNoVacationsIn2019();
        Console.WriteLine($"\n2c) Zespoły bez urlopów w 2019: {teamsWithoutVacations.Count}");
        foreach (var team in teamsWithoutVacations)
        {
            Console.WriteLine($"  - {team.Name}");
        }

        // Wyświetl SQL zapytania
        Console.WriteLine($"\nSQL dla zapytania 2a:\n{queryService.GetSqlForQuery2a()}");
        Console.WriteLine($"\nSQL dla zapytania 2b:\n{queryService.GetSqlForQuery2b()}");
        Console.WriteLine($"\nSQL dla zapytania 2c:\n{queryService.GetSqlForQuery2c()}");
    }

    static void TestVacationServices(EmployeeDbContext context, IVacationService vacationService)
    {
        var employees = context.Employees
            .Include(e => e.VacationPackage)
            .Include(e => e.Vacations)
            .ToList();

        foreach (var employee in employees)
        {
            var vacations = context.Vacations.Where(v => v.EmployeeId == employee.Id).ToList();
            var vacationPackage = employee.VacationPackage;
            var freeDays = vacationService.CountFreeDaysForEmployee(employee, vacations, vacationPackage);
            var canRequest = vacationService.IfEmployeeCanRequestVacation(employee, vacations, vacationPackage);

            Console.WriteLine($"\n {employee.Name} (ID: {employee.Id}):");
            Console.WriteLine($"  Przyznane dni: {vacationPackage?.GrantedDays ?? 0}");
            Console.WriteLine($"  Wykorzystane dni: {(vacationPackage?.GrantedDays ?? 0) - freeDays}");
            Console.WriteLine($"  Wolne dni: {freeDays}");
            Console.WriteLine($"  Może zgłosić wniosek: {(canRequest ? "TAK" : "NIE")}");
        }
    }
}