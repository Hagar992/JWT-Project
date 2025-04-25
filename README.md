# JWT Authentication Project

This project is a simple web application that uses **JWT (JSON Web Tokens)** for user authentication and login. It is built with **ASP.NET Core** and provides API endpoints for user registration, login, and role management.

## Features

- **User Registration**: Allows new users to register with their email, username, and password.
- **Login**: Users can log in using their email and password to receive a **Token**.
- **Role Management**: Admins can assign roles to users.
- **API Protection**: Uses **JWT** to protect API endpoints, ensuring that only users with valid tokens can access them.
- **Swagger Documentation**: Provides a UI to interact with and test the API using Swagger.

## Prerequisites

- .NET Core 5.0 or later
- SQL Server or any database that supports EF Core
- Visual Studio or any other .NET Core-supported IDE
- Familiarity with JWT and authentication in ASP.NET Core

## Setup Instructions

### 1. Setup the Environment

- Ensure that you have **.NET Core 5.0** or later installed.
- Install **SQL Server** or ensure that you have a compatible database for **EF Core**.
- Configure the `appsettings.json` file like this:

```json
{
  "JWT": {
    "Key": "very_secret_key_to_sign_the_tokens",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience",
    "DurationInDays": 7
  },
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String_Here"
  }
}
