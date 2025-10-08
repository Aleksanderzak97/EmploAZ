namespace EmploAZ.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public int? SuperiorId { get; set; }
    public int? VacationPackageId { get; set; }

    public virtual Employee? Superior { get; set; }
    public virtual Team Team { get; set; } = null!;
    public virtual VacationPackage VacationPackage { get; set; } = null!;
    public virtual ICollection<Vacation> Vacations { get; set; } = new List<Vacation>();
}
