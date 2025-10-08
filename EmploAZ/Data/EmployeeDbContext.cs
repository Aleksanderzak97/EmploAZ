using EmploAZ.Models;
using Microsoft.EntityFrameworkCore;

namespace EmploAZ.Data;

public class EmployeeDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Vacation> Vacations { get; set; }
    public DbSet<VacationPackage> VacationPackages { get; set; }

    public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Employee -> Superior (self-referencing)
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Superior)
            .WithMany()
            .HasForeignKey(e => e.SuperiorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Employee -> Team
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Team)
            .WithMany(t => t.Employees)
            .HasForeignKey(e => e.TeamId);

        // Employee -> VacationPackage
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.VacationPackage)
            .WithMany(vp => vp.Employees)
            .HasForeignKey(e => e.VacationPackageId);

        // Vacation -> Employee
        modelBuilder.Entity<Vacation>()
            .HasOne(v => v.Employee)
            .WithMany(e => e.Vacations)
            .HasForeignKey(v => v.EmployeeId);

        base.OnModelCreating(modelBuilder);
    }
}
