# 🎓 Institute Management System API

A RESTful backend built with **ASP.NET Core 8 Web API** for managing institute operations, including administrators, doctors, students, courses, course offerings, enrollments, and student marks.

The project focuses on secure authentication, role-based authorization, transactional data consistency, and a well-structured architecture using **Entity Framework Core** and **SQL Server**.

---

# ✨ Features

## 🔐 Authentication & Security

- JWT Authentication
- Refresh Token Authentication
- BCrypt Password Hashing
- Role-Based Authorization
- Authentication Rate Limiting
- Secure Token Rotation

## 🎓 Academic Management

- Administrator Management
- Doctor Management
- Student Management
- Course Management
- Course Offerings
- Student Enrollments
- Student Marks Management
- Exam Types

## ⚙️ Backend Features

- RESTful API Design
- DTO-based Request & Response Models
- Entity Framework Core
- SQL Server Integration
- FluentValidation
- Database Transactions
- Dependency Injection
- EF Core Migrations
- Swagger / OpenAPI Documentation

---

# 🏗 Architecture

The project follows a layered architecture with clear separation of responsibilities.

## Main Components

- Controllers
- DTOs
- Services
- Data (DbContext)
- Models
- Validators
- Middlewares
- Extensions
- Migrations

## Design Principles

- Separation of Concerns
- Layered Architecture
- Dependency Injection
- DTO Pattern
- Service Layer
- Transactional Operations

---

# 🛠 Technology Stack

- ASP.NET Core 8 Web API
- C#
- Entity Framework Core
- SQL Server
- JWT Authentication
- BCrypt
- FluentValidation
- Swagger / OpenAPI
- Rate Limiting
- Git

---

# 📂 Project Structure

```text
Controllers/
DTOs/
├── Request/
└── Response/
Data/
Models/
├── Entities/
└── MTM/
Services/
Validators/
Middlewares/
Extensions/
Migrations/
```

---

# 🚀 Getting Started

## Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 (or Visual Studio Code)

## Installation

### 1. Clone the repository

```bash
git clone <repository-url>
```

### 2. Open the solution

Open the project using **Visual Studio** or **Visual Studio Code**.

### 3. Configure the database

Update the SQL Server connection string inside:

```json
appsettings.json
```

### 4. Apply Entity Framework Core migrations

Using the .NET CLI:

```bash
dotnet ef database update
```

Or from the Visual Studio Package Manager Console:

```powershell
Update-Database
```

### 5. Run the project

```bash
dotnet run
```

Or simply press **F5** in Visual Studio.

### 6. Open Swagger

```text
https://localhost:{port}/swagger
```

---

# 📄 API Documentation

Swagger is enabled for interactive API exploration and testing.

After running the project, navigate to:

```text
https://localhost:{port}/swagger
```

---

# 🔒 Security

- JWT Authentication
- Refresh Tokens
- BCrypt Password Hashing
- Role-Based Authorization
- FluentValidation
- Authentication Rate Limiting

---

# 🎯 Project Highlights

- Secure authentication using JWT and Refresh Tokens
- Multi-role authorization (Admin, Doctor, Student)
- Transactional operations for data consistency
- Entity Framework Core with relational database modeling
- Input validation using FluentValidation
- RESTful API following modern backend development practices

---

# 🔮 Future Improvements

- Global Exception Handling Middleware
- Centralized Logging
- Automated Unit & Integration Testing
- Pagination & Filtering
- Response Caching
- Docker Support
- CI/CD Pipeline

---

# 👨‍💻 Author

**Dilar Almaoo**

Software Engineer
