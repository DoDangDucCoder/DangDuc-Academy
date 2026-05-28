<![CDATA[# 🎓 HDNXUdemy — Nền tảng học trực tuyến

<p align="center">
  <strong>Hệ thống E-Learning full-stack được xây dựng với .NET 8 + Angular 16/17</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/Angular-16%20%2F%2017-DD0031?style=for-the-badge&logo=angular&logoColor=white" alt="Angular" />
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" alt="Redis" />
  <img src="https://img.shields.io/badge/Stripe-008CDD?style=for-the-badge&logo=stripe&logoColor=white" alt="Stripe" />
  <img src="https://img.shields.io/badge/AWS%20S3-FF9900?style=for-the-badge&logo=amazon-s3&logoColor=white" alt="AWS S3" />
</p>

---

## 📖 Giới thiệu

**HDNXUdemy** là một nền tảng học trực tuyến (LMS — Learning Management System) được phát triển theo mô hình **Udemy Clone**, cho phép giảng viên tạo & quản lý khóa học, học viên đăng ký & thanh toán online, xem video bài giảng dạng streaming (HLS), tương tác Q&A, đánh giá khóa học, và nhiều tính năng khác.

Dự án gồm **3 ứng dụng chính**:

| Thành phần | Công nghệ | Mô tả |
|---|---|---|
| **Backend** | .NET 8 Web API | RESTful API, xử lý nghiệp vụ, xác thực JWT, thanh toán Stripe, video processing |
| **Frontend** | Angular 16 | Giao diện học viên — duyệt khóa học, xem video, thanh toán, quản lý tài khoản |
| **Admin** | Angular 17 (Steex Template) | Dashboard quản trị — quản lý khóa học, người dùng, đơn hàng, thống kê |

---

## 🏗️ Kiến trúc hệ thống

```
┌──────────────────────────────────────────────────────────────────┐
│                        CLIENT LAYER                              │
│  ┌─────────────────────┐         ┌─────────────────────────┐    │
│  │  Frontend (Ang 16)  │         │    Admin (Ang 17)        │    │
│  │  Port: 4200         │         │    Port: 4201            │    │
│  └────────┬────────────┘         └──────────┬──────────────┘    │
└───────────┼──────────────────────────────────┼──────────────────┘
            │ HTTP / SignalR                   │
┌───────────▼──────────────────────────────────▼──────────────────┐
│                       API GATEWAY LAYER                         │
│  ┌──────────────────────────┐  ┌──────────────────────────────┐ │
│  │  HDNXUdemyAPI            │  │  HDNXUdemyConvertVideoAPI    │ │
│  │  Port: 5194              │  │  Port: 5148                  │ │
│  │  (Main REST API)         │  │  (Video Stream + Convert)    │ │
│  └────────┬─────────────────┘  └──────────┬───────────────────┘ │
└───────────┼────────────────────────────────┼────────────────────┘
            │                                │
┌───────────▼────────────────────────────────▼────────────────────┐
│                    SERVICE / DATA LAYER                          │
│  ┌──────────────┐ ┌──────────────┐ ┌──────────────────────────┐ │
│  │ HDNXUdemy    │ │ HDNXUdemy    │ │ HDNXUdemyModel           │ │
│  │ Services     │ │ Data (EF)    │ │ (Entities / DTOs)        │ │
│  └──────┬───────┘ └──────┬───────┘ └──────────────────────────┘ │
└─────────┼────────────────┼──────────────────────────────────────┘
          │                │
┌─────────▼────────────────▼──────────────────────────────────────┐
│                   INFRASTRUCTURE LAYER                           │
│  ┌────────────┐ ┌────────┐ ┌───────┐ ┌────────┐ ┌────────────┐ │
│  │ PostgreSQL │ │ Redis  │ │ AWS S3│ │Cloudinary│ │ Stripe     │ │
│  └────────────┘ └────────┘ └───────┘ └────────┘ └────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📁 Cấu trúc thư mục

```
UdemyClone/
├── .github/
│   └── workflows/          # CI/CD pipelines (GitHub Actions)
├── backend/                # .NET 8 Solution (5 projects)
│   ├── HDNXUdemy/           # Data Layer — Entities, DbContext, Repositories
│   ├── HDNXUdemyModel/      # Shared Models — DTOs, Enums, Constants
│   ├── HDNXUdemyServices/   # Business Logic — Services, Email, Stripe, FFmpeg
│   ├── HDNXUdemyAPI/        # Main REST API — Controllers, Middlewares
│   ├── HDNXUdemyConvertVideoAPI/  # Video Processing API — HLS Streaming
│   └── scripts/             # Utility scripts (seed data, etc.)
├── frontend/               # Angular 16 — Student-facing UI
│   ├── src/app/components/  # 37+ page components
│   ├── src/app/core/        # Guards, Services, Helpers
│   └── src/app/models/      # TypeScript models & enums
├── admin/                  # Angular 17 — Admin Dashboard
│   ├── src/app/pages/       # Dashboard, Courses, Orders, Users...
│   ├── src/app/core/        # Auth, API Services, Guards
│   └── src/app/store/       # NgRx State Management
└── README.md               # ← Bạn đang đọc file này
```

---

## ✨ Tính năng chính

### 👨‍🎓 Phía Học viên (Frontend)
- 🏠 Trang chủ với banner, khóa học nổi bật, đối tác, thống kê
- 📚 Duyệt khóa học theo danh mục, tìm kiếm, lọc
- 🎬 Xem chi tiết khóa học — giáo trình, đánh giá, giảng viên
- ▶️ Xem video bài giảng trực tuyến (ArtPlayer + HLS streaming)
- 🛒 Giỏ hàng & Thanh toán qua **Stripe**
- 👤 Quản lý tài khoản — khóa học đã mua, thông tin cá nhân
- 🔖 Bookmark / Lưu khóa học yêu thích
- 💬 Q&A — đặt câu hỏi & thảo luận trong khóa học (hỗ trợ @mention)
- ⭐ Đánh giá & nhận xét khóa học
- 🔔 Thông báo real-time (SignalR)
- 🔐 Đăng nhập / Đăng ký (Email, Google, Facebook)
- 📩 Xác minh email sau đăng ký
- 📄 Trang tĩnh — About, FAQ, Privacy Policy, Terms of Service

### 🛡️ Phía Quản trị (Admin)
- 📊 Dashboard tổng quan — thống kê doanh thu, học viên, khóa học
- 📖 Quản lý khóa học — CRUD, thêm nội dung bài giảng, upload video
- 👥 Quản lý người dùng — tài khoản, phân quyền (Admin/Teacher/Student)
- 🧾 Quản lý đơn hàng — danh sách đơn, chi tiết thanh toán
- 🏷️ Quản lý danh mục & danh mục con
- 🏞️ Quản lý Banner
- 📰 Quản lý tin tức / bài viết
- 💳 Quản lý thanh toán Stripe
- 🎟️ Quản lý Coupon & Promotion
- ⚙️ Cấu hình hệ thống
- 📁 Quản lý file upload (Cloudinary / AWS S3)

### ⚙️ Phía Backend
- 🔒 Xác thực JWT + Refresh Token
- 📡 API Versioning (v1, v2...)
- 📄 Swagger/OpenAPI documentation
- 🎬 Video processing — Convert MP4 → HLS (.m3u8 + .ts) bằng FFmpeg
- 📤 Upload lên AWS S3 / Cloudinary / Google Drive
- 💳 Tích hợp Stripe Checkout + Webhook
- 📧 Gửi email qua MailKit / SendGrid / Resend
- 🔔 Real-time notifications via SignalR Hub
- 📦 Caching với Redis
- 🗄️ Entity Framework Core + PostgreSQL (NodaTime)
- 🔄 Background Jobs với Hangfire
- 🧱 Repository Pattern + Dependency Injection
- 🛡️ Custom Exception Middleware

---

## 🔧 Yêu cầu hệ thống

| Công cụ | Phiên bản tối thiểu |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0+ |
| [Node.js](https://nodejs.org/) | Bắt buộc Node 18 LTS, ví dụ `18.20.4` |
| [Angular CLI](https://angular.io/cli) | 16.x+ (Frontend) / 17.x+ (Admin) |
| [PostgreSQL](https://www.postgresql.org/) | 15+ |
| [Redis](https://redis.io/) | 7.x+ (optional cho local demo; dùng cho cache/background jobs) |
| [FFmpeg](https://ffmpeg.org/) | 6.x+ (cho video processing) |
| [Visual Studio 2022](https://visualstudio.microsoft.com/) hoặc [Rider](https://www.jetbrains.com/rider/) | Mới nhất |
| [VS Code](https://code.visualstudio.com/) | Mới nhất |

---

## 🚀 Hướng dẫn cài đặt & chạy

### 1. Clone repository

```bash
git clone https://github.com/<your-username>/UdemyClone.git
cd UdemyClone
```

### 2. Khởi chạy Backend

```bash
cd backend

# Restore NuGet packages
dotnet restore

# Cập nhật connection string trong appsettings.Development.json
# PostgreSQL: server=localhost;Database=HDNXUdemy;Username=postgres;Password=<your_password>;
# Keys cần sửa: AppConfig:DefaultConnection và Serilog:ConnectionStrings:DefaultConnection

# Chỉ chạy migration khi tạo DB mới từ migrations.
# Không chạy migration lên DB đã restore từ backup nếu schema đã đúng.
dotnet ef database update --project HDNXUdemy --startup-project HDNXUdemyAPI

# Chạy Main API (port 5194)
dotnet run --project HDNXUdemyAPI

# Chạy Video API (port 5148) — mở terminal khác
dotnet run --project HDNXUdemyConvertVideoAPI
```

### 3. Khởi chạy Frontend

```bash
cd frontend

# Cài đặt dependencies
npm install

# Chạy dev server (port 4200)
ng serve
# hoặc
npm start
```

### 4. Khởi chạy Admin

```bash
cd admin

# Cài đặt dependencies
npm install

# Chạy dev server (port 4201)
npm start -- --host 127.0.0.1 --port 4201
```

### 5. Truy cập ứng dụng

| Ứng dụng | URL |
|---|---|
| Frontend (Học viên) | http://localhost:4200 |
| Admin Dashboard | http://127.0.0.1:4201/ |
| Main API (Swagger) | http://localhost:5194/swagger |
| Video API (Swagger) | http://localhost:5148/swagger |

---

## Local Demo Readiness

### Ports
| Service | URL/Port |
|---|---|
| Frontend | http://127.0.0.1:4200 |
| Admin | http://127.0.0.1:4201/ |
| Backend API | http://localhost:5194 |
| ConvertVideo API | http://localhost:5148 |
| PostgreSQL | localhost:5432 |
| Redis | localhost:6379 |

Redis is optional for the local demo path that only browses courses, logs in, and uses core CRUD. Enable Redis when testing cache/background jobs/Hangfire:

```bash
docker compose up -d redis
redis-cli -h localhost -p 6379 ping
```

Expected Redis check: `PONG`.

### Environment

Copy `.env.example` and replace all `CHANGE_ME_*` placeholders before running outside local demo. `JWT_SECRET` must be at least 32 bytes. Do not commit real Sentry DSN, Stripe secret key, Cloudinary secret, AWS secret, Google client secret, or SMTP credentials.

Important local paths/settings:
- PostgreSQL connection strings: `AppConfig:DefaultConnection` and `Serilog:ConnectionStrings:DefaultConnection`.
- Video source path: `AppConfig:DiskBaseForVideo`, default `C:\VideoLMS`.
- Converted HLS folders: `Folder:StorageMainVideo` and `Folder:StorageStreamVideo`.
- File upload folder: `Folder:UploadSoftWareAndFile`.
- Stripe/Cloudinary/OAuth keys are placeholders in `appsettings*.json`; configure through local environment/appsettings before testing those external flows.

### Database Backup And Restore

This repo expects a PostgreSQL backup file such as `hdnx_udemy_backup_YYYYMMDD.sql`.

Create database and restore the `hdnx_udemy_backup_final_20260529.sql` final local demo backup:

```bash
createdb -h localhost -p 5432 -U postgres HDNXUdemy
psql -h localhost -p 5432 -U postgres -d HDNXUdemy -f hdnx_udemy_backup_final_20260529.sql
```

If the database already exists, restore into a clean local database created for demo. Do not run destructive reset commands against real data. After restoring this backup, do not run EF migrations unless you have confirmed the restored schema is missing required migrations.

### SPA Deep Links

The Angular dev server may return 404 when opening a deep link directly, for example `/courses/1` or `/auth/login`. For local demo, open the app root first and navigate inside the app:

- Frontend: `http://127.0.0.1:4200/`
- Admin: `http://127.0.0.1:4201/`

If direct URLs must work, configure the dev/proxy/static host to fall back to `index.html` for unknown SPA routes.

### Demo Accounts

Seed/demo script reference: `backend/scripts/seed-demo-data.ps1`.

| Role | Email | Password |
|---|---|---|
| Admin | `admin@themesbrand.com` | `123456` |
| Student | `student@hdnx.local` | `123456` |

If the restored database was not generated from the seed script, verify the student account password with the project owner before the demo.

### Known Local Demo Risks

- JWT logout is stateless; logout clears client-side token, but already issued JWTs remain valid until expiry unless a server-side denylist is added.
- ASP.NET Core DataProtection keys are stored under each API project's local `DataProtectionKeys/` folder for local runs; this avoids machine-specific `%LOCALAPPDATA%` warnings without requiring an absolute path.
- Dependency audit currently has no critical findings, but has high/moderate findings in Angular/CKEditor/Firebase/toolchain and transitive .NET packages. Accepted only for local demo; upgrade packages before public production.
- Redis may be offline for basic local demo; enable it for cache/background-job verification.
- Do not include `node_modules/`, `dist/`, `bin/`, `obj/`, `.vs/`, `.codex-run/`, `qa-artifacts/`, `logs/`, or `*.log` in source zip unless specifically requested.

---

## 🌐 Deployment (Production)

| Ứng dụng | URL Production |
|---|---|
| Frontend | https://web-hdnx.devproinsights.com |
| Admin | https://hdnx-admin.devproinsights.com |
| Main API | https://hdnxapi.devproinsights.com |
| Video API | https://hdnxvideo.devproinsights.com |

### Build Production

```bash
# Frontend
cd frontend && ng build --configuration=production

# Admin  
cd admin && ng build --configuration=production

# Backend
cd backend && dotnet publish -c Release
```

---

## 🛠️ Tech Stack tổng hợp

### Backend
| Công nghệ | Mục đích |
|---|---|
| .NET 8 | Runtime & Web API |
| Entity Framework Core 7 | ORM + PostgreSQL |
| AutoMapper | Object mapping |
| JWT Bearer | Authentication |
| Stripe.NET | Payment processing |
| AWS SDK (S3) | File storage |
| CloudinaryDotNet | Image/video CDN |
| Google Drive API | File backup |
| MailKit + SendGrid | Email services |
| SignalR | Real-time communication |
| Hangfire | Background job processing |
| Redis | Distributed caching |
| Xabe.FFmpeg | Video transcoding |
| Swashbuckle | Swagger documentation |
| NodaTime | Timezone-aware datetime |

### Frontend & Admin
| Công nghệ | Mục đích |
|---|---|
| Angular 16 / 17 | SPA Framework |
| Bootstrap 5 | CSS Framework |
| NgRx | State management (Admin) |
| ng-bootstrap / ngx-bootstrap | UI Components |
| ArtPlayer | Video player (HLS) |
| SweetAlert2 | Dialog/notification |
| ngx-owl-carousel-o | Carousel slider |
| Stripe.js | Payment UI |
| @microsoft/signalr | Real-time notifications |
| ApexCharts / ECharts | Charts (Admin) |
| FullCalendar | Calendar (Admin) |
| ngx-toastr | Toast notifications (Admin) |
| @ngx-translate | i18n (Admin) |
| CKEditor 5 | Rich text editor (Admin) |

---

## 📂 Tài liệu chi tiết từng phần

- 📘 [Backend README](./backend/README.md) — Chi tiết kiến trúc API, cấu hình, entities
- 📗 [Frontend README](./frontend/README.md) — Chi tiết giao diện học viên, components, routing
- 📙 [Admin README](./admin/README.md) — Chi tiết dashboard quản trị, modules, state management

---

## 👥 Vai trò người dùng

| Vai trò | Mô tả | Quyền hạn |
|---|---|---|
| **Admin** | Quản trị viên hệ thống | Toàn quyền — quản lý khóa học, users, đơn hàng, cấu hình |
| **Teacher** | Giảng viên | Tạo & quản lý khóa học riêng, upload video, xem thống kê |
| **Student** | Học viên | Mua khóa học, xem video, đánh giá, Q&A |
| **User** | Người dùng mới | Duyệt khóa học, đăng ký tài khoản |

---

## 💳 Luồng thanh toán (Stripe)

```
Học viên chọn khóa học → Thêm vào giỏ hàng → Checkout
    → Tạo Stripe Session → Redirect sang Stripe
        → Thanh toán thành công → Webhook callback
            → Cập nhật PurcharseCourse → Mở quyền xem khóa học
        → Thanh toán thất bại → Redirect trang lỗi
```

---

## 🎬 Luồng xử lý video

```
Teacher upload MP4 → HDNXUdemyConvertVideoAPI nhận file
    → Lưu vào disk (DiskBaseForVideo)
    → FFmpeg chuyển đổi MP4 → HLS (.m3u8 + .ts segments)
    → Upload lên AWS S3 / serve từ local
    → Cập nhật trạng thái: Open → Process → Finish → Public
    → Học viên xem video qua ArtPlayer (HLS)
```

---

## 📄 License

Dự án này được phát triển cho mục đích học tập và nghiên cứu.

---

<p align="center">
  Made with ❤️ by <strong>HDNX Team</strong>
</p>
]]>
