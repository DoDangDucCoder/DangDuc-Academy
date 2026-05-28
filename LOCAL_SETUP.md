# Local setup

1. Copy `.env.example` to `.env` and fill the values.
2. Run `docker compose up -d` to start PostgreSQL and Redis.
3. Open `backend/HDNXUdemyAPI` and `backend/HDNXUdemyConvertVideoAPI` in `dotnet user-secrets`.
4. Set the same secrets there instead of keeping them in `appsettings*.json`.
5. Run:
   - `dotnet run --project backend/HDNXUdemyAPI`
   - `dotnet run --project backend/HDNXUdemyConvertVideoAPI`
   - `npm start` in `frontend`
   - `npm start` in `admin`
