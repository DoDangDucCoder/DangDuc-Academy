<![CDATA[# ⚙️ HDNXUdemy Backend — .NET 8 Web API

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/EF%20Core-7.0.18-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" alt="Redis" />
  <img src="https://img.shields.io/badge/Stripe-008CDD?style=for-the-badge&logo=stripe&logoColor=white" alt="Stripe" />
  <img src="https://img.shields.io/badge/AWS%20S3-FF9900?style=for-the-badge&logo=amazon-s3&logoColor=white" alt="AWS S3" />
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" alt="Swagger" />
</p>

---

## 📖 Giới thiệu

Backend của HDNXUdemy được xây dựng trên **.NET 8** với kiến trúc **Clean Architecture (N-Layer)**, sử dụng **Entity Framework Core** kết nối **PostgreSQL**, hỗ trợ đầy đủ các tính năng: xác thực JWT, thanh toán Stripe, upload file đa nền tảng (AWS S3, Cloudinary, Google Drive), xử lý video HLS streaming bằng FFmpeg, real-time notification qua SignalR, background jobs với Hangfire, và caching với Redis.

Backend bao gồm **2 API servers** chạy độc lập:
- **HDNXUdemyAPI** (port `5194`): Main REST API — xử lý toàn bộ nghiệp vụ chính
- **HDNXUdemyConvertVideoAPI** (port `5148`): Video Processing API — upload, convert, stream video

---

## 🏗️ Kiến trúc Solution

```
HDNXUdemy.sln
│
├── HDNXUdemyAPI/              ← 🌐 Main Web API (Startup project)
│   ├── Controllers/           # API Controllers (10 controllers)
│   ├── Middlewares/            # Custom Exception Middleware
│   ├── Mapper/                 # AutoMapper Profiles
│   ├── ModelHelp/              # Helper models
│   ├── ProjectExtensisons/     # DI, Swagger, JWT Extensions
│   ├── Properties/             # Launch settings
│   ├── wwwroot/                # Static files, email templates
│   ├── Program.cs              # Application entry point
│   ├── appsettings.json        # Production config
│   └── appsettings.Development.json  # Dev config
│
├── HDNXUdemyConvertVideoAPI/  ← 🎬 Video Processing API
│   ├── Controllers/           # 4 controllers (Auth, Upload, Stream)
│   ├── Middlewares/            # Exception handling
│   ├── Mapper/                 # AutoMapper Profiles
│   ├── Program.cs              # Entry point (with Hangfire)
│   └── appsettings.json        # Config
│
├── HDNXUdemyData/             ← 🗄️ Data Access Layer
│   ├── Entities/              # 23 Entity classes
│   ├── EntitiesContext/        # ProjectContext (DbContext)
│   ├── GenericRepository/      # Generic CRUD Repository
│   ├── IRepository/            # Repository Interfaces
│   ├── Repository/             # Repository Implementations
│   ├── Migrations/             # EF Core Migrations
│   └── SeedData/               # Seed data scripts
│
├── HDNXUdemyModel/            ← 📦 Shared Models
│   ├── Base/                  # Base models, config helpers
│   ├── Constant/              # Enums, Constants, Messages
│   ├── Model/                  # Domain models
│   ├── RequestModel/           # Request DTOs
│   ├── ResponModel/            # Response DTOs
│   └── SystemExceptions/       # Custom exception classes
│
├── HDNXUdemyServices/         ← 🔧 Business Logic Layer
│   ├── IServices/             # 14 Service Interfaces
│   ├── Services/              # 14 Service Implementations
│   ├── CommonFunction/         # Shared utilities, SignalR Hub
│   └── RecuringJob/            # Hangfire recurring jobs
│
└── scripts/
    └── seed-demo-data.ps1     # PowerShell script seed dữ liệu demo
```

---

## 📡 API Controllers

### HDNXUdemyAPI — Main API

| Controller | Endpoint Prefix | Mô tả |
|---|---|---|
| `AuthenticationController` | `/api/v1/authentication` | Đăng nhập, đăng ký, xác minh email, Google/Facebook OAuth |
| `CourseController` | `/api/v1/course` | CRUD khóa học, nội dung bài giảng, đánh giá, Q&A, bookmark |
| `StudentController` | `/api/v1/student` | Quản lý học viên, tiến độ học, thông tin cá nhân |
| `MasterController` | `/api/v1/master` | Quản lý danh mục, banner, tin tức, cấu hình hệ thống |
| `PurchaseCourseController` | `/api/v1/purchase-course` | Đơn hàng, lịch sử mua, chi tiết thanh toán |
| `PromotionOfCourseController` | `/api/v1/promotion` | Coupon, mã giảm giá, chương trình khuyến mãi |
| `UploadToCloudController` | `/api/v1/upload-cloud` | Upload file lên AWS S3 / Cloudinary |
| `HomeController` | `/api/v1/home` | Dữ liệu trang chủ (thống kê, featured courses) |
| `WebhookController` | `/api/v1/webhook` | Stripe webhook handler |
| `BaseController` | — | Base controller (common logic) |

### HDNXUdemyConvertVideoAPI — Video API

| Controller | Endpoint Prefix | Mô tả |
|---|---|---|
| `AuthenticationController` | `/api/v1/authentication` | Xác thực cho video API |
| `UploadVideoDataToServerController` | `/api/v1/upload-video` | Upload & convert video MP4 → HLS |
| `GetStreamFileController` | `/api/v1/get-video-stream` | Stream video HLS (.m3u8 / .ts) |
| `BaseController` | — | Base controller |

---

## 🗄️ Database Entities (23 bảng)

| Entity | Mô tả |
|---|---|
| `Users` | Người dùng — Admin, Teacher, Student |
| `Course` | Khóa học — tiêu đề, mô tả, giá, giảng viên |
| `ContentCourse` | Phần/chương trong khóa học |
| `ContentCourseDetail` | Bài giảng chi tiết (video, tài liệu) |
| `Category` | Danh mục khóa học |
| `SubCategory` | Danh mục con |
| `PurcharseCourse` | Đơn hàng mua khóa học |
| `PurcharseCourseDetails` | Chi tiết từng khóa học trong đơn hàng |
| `CourseEvaluation` | Đánh giá & xếp hạng khóa học |
| `TheadQuestionCourse` | Câu hỏi Q&A trong khóa học |
| `DetailTheadQuestionCourse` | Câu trả lời cho Q&A |
| `BookmarkCourse` | Bookmark / lưu khóa học |
| `StudentProcess` | Tiến độ học của học viên |
| `StudentPromotion` | Mã khuyến mãi đã dùng |
| `Coupon` | Mã giảm giá |
| `PromotionCode` | Chương trình khuyến mãi |
| `Banner` | Banner quảng cáo trang chủ |
| `Partner` | Đối tác hiển thị |
| `Notification` | Thông báo hệ thống |
| `FileManager` | Quản lý file upload |
| `InformationManualBanking` | Thông tin chuyển khoản |
| `SystemConfig` | Cấu hình hệ thống |
| `BaseEntities` | Base class — Id, CreatedDate, UpdatedDate, IsDeleted |

---

## 🔧 Services Layer (14 services)

| Service | Interface | Mô tả |
|---|---|---|
| `AuthenticationServices` | `IAuthenticationServices` | Đăng nhập, đăng ký, xác minh, JWT token |
| `CourseServices` | `ICourseServices` | CRUD khóa học, nội dung, đánh giá, Q&A |
| `StudentServices` | `IStudentServices` | Quản lý học viên, tiến độ, bookmark |
| `MasterDataServices` | `IMasterDataServices` | Danh mục, banner, tin tức, cấu hình |
| `PurcharseCourseServices` | `IPurcharseCourseServices` | Xử lý đơn hàng, thanh toán |
| `StripeServices` | `IStripeServices` | Tích hợp Stripe Checkout & Webhook |
| `EmailServices` | `IEmailServices` | Gửi email hệ thống |
| `SendEmailSMSServices` | `ISendEmailSMSServices` | MailKit / SendGrid / Resend |
| `UploadDataToCloud` | `IUploadDataToCloud` | Upload AWS S3 / Cloudinary |
| `UploadFileVideoToServer` | `IUploadFileVideoToServer` | Upload & convert video |
| `HomeServices` | `IHomeServices` | Dữ liệu trang chủ |
| `LogServices` | `ILogServices` | Logging & audit trail |
| `ClientAPIServices` | `IClientAPIServices` | HTTP client wrapper |
| `ConfigClientAPIServices` | `IConfigClientAPIServices` | Cấu hình HTTP client |

---

## 🔒 Authentication & Authorization

### JWT Authentication Flow
```
Client gửi credentials → API validate → Tạo JWT Token
    → Token chứa: UserId, Role, Email, Expiry
    → Client đính kèm token trong header: Authorization: Bearer <token>
    → Middleware giải mã & xác thực token
    → Phân quyền theo Role: Admin, Teacher, Student, User
```

### Đăng nhập Social (OAuth)
- **Google** — GoogleLoginProvider (OAuth 2.0)
- **Facebook** — FacebookLoginProvider
- **Email** — Đăng ký truyền thống + xác minh email

---

## 💳 Tích hợp Stripe

```
                           ┌─────────────────┐
                           │   Stripe Server  │
                           └────────┬─────────┘
                                    │
           ┌────────────────────────┼────────────────────────┐
           │                        │                        │
    Create Session            Webhook Event          Redirect URL
           │                        │                        │
    ┌──────▼──────┐         ┌───────▼────────┐       ┌───────▼──────┐
    │ Purchase    │         │  Webhook       │       │ Payment      │
    │ Controller  │         │  Controller    │       │ Success/Error│
    └─────────────┘         └────────────────┘       └──────────────┘
```

**Cấu hình Stripe:**
- `StripePublishableKey` — Public key cho frontend
- `StripeSecretKey` — Secret key cho backend
- `StripeWebHookKey` — Webhook signing secret

---

## 🎬 Video Processing Pipeline

```
1. Teacher upload file MP4
2. API nhận file → lưu vào DiskBaseForVideo (C:\VideoLMS)
3. Hangfire Job trigger → FFmpeg convert:
   - Input:  video.mp4
   - Output: video/master.m3u8 + segment_000.ts, segment_001.ts, ...
4. Cập nhật trạng thái: Open → Process → Finish → Public
5. Serve video qua GetStreamFileController:
   - GET /api/v1/get-video-stream/stream/{filename}     → .m3u8
   - GET /api/v1/get-video-stream/stream/mp4/{filename}  → .mp4
6. Static file middleware cấu hình MIME types:
   - .m3u8 → application/x-mpegURL
   - .ts   → video/mp2t
```

---

## 📡 Real-time (SignalR)

```csharp
// Hub endpoint: /hub-notification
// Sử dụng cho:
// - Thông báo khi có comment mới trên khóa học
// - Thông báo khi bị @tag trong Q&A
// - Thông báo cập nhật khóa học
// - Thông báo khuyến mãi
// - Thông báo hệ thống
```

**Notification Types:**
- `CommentOnCourse` — Comment mới
- `TagOnCommentOnCourse` — Được @mention
- `UpdateOnCourse` — Khóa học cập nhật
- `DiscountOnCourse` — Giảm giá khóa học
- `PromotionOnCourse` — Khuyến mãi
- `UpgradeSystemOnCourse` — Thông báo hệ thống

---

## ⚙️ Cấu hình (appsettings.json)

| Section | Mô tả |
|---|---|
| `AppSettings.Secret` | JWT secret key |
| `AppSettings.ExpiresDate` | JWT expiry (days) |
| `AppConfig:DefaultConnection` | PostgreSQL connection string |
| `AppConfig.Schema` | Database schema name |
| `AppConfig.APIUrlGetVideoStream` | Video stream base URL |
| `AppConfig.DiskBaseForVideo` | Local path lưu video |
| `CloudServicesAWS` | AWS S3 credentials & bucket |
| `CloudinanySettings` | Cloudinary API credentials |
| `MailSettings` | SMTP email config (Gmail) |
| `Redis` | Redis host, port, password |
| `Stripe*` | Stripe keys & webhook secret |
| `EmailTemplate` | Email template filenames |
| `Folder` | Storage folder names |

---

## 📦 NuGet Packages chính

### HDNXUdemyAPI
| Package | Version | Mục đích |
|---|---|---|
| `Asp.Versioning.Mvc` | 7.1.0 | API versioning |
| `AutoMapper` | 15.1.1 | Object mapping |
| `AWSSDK.S3` | 3.7.305.7 | AWS S3 upload |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 7.0.18 | JWT auth |
| `Stripe.net` | 43.20.0 | Stripe integration |
| `Swashbuckle.AspNetCore` | 6.5.0 | Swagger UI |
| `Microsoft.EntityFrameworkCore.Design` | 7.0.18 | EF Core migrations |

### HDNXUdemyServices
| Package | Version | Mục đích |
|---|---|---|
| `CloudinaryDotNet` | 1.23.0 | Cloudinary CDN |
| `Google.Apis.Drive.v3` | 1.65.0 | Google Drive API |
| `Hangfire.Core` | 1.8.7 | Background jobs |
| `MailKit` | 4.16.0 | Email sending (SMTP) |
| `SendGrid` | 9.28.1 | SendGrid email |
| `Xabe.FFmpeg` | 5.2.6 | Video transcoding |
| `StackExchange.Redis` | 7.0.18 | Redis caching |
| `SignalR.Core` | 1.1.0 | Real-time hub |

### HDNXUdemyData
| Package | Version | Mục đích |
|---|---|---|
| `Npgsql.EntityFrameworkCore.PostgreSQL.NodaTime` | 7.0.18 | PostgreSQL + NodaTime |
| `EFCore.BulkExtensions` | 7.1.6 | Bulk operations |

### HDNXUdemyConvertVideoAPI
| Package | Version | Mục đích |
|---|---|---|
| `Hangfire.AspNetCore` | 1.8.7 | Background job server |
| `Hangfire.Redis.StackExchange` | 1.9.3 | Hangfire storage (Redis) |

---

## 🚀 Hướng dẫn chạy

### Prerequisites
- .NET 8 SDK
- PostgreSQL 15+
- Redis server
- FFmpeg (trong PATH)

### 1. Cấu hình Database

```bash
# Tạo database PostgreSQL
createdb HDNXUdemy

# Cập nhật connection string trong:
# HDNXUdemyAPI/appsettings.Development.json
# → AppConfig:DefaultConnection
# → Serilog:ConnectionStrings:DefaultConnection
```

### 2. Restore backup hoặc chạy migrations

```bash
cd backend

# Restore rehearsal backup vào DB local mới
createdb -h localhost -p 5432 -U postgres HDNXUdemy
psql -h localhost -p 5432 -U postgres -d HDNXUdemy -f hdnx_udemy_backup_final_20260529.sql

# Chỉ dùng khi tạo schema từ migrations, không chạy lên DB đã restore nếu không cần
dotnet ef database update --project HDNXUdemy --startup-project HDNXUdemyAPI
```

ASP.NET Core DataProtection có thể warning về key lưu dưới `%LOCALAPPDATA%`; warning này không block local demo. Cấu hình key path riêng nếu cần key ổn định qua nhiều máy/container.

### 3. Seed Data (tùy chọn)

```powershell
# Chạy script seed dữ liệu demo
cd backend/scripts
.\seed-demo-data.ps1
```

### 4. Khởi chạy API

```bash
# Terminal 1 — Main API
dotnet run --project HDNXUdemyAPI
# → http://localhost:5194
# → Swagger: http://localhost:5194/swagger

# Terminal 2 — Video API
dotnet run --project HDNXUdemyConvertVideoAPI
# → http://localhost:5148
# → Swagger: http://localhost:5148/swagger
```

---

## 🧪 API Testing

Swagger UI có sẵn ở môi trường Development:
- **Main API:** http://localhost:5194/swagger
- **Video API:** http://localhost:5148/swagger

API versioning format: `v1`, `v2`, ...  
Example: `GET /api/v1/course/list`

---

## 🌐 CORS Configuration

Các origin được phép:
```
http://localhost:4200          ← Frontend dev
http://localhost:4201          ← Admin dev
https://web-hdnx.devproinsights.com    ← Frontend production
https://hdnx-admin.devproinsights.com  ← Admin production
```

---

## 📄 Tham khảo thêm

- [📘 Main README](../README.md) — Tổng quan dự án
- [📗 Frontend README](../frontend/README.md) — Giao diện học viên
- [📙 Admin README](../admin/README.md) — Dashboard quản trị

---

<p align="center">
  Made with ❤️ by <strong>HDNX Team</strong>
</p>
]]>
