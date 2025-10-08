# EmploAZ

## 📋 Opis projektu

EmploAZ to rozwiązanie zadania rekrutacyjnego zaimplementowane w .NET 8.

## 🏗️ Architektura

- **Clean Architecture** z podziałem na warstwy
- **Dependency Injection** z Microsoft.Extensions.DependencyInjection
- **Entity Framework Core** z InMemory database
- **Transitive Closure Algorithm** dla hierarchii (O(n) z memoizacją)
- **Enterprise Patterns**: Service Layer, Interfaces

## 🚀 Technologie

- .NET 8
- Entity Framework Core 8.0.20
- EF Core InMemory Provider
- xUnit (testy jednostkowe)
- Microsoft DI Container

## 📁 Struktura projektu

```
EmploAZ/
├── EmploAZ/                    # Główny projekt
│   ├── Data/                   # DbContext i konfiguracja EF
│   ├── Models/                 # Modele danych
│   ├── Interfaces/             # Interfejsy serwisów
│   ├── Services/               # Implementacje logiki biznesowej
│   └── Program.cs              # Punkt wejścia i DI
├── EmploAZ.Tests/              # Projekt testów jednostkowych
│   ├── EmployeeHierarchyTests.cs
│   └── VacationServiceTests.cs
└── EmploAZ.sln                 # Solution file
```