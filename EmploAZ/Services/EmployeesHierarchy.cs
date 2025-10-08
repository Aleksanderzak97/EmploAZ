using EmploAZ.Interfaces;
using EmploAZ.Models;

namespace EmploAZ.Services;

public class EmployeesHierarchy : IEmployeesHierarchyService
{
    private Dictionary<int, Dictionary<int, int>>? _closure;

    /// <summary>
    /// Zadanie 1: Wypełnia strukturę hierarchii pracowników.
    /// </summary>
    public List<EmployeeStructure> FillEmployeesStructure(List<Employee> employees)
    {
        if (employees == null) throw new ArgumentNullException(nameof(employees));

        var byId = employees.ToDictionary(e => e.Id);
        _closure = new Dictionary<int, Dictionary<int, int>>(employees.Count);

        foreach (var e in employees)
        {
            var map = new Dictionary<int, int>();
            int distance = 0;
            var current = e;

            while (current?.SuperiorId != null)
            {
                distance++;
                var supId = current.SuperiorId.Value;

                if (_closure.TryGetValue(supId, out var known))
                {
                    map[supId] = Math.Min(map.ContainsKey(supId) ? map[supId] : int.MaxValue, distance);

                    foreach (var kv in known)
                    {
                        var supSup = kv.Key;
                        var dist = distance + kv.Value;
                        if (!map.TryGetValue(supSup, out var prev) || dist < prev)
                            map[supSup] = dist;
                    }
                    break;
                }
                else
                {
                    map[supId] = Math.Min(map.ContainsKey(supId) ? map[supId] : int.MaxValue, distance);
                    current = byId.TryGetValue(supId, out var sup) ? sup : null;
                }
            }

            _closure[e.Id] = map;
        }

        return _closure
            .SelectMany(kv => kv.Value.Select(inner => new EmployeeStructure
            {
                EmployeeId = kv.Key,
                SuperiorId = inner.Key,
                Distance = inner.Value
            }))
            .ToList();
    }

    /// <summary>
    /// Zadanie 1: Zwraca rząd przełożonego lub null jeśli superiorId nie jest przełożonym employeeId
    /// </summary>
    public int? GetSuperiorRowOfEmployee(int employeeId, int superiorId)
    {
        if (_closure == null)
            throw new InvalidOperationException("Employee hierarchy has not been initialized. Call FillEmployeesStructure first.");

        return _closure.TryGetValue(employeeId, out var dict) && dict.TryGetValue(superiorId, out var dist)
            ? dist
            : null;
    }
}
