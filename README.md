
# AuthService Microservice

AuthService is a professional microservice for user authentication and management built with ASP.NET Core, Entity Framework Core, SQL Server, and JWT. The solution follows SOLID principles and separates concerns among controllers, services, repositories, and mapping. This project provides:

- **User Authentication:** Registration and login endpoints that return a secure JWT token.
- **User CRUD Operations:** Full Create, Read, Update, and Delete functionality for user management.
- **AutoMapper Integration:** Mapping between entities and DTOs.
- **Environment-based Configuration:** Utilizes .env files for both development and production environments.
- **Docker Support:** Ready for containerization with Docker.

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Prerequisites](#prerequisites)
- [Installation and Setup](#installation-and-setup)
  - [Environment Variables](#environment-variables)
  - [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [Endpoints](#endpoints)
  - [Authentication Endpoints](#authentication-endpoints)
  - [User CRUD Endpoints](#user-crud-endpoints)
- [Testing](#testing)
- [Docker Deployment](#docker-deployment)
- [Contributing](#contributing)
- [License](#license)

---

## Architecture Overview

The project is structured into several layers to ensure clear separation of concerns:

- **Models/Entities:**  
  Contains the full blueprint for a User (including properties like Username, Email, FirstName, LastName, etc.).
  
- **DTOs:**  
  Contains Data Transfer Objects (e.g., CreateUserDto, UpdateUserDto, UserDto, RegisterRequest, LoginRequest) used to exchange data with clients in a simplified manner.
  
- **AutoMapper:**  
  The MappingProfile translates between entities and DTOs to ensure that the service layer can work with the correct data format.
  
- **Repositories:**  
  Implements data access logic via Entity Framework Core. The `UserRepository` handles all database operations.
  
- **Services:**  
  Contains business logic.  
  - **AuthService:** Manages user registration, password hashing, and token generation using JWT.  
  - **UserService:** Handles CRUD operations for user management.
  
- **Controllers:**  
  Exposes API endpoints. The project has two controllers:
  - **AuthController:** Manages authentication endpoints (registration and login).
  - **UsersController:** Manages user CRUD operations.
  
- **Program.cs:**  
  Configures dependency injection (DI), JWT authentication, database context, and middleware.

---

## Prerequisites

- **.NET 8.0 SDK** (or the target version specified in the project)
- **SQL Server** (or a compatible SQL Server instance)
- **Docker** (optional, for containerization)
- **Visual Studio** or your preferred code editor
- **Node.js / npm** (if you plan to integrate with frontend tests or use additional tools)

---

## Installation and Setup

### Clone the Repository

```bash
git clone https://github.com/EdgarC97/AuthService.git
cd AuthService
```

### Install Dependencies

Restore NuGet packages:

```bash
dotnet restore
```

### Environment Variables

This project uses environment variables for configuration. Create two files at the root of the project:

- **.env.local** (for development):

  ```env
  DB_CONNECTION=Server=localhost;Database=AuthDb;User Id=sa;Password=YourDevPassword!;
  JWT_SECRET=YourDevelopmentSecretKey
  JWT_EXPIRATION=60
  JWT_ISSUER=AuthServiceDev
  JWT_AUDIENCE=AuthServiceUsersDev
  ```

- **.env.production** (for production):

  ```env
  DB_CONNECTION=Server=yourprodserver;Database=AuthDb;User Id=sa;Password=YourProdPassword!;
  JWT_SECRET=YourProductionSecretKey
  JWT_EXPIRATION=120
  JWT_ISSUER=AuthServiceProd
  JWT_AUDIENCE=AuthServiceUsersProd
  ```

The project uses [DotNetEnv](https://github.com/tonerdo/dotnet-env) to load these files based on the `ASPNETCORE_ENVIRONMENT` variable.

### Database Setup

Make sure SQL Server is running and accessible. For development, the project uses `EnsureCreated()` in Program.cs to automatically create the database. In production, you may use EF Core migrations:

```bash
dotnet ef migrations add InitialMigration
dotnet ef database update
```

---

## Running the Application

To run the application locally:

```bash
dotnet run
```

The service will start and listen on a URL (e.g., `http://localhost:5147`). Swagger UI is available at `/swagger` for API exploration.

---

## Endpoints

### Authentication Endpoints

- **POST /api/Auth/register**  
  Registers a new user.  
  **Request Body:**
  ```json
  {
    "username": "johndoe",
    "email": "john.doe@example.com",
    "password": "yourpassword",
    "firstName": "John",
    "lastName": "Doe"
  }
  ```
  **Response:**  
  Returns a JWT token.

- **POST /api/Auth/login**  
  Authenticates an existing user.  
  **Request Body:**
  ```json
  {
    "username": "johndoe",
    "password": "yourpassword"
  }
  ```
  **Response:**  
  Returns a JWT token.

### User CRUD Endpoints

- **GET /api/Users**  
  Retrieves a list of all users.
  
- **GET /api/Users/{id}**  
  Retrieves a specific user by ID.

- **PUT /api/Users/{id}**  
  Updates a user's information.  
  **Request Body:** (example for UpdateUserDto)
  ```json
  {
    "email": "new.email@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "newpassword"  // optional
  }
  ```

- **DELETE /api/Users/{id}**  
  Deletes a user by ID.

---

## Testing

The project is designed with testability in mind using xUnit. Unit tests are provided for controllers and services, and you can write additional tests following these guidelines.

### Setting Up the Test Project

1. **Create the Test Project:**  
   If you haven’t already, add a new xUnit Test Project (e.g., `AuthService.Tests`) to your solution.

2. **Add a Reference to the Main Project:**  
   In the test project, add a project reference to `AuthService`:
   ```bash
   dotnet add reference ../AuthService/AuthService.csproj
   ```

3. **Install Required NuGet Packages:**  
   In the test project, install the following packages:
   ```bash
   dotnet add package Moq
   dotnet add package Microsoft.NET.Test.Sdk
   dotnet add package xunit
   dotnet add package xunit.runner.visualstudio
   ```

### Writing Unit Tests

- **Service Tests:**  
  Use Moq to mock dependencies (such as repositories) and test business logic in your AuthService and UserService classes.

- **Controller Tests:**  
  Mock service interfaces (IAuthService, IUserService) to test your controllers.  
  For example, an AuthController test might verify that a valid registration returns an OK result with a token, while invalid login returns an Unauthorized result.

### Running the Tests

Run the tests using the following command:

```bash
dotnet test
```

Or use the Test Explorer in Visual Studio to run and debug your tests.

---

## Docker Deployment

The project is configured for Docker. A sample Dockerfile is included. To build and run using Docker:

1. **Build the Docker image:**

   ```bash
   docker build -t authservice .
   ```

2. **Run the Docker container:**

   ```bash
   docker run -d -p 5000:80 --env-file .env.production authservice
   ```

You may also use `docker-compose` for multi-container setups (e.g., running SQL Server alongside the service).

---

## Contributing

Contributions are welcome! Please follow these guidelines:
- Fork the repository and create your branch.
- Ensure your code follows SOLID principles and clean architecture.
- Write unit tests for new features.
- Submit a pull request with a clear explanation of your changes.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

By following this README, users will be able to set up, run, and test the AuthService microservice with ease. Happy coding!
