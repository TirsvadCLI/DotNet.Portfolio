# Entity Relationship Diagram (ERD)

## Metadata
| **ID**      | **Description**                                                       | Cross Reference links                       |
|-------------|-----------------------------------------------------------------------|---------------------------------------------|
| UC-001-ERD  | Entity Relationship Diagram for Application user authentication use case  | [UC-001](../UC-001/README.md)               |

## Version
**Version:** 1.0  
**Reviewed/Approved:** 2026-01-24

## Version Log
| **Version** | **Date**       | **Author**      | **Change Description**                      |  
|-------------|----------------|-----------------|---------------------------------------------|
| 1.0         | 2026-01-24     | TirsvadCLI      | Initial creation of the entity relationship diagram.|

---

## Diagram
```mermaid
erDiagram

    ApplicationUser {
        Guid userId PK
        string passwordHash
        string email
    }

    IndentityUser {
        Guid userId PK
        string userName
        string normalizedUserName
        string email
        string passwordHash
    }

    IndentityRole {
        Guid roleId PK
        string roleName
        string normalizedRoleName
    }

    IndentityUserClaim {
        int id PK
        Guid userId FK
        string claimType
        string claimValue
    }

    IndentityUserToken {
        int id PK
        Guid userId FK
        string loginProvider
        string name
        string value
    }

    ApplicationUser ||--|| IndentityUser : "maps to"
    IndentityUser ||--o{ IndentityUserClaim : "has"
    IndentityUser ||--o{ IndentityRole : "assigned to"
    IndentityUser ||--o{ IndentityUserToken : "has"


