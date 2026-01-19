readme for me i can put on git, i am done with project
Leah - Role Play API 🎭
A modern, secure RESTful API for user authentication and management built with ASP.NET Core and PostgreSQL. Features email verification, OTP-based authentication, and secure password handling.

🚀 Live Demo
API Base URL: https://role-play-j81w.onrender.com

Health Check: https://role-play-j81w.onrender.com/health

✨ Features
✅ User Registration with email verification

✅ OTP-based Authentication (One-Time Password)

✅ Secure Password Hashing using BCrypt

✅ Email Notifications via Resend/Mailjet

✅ PostgreSQL Database with Entity Framework Core

✅ Dockerized Deployment on Render

✅ RESTful API Design with proper HTTP status codes

✅ Input Validation and error handling

✅ Production-ready with environment-based configuration

📚 API Documentation
Base URL
text
https://role-play-j81w.onrender.com/api
Authentication Endpoints
1. Register New User
http
POST /api/users/new-user
Content-Type: application/json

{
    "username": "johndoe",
    "email": "john@example.com",
    "fullName": "John Doe",
    "password": "SecurePass123!"
}
Response:

json
{
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "fullName": "John Doe",
    "role": "User",
    "isVerified": false,
    "message": "Verification email sent"
}
2. Verify Email with OTP
http
POST /api/users/verify-email
Content-Type: application/json

{
    "email": "john@example.com",
    "otpCode": "123456"
}
3. Login
http
POST /api/users/login
Content-Type: application/json

{
    "email": "john@example.com",
    "password": "SecurePass123!"
}
4. Resend OTP
http
POST /api/users/resend-otp
Content-Type: application/json

{
    "email": "john@example.com"
}
5. Get All Users (Admin)
http
GET /api/users/all-users
Authorization: Bearer {token}
🛠️ Tech Stack
Backend: ASP.NET Core 10.0

Database: PostgreSQL 16

ORM: Entity Framework Core 8.0

Authentication: BCrypt.Net-Next

Email Service: Resend API

Containerization: Docker

Hosting: Render.com

Database Hosting: Railway.app

🚦 Getting Started
Prerequisites
.NET 10.0 SDK

Docker (optional)

PostgreSQL or Docker

Local Development
Clone the repository

bash
git clone https://github.com/OwenLovesCoding/role-play.git
cd role-play
Configure environment variables

bash
# Set up user secrets
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=roleplay;Username=postgres;Password=yourpassword"
dotnet user-secrets set "Resend:ApiKey" "your_resend_api_key"
Apply database migrations

bash
dotnet ef database update --context UserContext
Run the application

bash
dotnet run
# API will be available at http://localhost:5173
Using Docker
bash
# Build and run with Docker
docker build -t role-play-api .
docker run -p 8080:8080 --env-file .env role-play-api
🔧 Configuration
Environment Variables
Variable	Description	Example
ConnectionStrings__DefaultConnection	PostgreSQL connection string	Host=localhost;Port=5432;Database=roleplay;...
Resend__ApiKey	Resend API key for emails	re_123456789
ASPNETCORE_ENVIRONMENT	Environment setting	Development or Production
Database Schema
Users Table: Stores user information with encrypted passwords

Email Verification: OTP codes with 15-minute expiration

Audit Fields: CreatedAt, UpdatedAt timestamps

🚢 Deployment
Deploy to Render
Connect your GitHub repository to Render

Configure environment variables in Render dashboard

Deploy automatically on git push

Render Configuration (render.yaml)
yaml
services:
  - type: web
    name: role-play-api
    runtime: docker
    region: oregon
    plan: free
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ConnectionStrings__DefaultConnection
        value: ${DATABASE_URL}
      - key: Resend__ApiKey
        sync: false
🧪 Testing
Test with Postman/curl
bash
# Test registration
curl -X POST https://role-play-j81w.onrender.com/api/users/new-user \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","fullName":"Test User","password":"Test123!"}'

📁 Project Structure
text
role-play/
├── Controllers/          # API Controllers
│   └── Users.cs         # User management endpoints
├── Models/              # Data models
│   ├── User.cs         # User entity
│   └── UserContext.cs  # Database context
├── DTOs/               # Data Transfer Objects
├── Migrations/         # Database migrations
├── Program.cs          # Application startup
├── appsettings.json    # Configuration
├── Dockerfile          # Docker configuration
├── render.yaml         # Render deployment config
└── README.md           # This file
🔒 Security Features
Password Hashing: BCrypt with salt rounds

Email Verification: Required before login

Input Validation: Strong regex patterns for username, email, password

SQL Injection Prevention: Parameterized queries via EF Core

No Sensitive Data Exposure: Passwords never returned in responses

📈 Performance
Database Indexing: Optimized queries with proper indexes

Connection Pooling: Efficient database connections

Async Operations: Non-blocking API calls

Caching: Built-in .NET response caching

🤝 Contributing
Fork the repository

Create a feature branch (git checkout -b feature/AmazingFeature)

Commit your changes (git commit -m 'Add some AmazingFeature')

Push to the branch (git push origin feature/AmazingFeature)

Open a Pull Request

🙏 Acknowledgments
ASP.NET Core for the robust framework

Render for free hosting

Railway for PostgreSQL hosting

Resend for email API

Built with ❤️ by Owen | GitHub | Live Demo

"Simplicity is the ultimate sophistication."
