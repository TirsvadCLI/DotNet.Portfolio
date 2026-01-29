# Domain Class Diagram (DCD)

## Metadata
| **ID**      | **Description**                                                        | Cross Reference links                       |
|-------------|-----------------------------------------------------------------------|---------------------------------------------|
| UC-001-DCD  | Domain Class Diagram for Application user authentication use case    | [UC-001](../UC-001/README.md)               |

## Version
**Version:** 1.0
**Reviewed/Approved:** 2026-01-24

## Version Log
| **Version** | **Date**       | **Author**      | **Change Description**                      |
|-------------|----------------|-----------------|---------------------------------------------|
| 1.0         | 2026-01-24     | TirsvadCLI      | Initial creation of the domain class diagram. |

---

## Diagram
```mermaid
classDiagram

  namespace Core.Services {
    class AuthenticationService {
      +AuthenticateAsync(username: string, password: string): AuthResult
    }
  }

  namespace Core.Managers {
    class UserManager {
      +FindByNameAsync(username: string): ApplicationUser
    }
    class SignInManager {
      +AuthenticateAsync(user: ApplicationUser, password: string): SignInResult
    }
  }

  namespace Infrastructure.Data {
    class ApplicationDbContext {
      +Users: DbSet<ApplicationUser>
      +GetPasswordHash(userId: Guid): string
    }
  }

  namespace Domain.Entities {
    class ApplicationUser {
      +Email: string
      +PasswordHash: string
    }

    class IEntity {
      <<interface>>
    }
  }
  
  AuthenticationService --> UserManager : uses
  AuthenticationService --> SignInManager : uses
  UserManager --> ApplicationDbContext : uses
  SignInManager --> ApplicationDbContext : uses
  ApplicationDbContext --> ApplicationUser : manages
  ApplicationUser ..|> IEntity : implements
