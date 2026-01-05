# 📘 Institute Management System – ASP.NET Core Web API

## 🔍 Project Overview

A secure and scalable Institute Management System built using ASP.NET Core 8 Web API, designed to manage students, courses, enrollments, marks, and administrative operations with role-based access control.

The project follows clean architecture principles, uses DTO-based communication, and implements modern backend best practices suitable for real-world applications.

## 🚀 Key Features

- 🔐 JWT Authentication & Authorization
- 👥 Multi-Role System (Admin / Student)
- 🧩 Clean & Modular Architecture
- 📦 DTOs (Request / Response separation)
- ✅ Advanced Validation Layer per Domain
- 🔄 Database Transactions
- 🔑 Secure Password Hashing (BCrypt)
- 📄 Swagger API Documentation
- 📬 Postman Collection for API Testing
- 🗄️ Entity Framework Core with SQL Server
- 📊 Student Marks & Enrollment Management

## 🛠️ Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- BCrypt
- Swagger
- Postman

## 🧱 Project Structure

Controllers/
DTOs/
├── Request
└── Response
Data/
Models/
├── Entities
└── MTM
Services/
Validators/
Middlewares/
Helpers/
Migrations/


This structure ensures maintainability, scalability, and clear separation of concerns.

## 🔐 Security

- JWT-based authentication
- Role-based authorization (Admin / Student)
- Secure password hashing using BCrypt
- Input validation at multiple layers

## 📌 API Documentation

- Swagger UI available for interactive API testing
- Postman Collection included for real request/response examples

## ⚙️ How to Run the Project

1. Configure your SQL Server connection string in `appsettings.json`
2. Apply migrations:
   ```bash
   Update-Database
3.Run the project

4.Open Swagger:
https://localhost:{port}/swagger

## 🎯 Project Goal

This project was built as a real-world backend simulation to demonstrate backend development skills and is intended for:

Backend Internship

Junior Backend Developer position

ASP.NET Core Web API roles

## 🔮 Future Improvements

Unit & Integration Testing

Refresh Token implementation

Role management dashboard

Deployment (Docker / Cloud)