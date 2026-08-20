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
│ Microsoft SQL Server / LocalDB              │
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
| Database            | Microsoft SQL Server / LocalDB       |
| Authentication      | Cookie Authentication                |
| Authorization       | Role-Based Access Control (RBAC)     |
| Password Security   | PBKDF2 Key Derivation                |
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
│   ├── AccountController.cs
│   ├── ComplaintController.cs
│   ├── TechnicianController.cs
│   └── AdminController.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Migrations/
│
├── Models/
│
├── Services/
│
├── Views/
│   ├── Account/
│   ├── Complaint/
│   ├── Technician/
│   ├── Admin/
│   └── Shared/
│
├── wwwroot/
│
├── Scripts/
│
├── docs/
│   └── images/
│
├── Program.cs
├── DesignTimeDbContextFactory.cs
├── appsettings.json
└── JanSamadhan.csproj
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

### 2. Configure the Database

Update the connection string in:

```text
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=JanSamadhanDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

> Use your own SQL Server connection string if you are not using LocalDB.

### 3. Apply Database Migrations

Run:

```bash
dotnet ef database update
```

If the EF CLI tool is not installed:

```bash
dotnet tool install --global dotnet-ef
```

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Run the Application

```bash
dotnet run
```

Or run the project directly through **Visual Studio 2022**.

---

## 🔐 Security

JanSamadhan implements several security mechanisms, including:

* Cookie-based authentication
* Role-Based Access Control
* Secure password hashing using PBKDF2
* Protected administrative routes
* Entity Framework Core parameterized database operations
* Server-side authorization checks

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

## 📸 Screenshots

Screenshots and project documentation can be found in:

```text
docs/images/
```

---

## 📋 Documentation

The repository also contains the detailed **Requirement Analysis** document covering:

* Problem definition
* Stakeholders
* Actors
* Functional requirements
* System architecture
* Technology stack
* Complaint lifecycle
* Database requirements
* Security requirements
* Non-functional requirements

See:

[`Requirement_Analysis.md`](./Requirement_Analysis.md)

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
