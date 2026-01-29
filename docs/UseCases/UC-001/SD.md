# Sequence Diagram (SD)

## Metadata
| **ID**      | **Description**                                                        | Cross Reference links                       |
|-------------|------------------------------------------------------------------------|---------------------------------------------|
| UC-001-SD   | Sequence Diagram for Application user authentication use case          | [UC-001](../UC-001/README.md)               |

## Version
**Version:** 1.0  
**Reviewed/Approved:** 2026-01-20

## Version Log
| **Version** | **Date**       | **Author**      | **Change Description**                      |
|-------------|----------------|-----------------|---------------------------------------------|
| 1.0         | 2026-01-20     | TirsvadCLI      | Initial creation of the sequence diagram.   |

---

## Diagram
```mermaid
sequenceDiagram
    participant AuthenticationService as "Core:AuthenticationService"
    participant UserManager as "Core:UserManager"
    participant SignInManager as "Core:SignInManager"
    participant ApplicationDbContext as "Infrastructure:ApplicationDbContext"
    AuthenticationService->>+UserManager: FindByNameAsync(username)
    UserManager->>+ApplicationDbContext: _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username (Users LINQ query)
    ApplicationDbContext-->>-UserManager: ApplicationUser
    UserManager-->>-AuthenticationService: ApplicationUser
    AuthenticationService->>+SignInManager: AuthenticateAsync(user, password)
    SignInManager->>+ApplicationDbContext: GetPasswordHash(userId)
    ApplicationDbContext-->>-SignInManager: PasswordHash (string)
    SignInManager-->>-AuthenticationService: SignInResult (Token)
```

### Notes
- Suffixes like Service. Manager and Helpers can be renamed based on actual implementation. See our QA guidelines for naming conventions.
