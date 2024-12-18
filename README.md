# Authentication API Application

## Overview
This project provides a robust Authentication API built with ASP.NET Core. It supports user registration, login, and role-based authentication. The application uses modern design principles and dependency injection to ensure scalability and maintainability.

## Features
- **User Registration**: Create new users with customizable password policies.
- **User Login**: Supports login via username or email.
- **JWT Authentication**: Secures endpoints using JSON Web Tokens.
- **Role Management**: Assign roles to users during registration.
- **Customizable Components**: Easily integrate with your existing systems and modify key configurations.

## Getting Started
### Prerequisites
- .NET 6 or later installed.
- PostgreSQL or another supported database (configured via Entity Framework Core).

### Setup Instructions
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/KeremEski/ASP.Net-Authentication-Identity-JWT.git
   cd authentication-api
   ```

2. **Install Dependencies**:
   ```bash
   dotnet restore
   ```

3. **Configure Database**:
   - Open `appsettings.json`.
   - Update the `DefaultConnection` string to match your database settings:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=your_host;Database=your_db;Username=your_user;Password=your_password"
     }
     ```

4. **Run Migrations**:
   ```bash
   dotnet ef database update
   ```

5. **Configure JWT Settings**:
   - In `appsettings.json`, update the `JWT` section with your custom values:
     ```json
     "JWT": {
       "Issuer": "your_issuer",
       "Audience": "your_audience",
       "SigninKey": "your_secret_key"
     }
     ```

6. **Run the Application**:
   ```bash
   dotnet run
   ```

### Endpoints
- **POST /api/auth/register**: Register a new user.
  - Request Body: `{ "Email": "string", "UserName": "string", "Password": "string" }`
  - Response: `{ "Email": "string", "UserName": "string", "Token": "string" }`

- **POST /api/auth/login**: Login as an existing user.
  - Request Body: `{ "UserName": "string", "Password": "string" }`
  - Response: `{ "Email": "string", "UserName": "string", "Token": "string" }`

## Customization
### Password Policy
- Modify the password requirements in `Program.cs`:
  ```csharp
  options.Password.RequireDigit = true;
  options.Password.RequiredLength = 8;
  options.Password.RequireNonAlphanumeric = true;
  ```

### Role Assignment
- Update the default role in `AuthController`:
  ```csharp
  var roleResult = await _userManager.AddToRoleAsync(user, "User");
  ```
  Replace "User" with your desired default role.

### Token Configuration
- The `TokenService` implementation can be customized to include additional claims or logic.

## Contribution
Feel free to fork this repository and submit pull requests. Feedback and suggestions are welcome!

## License
This project is licensed under the MIT License. See the LICENSE file for details.

---
For any issues or questions, please contact "kerem_eski@hotmail.com".

