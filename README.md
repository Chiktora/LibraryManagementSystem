# Library Management System

![.NET](https://img.shields.io/badge/.NET-8.0-blue) ![License](https://img.shields.io/badge/License-MIT-green) ![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux%20%7C%20macOS-lightgrey) ![Database](https://img.shields.io/badge/Database-MSSQL-orange) ![Status](https://img.shields.io/badge/Status-Development-red)

## 📌 Overview
Library Management System is a web-based application developed in ASP.NET Core MVC with Entity Framework Core. It enables libraries to manage books, authors, genres, publishers, and users efficiently. The system includes authentication and role-based authorization.

## 🚀 Features
- **Book Management** - Add, edit, delete, and search for books.
- **Author & Publisher Management** - Store and manage author and publisher details.
- **User Authentication & Authorization** - ASP.NET Identity integration with role-based access control (Admin/User).
- **Borrowing System** - Users can borrow and return books with due dates.
- **Search & Filter** - Advanced search and filtering options.
- **Admin Panel** - Manage users, roles, and book records.

## 📂 Project Structure
```
LibraryManagementSystem/
│-- Controllers/            # Handles requests and business logic
│-- Models/                 # Entity models for database
│-- Views/                  # Razor views for UI rendering
│-- Data/                   # Database context and migrations
│-- Migrations/             # Entity Framework Core migrations
│-- wwwroot/                # Static files (CSS, JS, images)
│-- appsettings.json        # Configuration settings
│-- Program.cs              # Application entry point
│-- LibraryManagementSystem.csproj  # Project file
```

## 🛠️ Technologies Used
- **.NET 8.0** - Web framework
- **ASP.NET Core MVC** - Web architecture
- **Entity Framework Core** - Database ORM
- **SQL Server** - Database system
- **Bootstrap 5** - Frontend styling
- **Identity Framework** - User authentication

## ⚡ Setup & Installation
### 🔹 Prerequisites
Ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### 🔹 Clone the Repository
```bash
git clone https://github.com/your-username/LibraryManagementSystem.git
cd LibraryManagementSystem
```

### 🔹 Configure Database
1. Update `appsettings.json` with your SQL Server connection string:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=LibraryDb;Trusted_Connection=True;"
}
```
2. Apply migrations:
```bash
dotnet ef database update
```

### 🔹 Run the Application
```bash
dotnet run
```
Open your browser and navigate to `http://localhost:5000`

## 🔑 Authentication & Roles
- **Admin:** `admin@library.com` / `Admin123!`
- **User:** Register as a normal user
- **Roles:** `Admin`, `User`

## 🤝 Contribution
Contributions are welcome! Follow these steps:
1. Fork the repository 🍴
2. Create a feature branch (`git checkout -b feature-branch`)
3. Commit changes (`git commit -m "Added new feature"`)
4. Push to your fork (`git push origin feature-branch`)
5. Open a Pull Request 🚀

## 📜 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
🔥 Developed with ❤️ using ASP.NET Core & EF Core.
