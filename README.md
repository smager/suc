# 🧩 SmagerUp Core (SUC)

**SmagerUp Core (SUC)** is the backbone of the **SmagerUp ecosystem**, responsible for managing and licensing modular web components — from JavaScript logic to HTML and CSS content. It powers **SmagerUp Studio (SUS)** by providing secure, versioned, and license-aware content delivery through a unified API.

---

## 🚀 Key Features

- 🔐 **JWT Authentication** — Secure token-based access for SmagerUp Studio clients.  
- 📦 **Modular Content Management** — Hosts JavaScript, HTML, and CSS modules with version control.  
- 🧾 **License Enforcement** — Supports free, paid, and commercial-use licensing per module.  
- ⚙️ **Dapper-based Data Layer** — High-performance and lightweight SQL access.  
- 🌐 **Unified API Response** — Standardized success/fail structure across all endpoints.  
- 🧩 **Integration Ready** — Designed for seamless communication with SmagerUp Studio (SUS).  

---

## ⚙️ Overview

SUC acts as the **engine and license authority** for all modules used in SmagerUp Studio.  
Each module or library hosted in SUC is versioned, licensed (free or paid), and accessible only through authentication.

| System | Code Name | Description |
|---------|------------|--------------|
| **SmagerUp Core** | `SUC` | Backend API that stores, secures, and licenses all JS modules. |
| **SmagerUp Studio** | `SUS` | Frontend app builder that connects to SUC for modules and resources. |

---

