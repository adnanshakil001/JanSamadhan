# JanSamadhan 🇮🇳

### Citizen Grievance & Civic Complaint Management System

**JanSamadhan** is a web-based **e-Governance Civic Complaint Management System** designed to provide citizens with a centralized platform for reporting, tracking, and resolving local civic issues.

The platform connects **Citizens, Technicians, and Administrators** through a structured complaint lifecycle, enabling transparent issue reporting, location-based complaint discovery, technician assignment, resolution tracking, feedback, and performance monitoring.

---

## 🎯 Problem Statement

Civic issues such as **potholes, broken streetlights, garbage accumulation, damaged roads, and other infrastructure problems** are often reported through disconnected channels such as phone calls, emails, or informal communication.

This can lead to:

* Lost or duplicate complaints
* Lack of complaint tracking
* Poor visibility into resolution progress
* Difficulty assigning issues to the appropriate department
* Limited accountability for field technicians
* Lack of centralized analytics for administrators

**JanSamadhan** addresses these challenges through a centralized digital platform for end-to-end civic complaint management.

---

## 🚀 Key Features

### 👤 Citizen Portal

* Secure registration and login
* Submit civic complaints
* Add complaint title and description
* Upload issue photographs
* Capture geographical coordinates
* View submitted complaints
* Track complaint status
* Discover nearby complaints using an interactive map
* Comment on complaints
* Rate resolved complaints
* Provide resolution feedback

### 🛠️ Technician Portal

* Department-specific work dashboard
* View available complaints
* Claim and process assigned complaints
* Update complaint status
* Upload resolution/after-work photographs
* Earn points for resolving complaints
* Earn badges based on performance
* View technician rankings on the scoreboard

### 🏛️ Administrator Portal

* Manage technicians
* Manage departments
* Monitor complaints across the platform
* View complaint locations on an interactive map
* Monitor active and resolved complaints
* Track system activity and performance

### 🗺️ Location-Based Complaint Management

* GPS-based complaint location
* Interactive maps using Leaflet.js
* OpenStreetMap integration
* Nearby complaint discovery
* Geographical visualization of civic issues

### 🏆 Gamification

Technicians are rewarded for resolving complaints through a performance-based system:

**Points → Badges → Scoreboard**

Example badge levels include:

* 🥉 Bronze
* 🥈 Silver
* 🥇 Gold

This encourages faster and more efficient complaint resolution.

---

## 🔄 Complaint Lifecycle

```text
Citizen
   │
   ▼
Create Complaint
   │
   ▼
OPEN
   │
   ▼
Help Desk / Department Review
   │
   ▼
Technician Assignment
   │
   ▼
IN PROGRESS
   │
   ▼
Issue Resolution
   │
   ▼
Resolution Photo Upload
   │
   ▼
FIXED / RESOLVED
   │
   ▼
Citizen Feedback & Rating
```

If an issue cannot be resolved, it can be **reassigned or escalated** for further action.

---

## 🏗️ System Architecture

JanSamadhan follows a **multi-tier ASP.NET Core MVC architecture**.

```text
┌─────────────────────────────────────────────┐
│              PRESENTATION LAYER             │
│                                             │
│ Razor Views │ Bootstrap │ JavaScript        │
│ Leaflet.js  │ OpenStreetMap                 │
└──────────────────────┬──────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────┐
│           APPLICATION / MVC LAYER           │
│                                             │
│ Controllers │ Business Services             │
│ Authentication │ RBAC │ Gamification       │
└──────────────────────┬──────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────┐
│              DATA ACCESS LAYER              │
│                                             │
│ Entity Framework Core                       │
│ ApplicationDbContext │ LINQ │ Migrations   │
└──────────────────────┬──────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────┐
│                DATABASE LAYER               │
│                                             │
│ SQLite (Default) / MS SQL Server            │
│ Users │ Complaints │ Departments            │
│ Assignments │ Comments │ Ratings            │
└─────────────────────────────────────────────┘
```

---

## 🛠️ Technology Stack

| Category            | Technology                           |
| ------------------- | ------------------------------------ |
| Framework           | ASP.NET Core MVC                     |
| Runtime             | .NET 8                               |
| Language            | C# 12                                |
| Frontend            | Razor Views, HTML5, CSS3, JavaScript |
| UI Framework        | Bootstrap 5                          |
| Mapping             | Leaflet.js                           |
| Map Data            | OpenStreetMap                        |
| ORM                 | Entity Framework Core 8              |
| Database            | SQLite (Default) / MS SQL Server     |
| Authentication      | Cookie Authentication                |
| Authorization       | Role-Based Access Control (RBAC)     |
| Password Security   | PBKDF2 (100,000 Iterations, Salted)  |
| Containerization    | Docker (Multi-Stage .NET 8)          |
| Database Migrations | EF Core Migrations                   |
| IDE                 | Visual Studio 2022 / VS Code         |

---

## 👥 User Roles

| Role              | Responsibilities                                                                    |
| ----------------- | ----------------------------------------------------------------------------------- |
| **Citizen**       | Report and track civic complaints, view nearby issues, comment and provide feedback |
| **Technician**    | View department complaints, resolve issues, upload proof, earn points and badges    |
| **Administrator** | Manage users/departments, monitor complaints, and analyze civic issue data          |

---

## 📂 Project Structure

```text
JanSamadhan/
│
├── Controllers/
│   ├── AccountController.cs          # Authentication, session cookies & user profiles
│   ├── AdminController.cs            # Admin dashboard, triage, assignment & CSV export
│   ├── BadgeController.cs            # Gamification badge showcase
│   ├── CommentController.cs          # Multi-party threaded comments & secure attachments
│   ├── ComplaintController.cs        # Grievance ingestion, GPS capture, ratings & status
│   ├── DepartmentController.cs       # Department-level triage & assignment
│   ├── HomeController.cs             # Landing page, nearby issues & scoreboards
│   ├── MapController.cs              # GeoJSON streaming & bounding-box proximity API
│   ├── RoleController.cs             # Administrative RBAC role assignment
│   └── TechnicianController.cs       # Work order queue, two-stage photo uploads & points
│
├── Data/
│   └── ApplicationDbContext.cs       # Entity Framework Core DbContext & relationships
│
├── Models/
│   ├── Admin.cs                      # Administrator credentials & profile
│   ├── Badge.cs                      # Gamification tier thresholds & icons
│   ├── Comment.cs                    # Threaded discussions & visibility flags
│   ├── Complaint.cs                  # Core grievance entity with GPS & department link
│   ├── ComplaintAssignment.cs        # Work-order dispatch history & audit logs
│   ├── ComplaintRating.cs            # 1-5 star citizen reviews & feedback
│   ├── Department.cs                 # Municipal department master data
│   ├── TechnicianPhoto.cs            # In-progress & fixed photo proof records
│   └── User.cs                       # Citizens & field technician profiles
│
├── Services/
│   └── PasswordService.cs            # PBKDF2 HMAC-SHA256 (100,000 iterations) hashing
│
├── Views/
│   ├── Account/                      # Citizen, Admin & Technician login/register views
│   ├── Admin/                        # Admin dashboard, triage, user & scoreboard views
│   ├── Badge/                        # Gamification badge showcase
│   ├── Complaint/                    # Filing, detail tracking & rating views
│   ├── Department/                   # Department management views
│   ├── Home/                         # Landing page, nearby discovery & leaderboards
│   ├── Map/                          # Geospatial GIS map interfaces
│   ├── Role/                         # Administrative role management console
│   ├── Shared/                       # _Layout with CSRF token & common partials
│   └── Technician/                   # Technician workspace, dual-photo upload & queue
│
├── wwwroot/
│   ├── css/                          # Application styles
│   └── uploads/                      # Upload directories (comments, technician)
│
├── Scripts/                          # Database seed scripts
├── .dockerignore                      # Docker build exclusions
├── .gitignore                         # Git exclusions (credentials & temporary files)
├── Dockerfile                         # Production multi-stage .NET 8 Dockerfile
├── Program.cs                         # Application entry point, DI & middleware pipeline
├── DesignTimeDbContextFactory.cs      # EF Core design-time factory
├── JanSamadhan.csproj                 # Project manifest & NuGet dependencies
└── Requirement_Analysis_v2.pdf       # Software requirements specification
```

---

## ⚙️ Getting Started

### Prerequisites

Make sure the following are installed:

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Microsoft SQL Server or SQL Server LocalDB
* Visual Studio 2022 or VS Code
* Git

### 1. Clone the Repository

```bash
git clone https://github.com/adnanshakil001/JanSamadhan.git
cd JanSamadhan
```

### 2. Run the Application (Local Development)

JanSamadhan is pre-configured with **SQLite** for rapid local bootstrapping. Simply restore dependencies and run:

```bash
dotnet restore
dotnet run
```

Open your browser at `http://localhost:5000` (or the port displayed in your terminal).

> **Note for SQL Server**: If you prefer Microsoft SQL Server or LocalDB, update `DefaultConnection` in `appsettings.json` and enable `options.UseSqlServer(...)` in `Program.cs`.

---

### 🐳 Run with Docker

To build and run JanSamadhan as a self-contained container:

```bash
# 1. Build the Docker image
docker build -t jansamadhan .

# 2. Run the container on port 8080 with persistent data volumes
docker run -p 8080:8080 -v jansamadhan_data:/app/data -v jansamadhan_uploads:/app/wwwroot/uploads jansamadhan
```

Visit `http://localhost:8080`.

---

### ☁️ Cloud Deployment (Render / Railway / VPS)

JanSamadhan includes a production-ready `Dockerfile` compatible with container-native cloud platforms:

1. Push your repository to GitHub.
2. In **Render** or **Railway**, create a new **Web Service** and connect this repository.
3. Select **Docker** as the runtime.
4. Set the following environment variables:
   * `ASPNETCORE_ENVIRONMENT`: `Production`
   * `ADMIN_EMAIL`: `your-admin-email@example.com`
   * `ADMIN_PASSWORD`: `YourStrongSecretPassword123!`
5. (Recommended) Mount a persistent disk at `/app/data` to persist the SQLite database across restarts.

---

## 🔐 Security & Hardening

JanSamadhan implements comprehensive, production-grade security mechanisms:

* **Dual-Channel CSRF Protection:** Enforces server-side `AutoValidateAntiforgeryTokenAttribute` on all POST actions, HTML form verification tokens, and automatic `X-CSRF-TOKEN` header injection via `$.ajaxSetup` for all AJAX/`$.post` operations.
* **Cryptographic Password Security:** Salted PBKDF2 key derivation using HMAC-SHA256 with **100,000 iterations** and 128-bit CSPRNG salts, verified via constant-time comparison to prevent timing attacks.
* **Zero-Trust File Upload Validation:** Enforced across complaint, technician, and comment uploads with extension whitelisting (`.jpg`, `.png`, `.webp`, `.gif`, `.pdf`), MIME cross-validation, 15MB file-size limits, and pure GUID file renaming to prevent webshell and directory traversal attacks.
* **Role-Based Access Control (RBAC):** Strict policy and route enforcement across Citizen (`User`), `Technician`, `DepartmentManager`, and `Admin` actors.
* **Insecure Direct Object Reference (IDOR) Mitigation:** Server-side ownership validation and restricted state-machine transitions preventing unauthorized complaint manipulation.
* **Hardened Transport & Session Cookies:** Encrypted auth cookies configured with `HttpOnly = true`, `CookieSecurePolicy.Always`, `SameSite = SameSiteMode.Lax`, and 7-day sliding expiration.
* **Spreadsheet Formula Injection Sanitization:** Cell escaping in CSV/Excel reporting pipelines to neutralize CWE-1236 attacks.
* **Zero Hardcoded Secrets:** Runtime environment variable injection for production administrator initialization (`ADMIN_EMAIL`, `ADMIN_PASSWORD`).
* **SQL Injection Immunity:** 100% parameterized LINQ queries through Entity Framework Core 8.

---

## 📊 Core Functional Modules

```text
Authentication
     │
     ├── Citizen
     ├── Technician
     └── Administrator
     
Complaint Management
     │
     ├── Create Complaint
     ├── Track Complaint
     ├── Assign Complaint
     ├── Update Status
     └── Resolve Complaint

Location Services
     │
     ├── GPS Coordinates
     ├── Nearby Complaints
     └── Interactive Maps

Technician Management
     │
     ├── Department Assignment
     ├── Resolution Tracking
     ├── Points
     ├── Badges
     └── Scoreboard

Feedback
     │
     ├── Comments
     ├── Ratings
     └── Resolution Feedback

Administration
     │
     ├── User Management
     ├── Department Management
     ├── Complaint Monitoring
     └── Map-based Analytics
```

---

## 📋 Documentation

Comprehensive architectural specifications and engineering documentation:

* **System Design Blueprint:** Detailed architecture, data flow diagrams, geospatial math, and interview Q&A in [`SYSTEM_DESIGN.md`](./SYSTEM_DESIGN.md).
* **Requirement Analysis Specification:** Full requirements specification available in [`Requirement_Analysis_v2.pdf`](./Requirement_Analysis_v2.pdf).

---

## 🔮 Future Enhancements

Potential future improvements include:

* 📱 Progressive Web App / mobile application
* 🔔 Email and SMS notifications
* 🤖 AI-based automatic complaint categorization
* 🧠 Duplicate complaint detection
* 📍 Advanced geospatial heatmap analytics
* 📊 Advanced administrator dashboards
* 🔎 Intelligent complaint prioritization
* ☁️ Cloud deployment
* 📈 Department-level performance analytics
* 🌐 Multi-language support

---

## 🎓 Project Context

**JanSamadhan** is developed as a major software engineering project demonstrating the practical implementation of:

* Software Requirement Analysis
* MVC Architecture
* Object-Oriented Programming
* Relational Database Design
* RESTful/backend development concepts
* Authentication & Authorization
* Role-Based Access Control
* Geospatial application development
* Entity Framework Core
* Agile-oriented project planning

---

## 👨‍💻 Author

**Adnan Shakil**

* GitHub: [@adnanshakil001](https://github.com/adnanshakil001)
* Project: [JanSamadhan](https://github.com/adnanshakil001/JanSamadhan)

---

## ⭐ Support

If you find this project useful or interesting, consider giving the repository a **⭐ star** on GitHub.

---

**JanSamadhan — Making civic issue reporting transparent, accountable, and accessible. 🇮🇳**
