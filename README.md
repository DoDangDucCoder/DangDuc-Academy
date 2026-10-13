# HDNXUdemy Clone - Full Stack E-Learning Platform

## 📋 Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend API** | .NET 8, Entity Framework Core, PostgreSQL |
| **Frontend** | Angular 17, SCSS, Bootstrap |
| **Admin Panel** | Angular 17, NgRx, Bootstrap |
| **Real-time** | SignalR |
| **Authentication** | JWT Bearer + Refresh Tokens |
| **File Storage** | Cloudinary |
| **Payment** | Stripe |
| **Containerization** | Docker, Docker Compose |

---

## 🚀 Getting Started

### Environment Keys

Legacy public keys have been removed from source. Configure new Google, Facebook, Firebase, Maps, and Stripe publishable keys using the placeholders documented in [Environment Keys](docs/environment-keys.md).

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) & npm
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Docker](https://www.docker.com/) (optional, for containerized setup)

### 1. Clone & Setup

```bash
git clone <repository-url>
cd UdemyClone
```

### 2. Backend Setup

```bash
cd backend/HDNXUdemyAPI

# Restore packages
dotnet restore

# Update connection string in appsettings.json
# "ConnectionStrings": { "DefaultConnection": "Host=localhost;Port=5432;Database=hdnx_udemy;Username=postgres;Password=your_password" }

# Run migrations
dotnet ef database update

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
docker-compose up -d
```

The Compose stack starts PostgreSQL, Redis, Elasticsearch, the backend API, video API, frontend, and admin dashboard. Frontend is exposed on `http://localhost`, admin on `http://localhost:81`, backend on `http://localhost:5194`, video API on `http://localhost:5195`, and Elasticsearch on `http://localhost:9200`.

### 6. Kubernetes Base

Phase 5 Kubernetes manifests live in `k8s/base` and can be rendered with:

```powershell
kubectl kustomize k8s/base
```

Create the `hdnx-secrets` secret before applying the base, then deploy with:

```powershell
kubectl apply -k k8s/base
```

---

## 🔑 API Keys & External Services Configuration

### Google OAuth (Required for Google Login)

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Navigate to **APIs & Services** → **Credentials**
4. Click **Create Credentials** → **OAuth 2.0 Client ID**
5. Choose **Web Application**
6. Add Authorized redirect URIs:
   - `http://localhost:4200` (development)
   - `https://your-domain.com` (production)
7. Copy the **Client ID**
8. Update in the following files:

**Frontend** - `frontend/src/environments/environment.ts`:
```typescript
clientIdGoogle: 'YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com'
```

**Backend** - `backend/HDNXUdemyAPI/appsettings.json`:
```json
"Authentication": {
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  }
}
```

---

### Stripe Payment (Required for Course Purchases)

1. Go to [Stripe Dashboard](https://dashboard.stripe.com/)
2. Sign up / Log in
3. Navigate to **Developers** → **API Keys**
4. Copy the **Publishable Key** (pk_test_...) and **Secret Key** (sk_test_...)

**Frontend** - `frontend/src/environments/environment.ts`:
```typescript
stripePublicKey: 'pk_test_YOUR_STRIPE_PUBLISHABLE_KEY'
```

**Backend** - `backend/HDNXUdemyAPI/appsettings.json`:
```json
"Stripe": {
  "SecretKey": "sk_test_YOUR_STRIPE_SECRET_KEY",
  "PublishableKey": "pk_test_YOUR_STRIPE_PUBLISHABLE_KEY"
}
```

---

### Cloudinary (Required for Image/Video Uploads)

1. Go to [Cloudinary Console](https://cloudinary.com/console)
2. Sign up / Log in
3. Copy **Cloud Name**, **API Key**, and **API Secret** from the dashboard

**Backend** - `backend/HDNXUdemyAPI/appsettings.json`:
```json
"Cloudinary": {
  "CloudName": "YOUR_CLOUD_NAME",
  "ApiKey": "YOUR_API_KEY",
  "ApiSecret": "YOUR_API_SECRET"
}
```

---

### SMTP Email (Required for Forgot Password)

The forgot password feature sends OTP codes via email. Configure your SMTP provider:

**Option A: Gmail SMTP**
1. Enable [2-Step Verification](https://myaccount.google.com/security)
2. Generate an [App Password](https://myaccount.google.com/apppasswords)
3. Use the generated password in config

**Option B: Any SMTP Provider** (Mailgun, SendGrid, etc.)

**Backend** - `backend/HDNXUdemyAPI/appsettings.json`:
```json
"EmailSettings": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "SmtpUser": "your-email@gmail.com",
  "SmtpPassword": "your-app-password",
  "FromEmail": "your-email@gmail.com",
  "FromName": "HDNX Academy"
}
```

> **Note:** Currently the Forgot Password feature generates an OTP token stored in the database. The OTP is returned in the API response for development. In production, integrate the SMTP settings above to send the OTP via email.

---

### Facebook Login (Optional)

1. Go to [Facebook Developers](https://developers.facebook.com/)
2. Create an App → Choose **Consumer** type
3. Add **Facebook Login** product
4. Copy the **App ID**

**Frontend** - `frontend/src/environments/environment.ts`:
```typescript
facebookId: 'YOUR_FACEBOOK_APP_ID'
```

---

## 📁 Project Structure

```
UdemyClone/
├── backend/
│   ├── HDNXUdemyAPI/          # Web API project (.NET 8)
│   ├── HDNXUdemyServices/     # Business logic layer
│   ├── HDNXUdemy/             # Data access layer (EF Core)
│   └── HDNXUdemyModel/        # DTOs, Models, Request/Response
├── frontend/                  # Angular 17 Student UI
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
