# TirsvadWeb Portfolio – AI Agent Instructions

## Project Overview
Portfolio website for TirsvadWeb, showcasing web projects, skills, and services. Built with modern web technologies for an engaging user experience.

## Architecture & Structure

### Core Dependencies
- **TirsvadCLI.Portfolio.Domain** ← TirsvadCLI.Portfolio.Core ← TirsvadCLI.Portfolio.Infrastructure
- **TirsvadCLI.Portfolio.Domain** ← TirsvadCLI.Portfolio.Core ← TirsvadCLI.Portfolio.WebUI.Client ← TirsvadCLI.Portfolio.WebUI

### Key Projects
- **Domain**: Entities & business rules
- **Core**: Application logic, CQRS, services
- **Infrastructure**: EF Core, external services, email, file access
- **Tests**: Unit & integration tests

## C# Conventions
- Microsoft naming conventions
- `PascalCase` for types/methods
- `camelCase` for parameters/private fields
- Prefix interfaces with `I` (e.g., `IRepository`)
- Suffix async methods with `Async` (e.g., `GetByIdAsync`)
- Prefix private fields with `_` (e.g., `_repository`)
- Use `{}` for blocks except single-line `return`/`throw`
- Keep single-line blocks on one line (e.g., `if (x) return y;`)
- Prefer primary constructors for required dependencies
- Never use primary constructor parameters directly—assign to private fields

### Class Naming & Responsibility
- Use descriptive names; follow Single Responsibility Principle
- **Helper**: Static, pure functions, no state, very small
- **Manager**: Methods for a context, no state outside injected classes, business logic not in domain model
- **Mapper**: Transforms one object to another
- **Service**: Performs operations with side effects or orchestration, may have state
- **Handler**: Responds to requests, executes business logic via other classes

## Key Dependencies & Patterns
- **EF Core**: Data access (PostgreSQL default, easily changed to SQL Server)

### Central Package Management
- All package versions in `Directory.Packages.props`
- Use `<PackageReference Include="..." />` without Version attribute
