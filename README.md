# DangDuc Academy - Full Stack E-Learning Platform

## Overview

DangDuc Academy is a full-stack e-learning platform for course management and video-based learning. It supports course discovery, user authentication, lesson tracking, and online video streaming.

This personal project demonstrates backend architecture, REST API development, relational database design, and video streaming implementation.

## 📋 Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend API** | ASP.NET Core 8 Web API, Entity Framework Core, PostgreSQL |
| **Frontend** | Angular 16, SCSS, Bootstrap |
| **Admin Panel** | Angular 17, NgRx, Bootstrap |
| **Real-time** | SignalR |
| **Authentication** | JWT access tokens and refresh tokens |
| **File Storage** | AWS S3, Cloudinary |
| **Payment** | Stripe |
| **Containerization** | Docker, Docker Compose |

---

## 🚀 Getting Started

### Environment Keys

For guidance on external key handling, see [Environment Keys](docs/environment-keys.md). Do not commit real credentials to the repository.

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) & npm
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/) (optional, for containerized setup)

Run each setup section from the repository root. Use separate terminals when running the backend and frontend at the same time.

### 1. Clone & Setup

```bash
git clone https://github.com/DoDangDucCoder/DangDuc-Academy.git
cd DangDuc-Academy
```

### 2. Backend Setup

```bash
cd backend/HDNXUdemyAPI

# Restore packages
dotnet restore

# Set AppConfig:DefaultConnection to your local PostgreSQL connection string.

# If the EF CLI is not installed
dotnet tool install --global dotnet-ef --version 8.0.11

# Run migrations
dotnet ef database update --project ../HDNXUdemy/HDNXUdemyData.csproj --startup-project .

# Run the API
dotnet run
# API will be available at http://localhost:5194
```

### 3. Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Start development server
ng serve
# App will be available at http://localhost:4200
```

### 4. Admin Panel Setup

```bash
cd admin

# Install dependencies
npm install

# Start development server
ng serve --port 4201
# Admin will be available at http://localhost:4201
```

### 5. Docker Setup (Alternative)

```bash
# From root directory
docker compose up -d
```

The Compose stack starts PostgreSQL, Redis, Elasticsearch, the backend API, video API, frontend, and admin dashboard. Frontend is exposed on `http://localhost`, admin on `http://localhost:81`, backend on `http://localhost:5194`, video API on `http://localhost:5195`, and Elasticsearch on `http://localhost:9200`.

### 6. Kubernetes Base

Phase 5 Kubernetes manifests live in `k8s/base` and can be rendered with:

```powershell
kubectl kustomize k8s/base
```

Use `k8s/base/secret.example.yaml` as a template to create the `hdnx-secrets` secret. Replace its sample values before applying the base, then deploy with:

```powershell
kubectl apply -k k8s/base
```

---

## 🔑 API Keys & External Services Configuration

### Stripe Payment (Required for Course Purchases)

1. Go to [Stripe Dashboard](https://dashboard.stripe.com/)
2. Sign up / Log in
3. Navigate to **Developers** → **API Keys**
4. Copy the publishable and secret keys from the Stripe Dashboard.
5. Configure the frontend publishable key and the backend `AppConfig:StripePublishableKey` and `AppConfig:StripeSecretKey` values through local or deployment configuration. See [Environment Keys](docs/environment-keys.md), and do not commit credentials.

---

### Cloudinary (Optional File Storage Provider)

1. Go to [Cloudinary Console](https://cloudinary.com/console)
2. Sign up / Log in
3. Copy **Cloud Name**, **API Key**, and **API Secret** from the dashboard

**Backend** - `backend/HDNXUdemyAPI/appsettings.json`:
```json
"CloudinanySettings": {
  "CloudName": "YOUR_CLOUD_NAME",
  "APIKey": "YOUR_API_KEY",
  "APISecret": "YOUR_API_SECRET"
}
```

---

## 📁 Project Structure

```
DangDuc-Academy/
├── backend/
│   ├── HDNXUdemyAPI/          # Web API project (.NET 8)
│   ├── HDNXUdemyConvertVideoAPI/ # Video processing and streaming API
│   ├── HDNXUdemyServices/     # Business logic layer
│   ├── HDNXUdemy/             # Data access layer (EF Core)
│   └── HDNXUdemyModel/        # DTOs, Models, Request/Response
├── frontend/                  # Angular 16 Student UI
├── admin/                     # Angular 17 Admin Dashboard
├── docker-compose.yml
└── README.md
```

## 🎯 Features

### Student Frontend
- 🔐 JWT Authentication (Register/Login/Forgot Password)
- 🌙 Dark Mode toggle
- 🔍 Live course search with autocomplete
- 📚 Course browsing by category
- 📊 Student progress tracking with visual progress bars
- 🛒 Shopping cart & checkout (Stripe)
- 🔔 Real-time notifications (SignalR)
- ⭐ Course ratings & reviews
- 📑 Bookmark courses
- 👨‍🏫 Training online (live sessions)

### Admin Panel
- 📊 Dashboard with analytics
- 📚 Course CRUD management
- 👥 User management
- 🌙 Dark mode (Light/Dark/Auto)
- 📥 Export data (CSV/Excel)
- 🔔 Notification management
- 💰 Purchase order tracking

---

## 🏥 Health Checks

The API exposes health check endpoints:
- `GET /health/live` - API liveness probe
- `GET /health/ready` - Database readiness probe

---

## 📝 License

This project is for educational purposes.
