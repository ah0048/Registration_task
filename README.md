# Company Registration System

A comprehensive full-stack company registration system built with .NET Core Web API backend and Angular frontend, featuring secure authentication, OTP verification, and file upload capabilities.

## 🚀 Features

### Core Functionality

- **Company Registration**: Multi-step registration process with comprehensive validation
- **OTP Verification**: Email-based OTP system with resend functionality
- **Password Management**: Secure password setting with complexity requirements
- **Authentication**: JWT-based authentication system
- **File Upload**: Company logo upload with image validation and cloud storage
- **Dashboard**: Protected home page with company information

### Technical Highlights

- **4-Layer Architecture**: Clean separation of concerns (Data, Repository, Services, API)
- **Result Pattern**: Consistent error handling across the application
- **Real-time Validation**: Client and server-side validation
- **Responsive Design**: Bootstrap-based responsive UI
- **Security**: JWT authentication, password complexity, file validation
- **Cloud Integration**: Cloudinary for image storage
- **Email Service**: MailKit integration for OTP delivery

## 🏗️ Architecture

### Backend (.NET Core 9.0)

```
Backend/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   └── HomeController.cs
├── Services/            # Business Logic Layer
│   ├── Implementation/
│   └── Interfaces/
├── Repositories/        # Data Access Layer
│   ├── Implementation/
│   └── Interfaces/
├── Models/             # Data Models
├── DTOs/               # Data Transfer Objects
├── Helpers/            # Utilities and Validators
└── Migrations/         # EF Database Migrations
```

### Frontend (Angular 16+)

```
Frontend/src/app/
├── Components/          # UI Components
│   ├── register/
│   ├── check-otp/
│   ├── set-password/
│   ├── login/
│   └── home/
├── Services/           # HTTP Services
├── Models/             # TypeScript Interfaces
├── Guards/             # Route Guards
└── Interceptors/       # HTTP Interceptors
```

## 🛠️ Technology Stack

### Backend

- **.NET Core 9.0** - Web API Framework
- **Entity Framework Core** - ORM with PostgreSQL
- **ASP.NET Core Identity** - User Management
- **JWT Bearer Authentication** - Security
- **AutoMapper** - Object Mapping
- **Cloudinary** - Image Storage
- **MailKit** - Email Service
- **Swagger/OpenAPI** - API Documentation

### Frontend

- **Angular 16+** - Frontend Framework
- **TypeScript** - Programming Language
- **Bootstrap 5** - UI Framework
- **RxJS** - Reactive Programming
- **SweetAlert2** - User Notifications
- **ngx-spinner** - Loading Indicators

### Database

- **PostgreSQL** - Primary Database
- **Entity Framework Migrations** - Schema Management

## 📋 Prerequisites

- **.NET 9.0 SDK** or later
- **Node.js 18+** and npm
- **PostgreSQL 12+**
- **Angular CLI** (`npm install -g @angular/cli`)

## ⚙️ Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/ah0048/Registration_task.git
cd Registration_task
```

### 2. Backend Setup

#### Database Configuration

1. Create a PostgreSQL database
2. Update connection string in `Backend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=CompanyRegistration;Username=your_username;Password=your_password"
  }
}
```

#### Email Configuration

Configure email settings in `appsettings.json`:

```json
{
  "EmailSettings": {
    "From": "your_email",
    "Username": "your_username",
    "AppPassword": "your_app_password",
    "Host": "smtp.gmail.com",
    "Port": 587
  }
}
```

#### Cloudinary Configuration

Set up Cloudinary for image uploads:

```json
{
  "CloudinarySettings": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_api_key",
    "ApiSecret": "your_api_secret"
  }
}
```

#### JWT Configuration

Configure JWT settings:

```json
{
  "JwtSettings": {
    "Key": "your-super-secret-key-at-least-32-characters-long",
    "Issuer": "https://localhost:7270",
    "Audience": "https://localhost:4200"
  }
}
```

#### Run Backend

```bash
cd Backend
dotnet restore
dotnet ef database update
dotnet run
```

The API will be available at `https://localhost:7270`

### 3. Frontend Setup

```bash
cd Frontend
npm install
ng serve
```

The application will be available at `http://localhost:4200`

## 📚 API Documentation

### Authentication Endpoints

#### Register Company

```http
POST /api/Auth/register
Content-Type: multipart/form-data

FormData:
- companyNameAr: string (required)
- companyNameEn: string (required)
- email: string (required)
- phoneNumber: string (optional)
- websiteUrl: string (required)
- logo: file (optional)
```

#### Verify OTP

```http
POST /api/Auth/otp-valid
Content-Type: application/json

{
  "id": "user-id",
  "otpCode": "123456"
}
```

#### Set Password

```http
POST /api/Auth/set-password
Content-Type: application/json

{
  "id": "user-id",
  "newPassword": "SecurePass123!",
  "confirmPassword": "SecurePass123!"
}
```

#### Login

```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

#### Get Home Data

```http
GET /api/Home
Authorization: Bearer {jwt-token}
```

## 🎯 User Flow

1. **Registration**: User fills company registration form with optional logo upload
2. **Email Verification**: System sends OTP to registered email
3. **OTP Validation**: User enters OTP with visual tooltip showing the code
4. **Password Setup**: User creates secure password with validation
5. **Login**: User authenticates with email and password
6. **Dashboard**: Access to protected home page with company information

## 🔒 Security Features

- **Input Validation**: Comprehensive client and server-side validation
- **File Upload Security**: Image type, size, and MIME type validation
- **Password Complexity**: Minimum 7 characters with uppercase, lowercase, number, and special character
- **JWT Authentication**: Secure token-based authentication
- **CORS Protection**: Configured for specific origins
- **SQL Injection Prevention**: Entity Framework parameterized queries

## 🎨 UI/UX Features

- **Responsive Design**: Works on desktop, tablet, and mobile devices
- **Loading States**: Spinner indicators for async operations
- **Form Validation**: Real-time validation with user-friendly error messages
- **Image Preview**: Logo preview before upload
- **Password Visibility**: Toggle for password fields
- **Success/Error Notifications**: SweetAlert2 integration
- **Professional Styling**: Bootstrap-based modern interface

## 🧪 Testing

### Backend Testing

```bash
cd Backend
dotnet test
```

### Frontend Testing

```bash
cd Frontend
ng test
```

## 📁 Project Structure Details

### Backend Key Files

- `Program.cs` - Application configuration and dependency injection
- `AuthService.cs` - Core authentication business logic
- `CompanyUser.cs` - User entity model
- `Result.cs` - Result pattern implementation
- `AllowedImageExtensionsAttribute.cs` - Custom file validation

### Frontend Key Files

- `auth.service.ts` - HTTP service for authentication
- `auth-guard.ts` - Route protection
- `auth-interceptor.ts` - JWT token injection
- `register.component.ts` - Registration form logic
- `home.component.ts` - Dashboard functionality

## 🚀 Deployment

### Backend Deployment

1. Update connection strings for production
2. Configure HTTPS certificates
3. Set up environment variables for sensitive data
4. Deploy to IIS, Azure App Service, or Docker

### Frontend Deployment

```bash
ng build --prod
```

Deploy the `dist/` folder to web server or CDN.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request


---

⭐ **Star this repository if you found it helpful!**

