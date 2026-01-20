# KPI

## Version
- Version: 1.0.0
- Date: 2024-06-15

## Change History
| Date       | Version | Description                | Author       |
|------------|---------|----------------------------|--------------|
| 2024-06-15 | 1.0.0   | Initial KPI document       | TirsvadCLI   |

---

## Requirements
| ID     | Type                      | Requirement                                                                 | FURPS+ ID(s)         |
|--------|---------------------------|------------------------------------------------------------------------------|----------------------|
| R-001  | Functional requirement    | The system must allow users to create, edit, view, and delete projects.      | [F-002]             |
| R-002  | Functional requirement    | The system must support categorization and tagging of projects.              | [F-003]             |
| R-003  | Functional requirement    | The system must support user authentication and role-based access.           | [F-001], [F-007]    |
| R-004  | Non-functional requirement| The system must be user-friendly and accessible (WCAG 2.1 AA).               | [U-001], [U-003]    |
| R-005  | Non-functional requirement| The system must be reliable and available.                                   | [R-001]             |
| R-006  | Non-functional requirement| CRUD operations must be performed within an acceptable time frame.           | [P-001], [P-004]    |
| R-007  | Business requirement      | The system must support integration with external services (e.g., GitHub).   | [F-005], [X-005]    |
| R-008  | Business requirement      | The system must support future expansion and new features.                   | [S-005]             |
| R-009  | Business requirement      | The system must be maintainable and extensible.                              | [S-001], [S-005]    |
| R-010  | Functional requirement    | The system must allow users to create, edit, view, and delete user profiles. | [F-011]             |

## Functional KPI measurements
| ID     | KPI                                 | Measurement method                | Target         | Frequency            | FURPS+ ID(s)         |
|--------|-------------------------------------|-----------------------------------|----------------|----------------------|----------------------|
| K-001  | Project CRUD operations             | Number of successful operations   | 100% success   | Monthly              | [F-002], [P-001], [P-004] |
| K-002  | Project categorization/tagging      | % of projects with tags/categories| >= 90%         | Quarterly            | [F-003]              |
| K-003  | User authentication success         | Login success rate                | >= 99%         | Monthly              | [F-001], [F-007]     |
| K-011  | User profile CRUD operations        | Number of successful operations   | 100% success   | Monthly              | [F-011]              |

## Non-functional KPI measurements
| ID     | KPI                                 | Measurement method                | Target                 | Frequency            | FURPS+ ID(s)         |
|--------|-------------------------------------|-----------------------------------|------------------------|----------------------|----------------------|
| K-004  | Usability                           | User survey (1-5 scale)           | avg >= 4               | Annual               | [U-001], [U-002]     |
| K-005  | Accessibility                       | Accessibility audit               | WCAG 2.1 AA compliant  | Annual               | [U-003]              |
| K-006  | System reliability                  | Uptime monitoring                 | >= 99.5% uptime        | Monthly              | [R-001]              |
| K-007  | CRUD operation response time        | Average response time             | < 500 milliseconds     | Monthly              | [P-001], [P-004]     |

## Business KPI measurements
| ID     | KPI                                 | Measurement method                | Target                 | Frequency            | FURPS+ ID(s)         |
|--------|-------------------------------------|-----------------------------------|------------------------|----------------------|----------------------|
| K-008  | Integration with external services   | Number of integrations            | >= 2 integrations      | Annual               | [F-005], [X-005]     |
| K-009  | Support for future expansion        | Time to add new feature           | < 1 month              | Annual               | [S-005]              |
| K-010  | Maintainability                     | Code review/technical debt score  | Score >= 8/10          | Quarterly            | [S-001], [S-005]     |

<!-- Links -->

<!-- FURPS+ ID Links -->
[F-001]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-002]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-003]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-004]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-005]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-006]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-007]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-008]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-009]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-010]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[F-011]: ../preliminary-analysis/furps.md#functionality "FURPS + Functionality"
[U-001]: ../preliminary-analysis/furps.md#usability "FURPS + Usability"
[U-002]: ../preliminary-analysis/furps.md#usability "FURPS + Usability"
[U-003]: ../preliminary-analysis/furps.md#usability "FURPS + Usability"
[U-004]: ../preliminary-analysis/furps.md#usability "FURPS + Usability"
[U-005]: ../preliminary-analysis/furps.md#usability "FURPS + Usability"
[R-001]: ../preliminary-analysis/furps.md#reliability "FURPS + Reliability"
[R-002]: ../preliminary-analysis/furps.md#reliability "FURPS + Reliability"
[R-003]: ../preliminary-analysis/furps.md#reliability "FURPS + Reliability"
[R-004]: ../preliminary-analysis/furps.md#reliability "FURPS + Reliability"
[R-005]: ../preliminary-analysis/furps.md#reliability "FURPS + Reliability"
[P-001]: ../preliminary-analysis/furps.md#performance "FURPS + Performance"
[P-002]: ../preliminary-analysis/furps.md#performance "FURPS + Performance"
[P-003]: ../preliminary-analysis/furps.md#performance "FURPS + Performance"
[P-004]: ../preliminary-analysis/furps.md#performance "FURPS + Performance"
[S-001]: ../preliminary-analysis/furps.md#supportability "FURPS + Supportability"
[S-002]: ../preliminary-analysis/furps.md#supportability "FURPS + Supportability"
[S-003]: ../preliminary-analysis/furps.md#supportability "FURPS + Supportability"
[S-004]: ../preliminary-analysis/furps.md#supportability "FURPS + Supportability"
[S-005]: ../preliminary-analysis/furps.md#supportability "FURPS + Supportability"
[X-001]: ../preliminary-analysis/furps.md#plus "FURPS + Extensibility"
[X-002]: ../preliminary-analysis/furps.md#plus "FURPS + Extensibility"
[X-003]: ../preliminary-analysis/furps.md#plus "FURPS + Extensibility"
[X-004]: ../preliminary-analysis/furps.md#plus "FURPS + Extensibility"
[X-005]: ../preliminary-analysis/furps.md#plus "FURPS + Extensibility"
