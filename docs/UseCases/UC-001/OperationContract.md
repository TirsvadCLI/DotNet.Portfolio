
# Operation Contract (OC)

## Metadata
| **ID** | **Description** | Cross Reference links |
|--------|-----------------|-----------------------|
| UC-001-OC | Operation Contract for Login Request in Application User Authentication Use Case | [UC-001] |

## Version
**Version:** 1.0  
**Reviewed/Approved:** 2026-01-24

## Version Log
| **Version** | **Date**       | **Author**      | **Change Description**                      |
|-------------|----------------|-----------------|---------------------------------------------|
| 1.0         | 2026-01-24     | TirsvadCLI      | Initial creation of the operation contract. |

## Operation: Login Request

| **Operation Name** | Login Request |
|--------------------|--------------|
| **Cross Reference**| [UC-001-SSD] |
| **Input**          | username: string, password: string |
| **Output**         | LoginResult (success: bool, message: string, userId: Guid?, token: string?) |
| **Preconditions**  | User is not authenticated. System is available. |
| **Postconditions** | If credentials are valid, user is authenticated and session is established. |

<!-- Links -->
[UC-001]: ./README.md "Use Case UC-001: Application user authentication"
[UC-001-SSD]: ./ssd.md "System Sequence Diagram for UC-001"
