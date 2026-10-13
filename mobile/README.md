# Mobile App

Phase 5 mobile starts as an Expo React Native app pointed at the existing backend API.

## Run

```powershell
cd mobile
npm install
$env:EXPO_PUBLIC_API_BASE_URL="https://your-api-host.example"
npm run start
```

For local development, point `EXPO_PUBLIC_API_BASE_URL` to the backend host reachable from the device or emulator. The app intentionally does not hardcode localhost so staging/production builds can use environment-specific API hosts.

For Android emulator access to a host machine API, use `http://10.0.2.2:5194` instead of `localhost`.
