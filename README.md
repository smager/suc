# 🧩 SmagerUp Core (SUC)

**SmagerUp Core (SUC)** is the central backend platform of the SmagerUp ecosystem.  
It securely hosts, licenses, and manages JavaScript modules and libraries used by **SmagerUp Studio (SUS)** — the visual app builder.

---

## ⚙️ Overview

SUC acts as the **engine and license authority** for all modules used in SmagerUp Studio.  
Each module or library hosted in SUC is versioned, licensed (free or paid), and accessible only through authentication.

| System | Code Name | Description |
|---------|------------|--------------|
| **SmagerUp Core** | `SUC` | Backend API that stores, secures, and licenses all JS modules. |
| **SmagerUp Studio** | `SUS` | Frontend app builder that connects to SUC for modules and resources. |

---

## 🔒 Core Responsibilities

- **Authentication & Authorization**
  - Token-based login (JWT)
  - API key validation per account
- **Module Management**
  - Store and serve JS libraries
  - Version control per module
- **License Management**
  - License validation before access
  - License type enforcement (FreeUseNonModifiable, PaidNonModifiable)
- **Account Management**
  - API key issuance
  - Module access control
- **Integration with SmagerUp Studio**
  - SUS connects to SUC via REST API
  - Authenticated module download and rendering

